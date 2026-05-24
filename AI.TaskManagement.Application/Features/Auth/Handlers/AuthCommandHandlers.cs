using MediatR;
using AI.TaskManagement.Application.DTOs.Auth;
using AI.TaskManagement.Application.Features.Auth.Commands;
using AI.TaskManagement.Domain.Entities;
using AI.TaskManagement.Domain.Interfaces;
using AI.TaskManagement.Infrastructure.Services.Auth;
using AI.TaskManagement.Shared.Exceptions;
using AI.TaskManagement.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AI.TaskManagement.Application.Features.Auth.Handlers;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public RegisterUserCommandHandler(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var userRepo = _unitOfWork.Repository<User>();
        var roleRepo = _unitOfWork.Repository<Role>();

        // Check if user already exists
        var existingUser = await userRepo.GetAllAsync();
        if (existingUser.Any(u => u.Email == request.Email))
            throw new ConflictException("User with this email already exists");

        // Get member role
        var roles = await roleRepo.GetAllAsync();
        var memberRole = roles.FirstOrDefault(r => r.Name == "Member");
        if (memberRole == null)
            throw new BusinessException("Member role not found");

        // Create user
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            RoleId = memberRole.Id,
            IsActive = true
        };

        await userRepo.AddAsync(user);
        user.Role = memberRole;

        // Create refresh token
        var refreshToken = new RefreshToken
        {
            Token = _jwtService.GenerateRefreshToken(),
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };

        var refreshTokenRepo = _unitOfWork.Repository<RefreshToken>();
        await refreshTokenRepo.AddAsync(refreshToken);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = memberRole.Name,
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = refreshToken.Token
        };
    }
}

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public LoginUserCommandHandler(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var userRepo = _unitOfWork.Repository<User>();
        var users = await userRepo.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Email == request.Email);

        if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password");

        if (!user.IsActive)
            throw new UnauthorizedException("User account is inactive");

        // Load role
        var roleRepo = _unitOfWork.Repository<Role>();
        var roles = await roleRepo.GetAllAsync();
        user.Role = roles.FirstOrDefault(r => r.Id == user.RoleId);

        // Create or update refresh token
        var refreshTokenRepo = _unitOfWork.Repository<RefreshToken>();
        var refreshTokens = await refreshTokenRepo.GetAllAsync();
        var existingRefreshToken = refreshTokens.FirstOrDefault(rt => rt.UserId == user.Id && !rt.IsRevoked);

        string refreshToken;
        if (existingRefreshToken != null && existingRefreshToken.ExpiryDate > DateTime.UtcNow)
        {
            refreshToken = existingRefreshToken.Token;
        }
        else
        {
            var newRefreshToken = new RefreshToken
            {
                Token = _jwtService.GenerateRefreshToken(),
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };
            await refreshTokenRepo.AddAsync(newRefreshToken);
            refreshToken = newRefreshToken.Token;
        }

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role?.Name ?? "Member",
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = refreshToken
        };
    }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userId = Guid.Parse(principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
            ?? throw new UnauthorizedException("Invalid token"));

        var refreshTokenRepo = _unitOfWork.Repository<RefreshToken>();
        var refreshTokens = await refreshTokenRepo.GetAllAsync();
        var storedRefreshToken = refreshTokens.FirstOrDefault(rt => rt.UserId == userId && rt.Token == request.RefreshToken);

        if (storedRefreshToken == null || storedRefreshToken.IsRevoked || storedRefreshToken.ExpiryDate < DateTime.UtcNow)
            throw new UnauthorizedException("Invalid refresh token");

        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(userId) ?? throw new NotFoundException("User not found");

        var roleRepo = _unitOfWork.Repository<Role>();
        var roles = await roleRepo.GetAllAsync();
        user.Role = roles.FirstOrDefault(r => r.Id == user.RoleId);

        var newRefreshToken = _jwtService.GenerateRefreshToken();
        storedRefreshToken.IsRevoked = true;
        await refreshTokenRepo.UpdateAsync(storedRefreshToken);

        var newToken = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };
        await refreshTokenRepo.AddAsync(newToken);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role?.Name ?? "Member",
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = newRefreshToken
        };
    }
}

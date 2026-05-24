using MediatR;
using AI.TaskManagement.Application.DTOs.Auth;

namespace AI.TaskManagement.Application.Features.Auth.Commands;

public class RegisterUserCommand : IRequest<AuthResponse>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public RegisterUserCommand(string firstName, string lastName, string email, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
    }
}

public class LoginUserCommand : IRequest<AuthResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }

    public LoginUserCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
}

public class RefreshTokenCommand : IRequest<AuthResponse>
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public RefreshTokenCommand(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}

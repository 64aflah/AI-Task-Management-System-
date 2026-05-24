import { HubConnectionBuilder, HubConnectionState } from '@signalr/signalr';

let connection: any = null;

export const initializeNotificationHub = (token: string) => {
  if (connection && connection.state === HubConnectionState.Connected) {
    return connection;
  }

  connection = new HubConnectionBuilder()
    .withUrl('https://localhost:5001/hubs/notifications', {
      accessTokenFactory: () => token,
      skipNegotiation: true,
      transport: 1, // WebSockets
    })
    .withAutomaticReconnect()
    .build();

  connection.on('ReceiveNotification', (message: string) => {
    console.log('Notification received:', message);
    // Dispatch notification to Redux or trigger UI update
  });

  connection.start().catch((err: any) => console.error('SignalR connection error:', err));

  return connection;
};

export const closeNotificationHub = () => {
  if (connection) {
    connection.stop();
  }
};

export const getNotificationHub = () => connection;

let sessionUsers = [];
let sessionKey = '';
let participantId = 0;

const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl('/sessionHub', { transport: signalR.HttpTransportType.WebSockets })
    .withAutomaticReconnect()
    .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
    .build();

hubConnection.serverTimeoutInMilliseconds = 60000;
hubConnection.keepAliveIntervalInMilliseconds = 30000;
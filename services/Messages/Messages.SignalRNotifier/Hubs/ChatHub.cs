using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Messages.SignalRNotifier.Hubs;

[Authorize]
public sealed class ChatHub : Hub<IChatHubReceiver>, IChatHub;
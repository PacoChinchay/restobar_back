using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace restobar_core.Application.Hubs;

[Authorize]
public class StockHub : Hub { }

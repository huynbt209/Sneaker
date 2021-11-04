using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Sneaker.Hubs
{
    public class GroupOrderHub : Hub
    {
        public async Task AddItemGroup(string user, int itemId)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, itemId);
        }
    }
}
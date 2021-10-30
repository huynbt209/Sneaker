using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sneaker.Models;
using Microsoft.EntityFrameworkCore;
using Sneaker.Data;

namespace Sneaker.SocketServices
{
    public class ItemWebsocketHandler : WebSocketHandler
    {
        private readonly IServiceScopeFactory _service;
        public ItemWebsocketHandler(WebSocketConnectionManager webSocketConnectionManager, IServiceScopeFactory service) : base(webSocketConnectionManager)
        {

            _service = service;

        }
        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            Item data = JsonConvert.DeserializeObject<Item>(Encoding.UTF8.GetString(buffer, 0, result.Count));
            

            var responseObject = await GetResponseObjectAsync(data);

            await SendMessageToAllAsync(
                responseObject
            );
        }
        private async Task<JObject> GetResponseObjectAsync(Item payload)
        {
            using (var scope = _service.CreateScope())
            {

                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                await context.Items.AddAsync(payload);
                var list = await context.Items.ToListAsync();


                return JObject.FromObject(list);
            }
           

        }

    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace MyFirstGame.Hubs
{
    public class Hub : Microsoft.AspNetCore.SignalR.Hub
    {
        public void SendNumber(int Number)
        {
            Clients.Others.SendAsync("UpdateNumber", Number);
        }
    }

    public class GameHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public static Data.Arena Arena = new Data.Arena(10, 15);

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Rotine();
        }

        public bool OnLogin(string name)
        {
            if (!Arena.CanEnterNewUser(Context.ConnectionId))
            {
                //notifica todos
                Clients.All.SendAsync("UpdateArena", JsonConvert.SerializeObject(Arena));
                return false;
            }
            else
            {
                var newPosition = Arena.NewPosition();
                Arena.Slots[newPosition[0], newPosition[1]].ID = Context.ConnectionId;
                Arena.Users.Add(new Data.Users(name, Context.ConnectionId));

                Rotine();

                return true;
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Arena.Quit(Context.ConnectionId);

            await Rotine();

            await base.OnDisconnectedAsync(exception);
        }

        public async void CommandAction(Data.Arena.Movement command)
        {
            //movimenta o peao
            Arena.Move(Context.ConnectionId, command);

            await Rotine();
        }

        private async Task Rotine()
        {
            if (!Arena.IsThereFruit())
            {
                var newPosition = Arena.NewPosition();
                Arena.Slots[newPosition[0], newPosition[1]].IsThereFruit = true;
            }

            var convertedObject = JsonConvert.SerializeObject(Arena);
            await Clients.All.SendAsync("UpdateArena", convertedObject);
        }
    }
}

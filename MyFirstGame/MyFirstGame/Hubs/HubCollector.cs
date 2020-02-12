using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Drawing;

namespace MyFirstGame.Hubs
{
    public class Hub : Microsoft.AspNetCore.SignalR.Hub
    {
        public void SendNumber(int Number)
        {
            Clients.Others.SendAsync("UpdateNumber", Number);
        }
    }

    public class CollectorHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public static Data.Collector.Arena Arena = new Data.Collector.Arena(10, 15);

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Rotine();
        }

        public bool OnLogin(string name, int ArgbColor)
        {
            if (!Arena.CanEnterNewUser(Context.ConnectionId))
            {
                var user = Arena.GetUserByID(Context.ConnectionId);

                //update color ----> do before = check if can this color
                user.Color = Color.FromArgb(ArgbColor);
                user.Name = name;

                //notifica todos
                Clients.All.SendAsync("UpdateArena", JsonConvert.SerializeObject(Arena));
                return false;
            }
            else
            {
                var newPosition = Arena.NewPosition();
                Arena.Slots[newPosition[0], newPosition[1]].ID = Context.ConnectionId;
                Arena.Users.Add(new Data.Collector.Collector(name, Context.ConnectionId, Color.FromArgb(ArgbColor)));

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

        public async void CommandAction(Data.Movement command)
        {
            var user = Arena.GetUserByID(Context.ConnectionId);
            if (user != null)
            {
                //movimenta o peao
                Arena.Move(Context.ConnectionId, command);

                await Rotine();
            }
        }

        private async Task Rotine()
        {
            if (!Arena.IsThereFruit())
            {
                var newPosition = Arena.NewPosition();
                Arena.Slots[newPosition[0], newPosition[1]].IsFruit= true;
            }

            var convertedObject = JsonConvert.SerializeObject(Arena);
            await Clients.All.SendAsync("UpdateArena", convertedObject);
        }
    }
}

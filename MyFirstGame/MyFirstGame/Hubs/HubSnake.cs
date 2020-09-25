using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using MyFirstGame.Data.Snake;
using System.Diagnostics;

namespace MyFirstGame.Hubs
{
    public class SnakeHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private static Data.Snake.Arena Arena = new Data.Snake.Arena(10, 15);

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
                Arena.Slots[newPosition[0], newPosition[1]].Part = Data.Slot.SnakeParties.Head;
                Arena.Slots[newPosition[0], newPosition[1]].Sequence = 0;
                Arena.Users.Add(new Snake(name, Context.ConnectionId, Color.FromArgb(ArgbColor), Arena.Lines, Arena.Columns, Arena.Slots[newPosition[0],newPosition[1]]));
            }

            //ATUALIZA TODOS    
            Clients.All.SendAsync("UpdateArena", JsonConvert.SerializeObject(Arena));

            //retorno
            return true;
        }

        public static async Task CoreTime(IHubContext<SnakeHub> hub)
        {
            //atualizar novas posições
            Arena.MoveSnakes();

            //imprime no debug
            Print();

            //verifica ganhadores e frutas
            Arena.Rotine();

            //ATUALIZA TODOS    
            await hub.Clients.All.SendAsync("UpdateArena", JsonConvert.SerializeObject(Arena));

            //aumentar a velocidade gradativamente
            var speed = (10 * Arena.NFruits);

            //espera
            await Task.Delay(450 - (speed > 450 ? 450 : speed));
        }

        public void CommandAction(Data.Movement command)
        {
            var user = Arena.GetUserByID(Context.ConnectionId);
            if (user != null && command != Data.Movement.None)
            {
                //movimenta o peao
                user.LastForce = command; //me parece nao funcionar
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Arena.Quit(Context.ConnectionId);

            Arena.NFruits = 0;
            //await Rotine();

            await base.OnDisconnectedAsync(exception);
        }

        public static void Print()
        {
            for (int l = 0; l < Arena.Slots.GetLength(0); l++)
            {
                for (int c = 0; c < Arena.Slots.GetLength(1); c++)
                {
                    Debug.Write(((Arena.Slots[l, c].IsFruit) ? "F" : " ") + ((Arena.Slots[l, c].ID != null) ? "P" :" ") + "-");
                }
                Debug.WriteLine("");
            }
        }
    }
}

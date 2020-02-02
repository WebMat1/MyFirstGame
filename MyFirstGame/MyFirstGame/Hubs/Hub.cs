using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MyFirstGame.Hubs
{
    public class Hub : Microsoft.AspNetCore.SignalR.Hub
    {
        public void SendNumber(int Number)
        {
            Clients.Others.SendAsync("UpdateNumber", Number);
        }
    }
}

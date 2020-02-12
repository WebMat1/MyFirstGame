using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyFirstGame.Helper
{
    public class TimerHelper : BackgroundService
    {
        private readonly IHubContext<Hubs.SnakeHub> _hubContext;

        public TimerHelper(IHubContext<Hubs.SnakeHub> _hub)
        {
            _hubContext = _hub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Hubs.SnakeHub.CoreTime(_hubContext);
            }
        }
    }
}

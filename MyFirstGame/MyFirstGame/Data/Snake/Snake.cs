using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstGame.Data.Snake
{
    public class Snake : User, IDisposable
    {
        [JsonIgnore]
        public Movement LastForce { get; set; }
        [JsonIgnore]
        public Arena VirtualArena { get; set; }
        public Snake(string _Name, string _Id, Color _Color, int Lines, int Columns, Slot start) :base(_Name,_Id,_Color)
        {
            LastForce = Movement.None;
            VirtualArena = new Arena(Lines, Columns);
            VirtualArena.Slots[start.Row, start.Column] = (Slot)start.Clone();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

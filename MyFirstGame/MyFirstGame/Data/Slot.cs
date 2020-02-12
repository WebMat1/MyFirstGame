using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstGame.Data
{
    public class Slot : ICloneable
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public string ID { get; set; }
        public bool IsFruit { get; set; } = false;
        [JsonIgnore]
        public int Sequence { get; set; }
        public bool Colision { get; set; } = false;

        public Slot(int row, int column)
        {
            Row = row;
            Column = column;
        }
        
        public SnakeParties Part { get; set; }

        public enum SnakeParties
        {
            //All,
            Head,
            //Body,
            Anus
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

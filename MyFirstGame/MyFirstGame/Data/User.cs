using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstGame.Data
{
    public class User
    {
        [JsonProperty]
        public string Id { get; private set; }
        public int Score { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }

        public User(string _Name, string _Id, Color _color)
        {
            Id = _Id;
            Name = _Name;
            Color = _color;
        }
    }
}

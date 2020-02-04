using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstGame.Data
{
    public class Users
    {
        [JsonIgnore]
        public string Id { get; private set; }
        public int Score { get; set; }
        public string Name { get; set; }

        public Users(string _Name, string _Id)
        {
            Id = _Id;
            Name = _Name;
        }
    }
}

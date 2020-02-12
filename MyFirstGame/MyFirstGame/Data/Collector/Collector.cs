using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstGame.Data.Collector
{
    public class Collector : Data.User
    {
        public Collector(string _Name, string _Id, Color _color) : base(_Name, _Id, _color) { }
    }
}

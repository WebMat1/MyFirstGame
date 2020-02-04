using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstGame.Data
{
    public class Slot
    {
        public int Row { get; private set; }
        public int Column { get; private set; }

        public bool IsThereSomeone { get => (ID != null) ? true : false; }
        public bool IsThereFruit { get; set; } = false;
        public bool IsMe(string id)
        {
            return (id == ID);
        } 

        public string ID { get; set; }

        public string Color(string id)
        {
            if (IsMe(id))
                return "bg-primary";

            if (IsThereSomeone)
                return "bg-danger";

            if (IsThereFruit)
                return "bg-success";

            return "bg-light";

        }

        public Slot(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}

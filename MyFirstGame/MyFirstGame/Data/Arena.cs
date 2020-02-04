using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstGame.Data
{
    public class Arena
    {
        public Slot[,] Slots { get; set; }
        public List<Users> Users { get; set; } = new List<Users>();
        public enum Movement
        {
            Up,
            Down,
            Left,
            Right,
            None
        }
        public Arena(int lines, int columns)
        {
            Slots = new Slot[lines, columns];

            for (int l = 0; l < lines; l++)
            {
                for (int c = 0; c < columns; c++)
                {
                    Slots[l, c] = new Data.Slot(l,c);
                }
            }
        }

        public void Move(string id, Movement mov)
        {
            Slot Origin = GetSlotByID(id);
            Slot Target = null;

            switch (mov)
            {
                case Movement.Up:
                    // se bater na parede de cima
                    if (Origin.Row == 0)
                        Target = Slots[Slots.GetLength(0) - 1, Origin.Column];
                    else
                        Target = Slots[Origin.Row - 1, Origin.Column]; //movimenta
                    break;
                case Movement.Down:
                    //se bater na parede de baixo
                    if (Origin.Row == Slots.GetLength(0) - 1)
                        Target = Slots[0, Origin.Column];
                    else
                        Target = Slots[Origin.Row +1, Origin.Column];
                    break;
                case Movement.Left:
                    //se bater na parede da esquerda
                    if (Origin.Column == 0)
                        Target = Slots[Origin.Row, Slots.GetLength(1) -1];
                    else
                        Target = Slots[Origin.Row, Origin.Column - 1]; //movimenta
                    break;
                case Movement.Right:
                    //se bater na parede da direita
                    if (Origin.Column == Slots.GetLength(1) - 1)
                        Target = Slots[Origin.Row, 0];
                    else
                        Target = Slots[Origin.Row, Origin.Column+1];
                    break;
            }

            //faz a substituição do movimento
            if (Target != null && Target.ID == null)
            {
                Target.ID = Origin.ID;
                Origin.ID = null;

                if (Target.IsThereFruit == true)
                {
                    Target.IsThereFruit = false;
                    GetUserByID(id).Score++;
                }
                    
            }
        }

        public bool CanEnterNewUser(string id)
        {
            if (Users == null || Users.Count >= 10) //maximo de usuarios
                return false;

            if (GetUserByID(id) != null)
                return false;

            return true;
        }
        public int[] NewPosition()
        {
            int[] pos = new int[2];
            Random r = new Random();
            do
            {
                pos[0] = r.Next(0, Slots.GetLength(0));
                pos[1] = r.Next(0, Slots.GetLength(1));
            } while (Slots[pos[0],pos[1]].ID != null || Slots[pos[0], pos[1]].IsThereFruit == true);

            return pos;
        }

        private Slot GetSlotByID(string id)
        {
            Slot aux = null;
            for(int l = 0; l < Slots.GetLength(0); l++)
            {
                for (int c = 0; c< Slots.GetLength(1); c++)
                {
                    if (Slots[l, c].ID == id)
                        aux = Slots[l, c];
                }
            }
            return aux;
        }

        private Users GetUserByID(string id)
        {
            return Users.FirstOrDefault(q => q.Id == id);
        }
        public bool IsThereFruit()
        {
            for (int l = 0; l < Slots.GetLength(0); l++)
            {
                for (int c = 0; c < Slots.GetLength(1); c++)
                {
                    if (Slots[l, c].IsThereFruit)
                        return true;
                }
            }

            return false;
        }

        public void Quit(string id)
        {
            GetSlotByID(id).ID = null;

        }
    }
}

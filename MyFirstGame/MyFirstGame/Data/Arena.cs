using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstGame.Data
{
    public class Arena<T> where T : User
    {
        public Arena(int lines, int columns)
        {
            Slots = new Slot[lines, columns];

            for (int l = 0; l < lines; l++)
            {
                for (int c = 0; c < columns; c++)
                {
                    Slots[l, c] = new Slot(l, c);
                }
            }
        }

        public Slot[,] Slots { get; set; }
        public virtual List<T> Users { get; set; } = new List<T>();

        public bool CanEnterNewUser(string id)
        {
            if (Users == null || Users.Count >= 10) //maximo de usuarios
                return false;

            if (GetUserByID(id) != null)
                return false;

            return true;
        }
        public Slot GetSlotByID(string id)
        {
            Slot aux = null;
            for (int l = 0; l < Slots.GetLength(0); l++)
            {
                for (int c = 0; c < Slots.GetLength(1); c++)
                {
                    if (Slots[l, c].ID == id)
                        aux = Slots[l, c];
                }
            }
            return aux;
        }
        public IList<Slot> GetSlotsByID(string id)
        {
            List<Slot> ImageSnake = new List<Slot>();
            for (int l = 0; l < Slots.GetLength(0); l++)
            {
                for (int c = 0; c < Slots.GetLength(1); c++)
                {
                    if (Slots[l, c].ID == id)
                        ImageSnake.Add(Slots[l, c]);
                }
            }
            return ImageSnake;
        }

        public T GetUserByID(string id)
        {
            return Users.FirstOrDefault(q => q.Id == id);
        }
        
        public bool IsThereFruit()
        {
            for (int l = 0; l < Slots.GetLength(0); l++)
            {
                for (int c = 0; c < Slots.GetLength(1); c++)
                {
                    if (Slots[l, c].IsFruit)
                        return true;
                }
            }

            return false;
        }

        public void Quit(string id)
        {
            //when you died
            Die(id);

            //GetSlotByID(id).ID = null;
            Users.Remove(GetUserByID(id));
        }

        public void Die(string id)
        {
            foreach (Slot slot in GetSlotsByID(id))
            {
                slot.ID = null;
                slot.Part = Slot.SnakeParties.Head;
                slot.Sequence = 0;
            }
        }

        public int[] NewPosition()
        {
            int[] pos = new int[2];
            Random r = new Random();
            do
            {
                pos[0] = r.Next(0, Slots.GetLength(0));
                pos[1] = r.Next(0, Slots.GetLength(1));
            } while (Slots[pos[0], pos[1]].ID != null || Slots[pos[0], pos[1]].IsFruit == true);

            return pos;
        }

        public Color SlotColor(Slot slot)
        {
            //se for fruta
            if (slot.IsFruit)
                return Color.Green;

            //se for vazio
            if (slot.ID == null)
                return Color.LightGray; 

            //recebe o usuario
            var user = GetUserByID(slot.ID);

            //verifica se o usuario esta posicionado
            if (user == null)
                return Color.LightGray;

            //retorna se é corpo ou cabeça da snake
            if (Slots[slot.Row, slot.Column].Part != Slot.SnakeParties.Head)
                return Color.FromArgb(50, user.Color);//cor mais fraca
            else
                return user.Color; //cor mais forte

        }

        public Slot MoveTarget(Slot Origin, Movement mov)
        {
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
                        Target = Slots[Origin.Row + 1, Origin.Column];
                    break;
                case Movement.Left:
                    //se bater na parede da esquerda
                    if (Origin.Column == 0)
                        Target = Slots[Origin.Row, Slots.GetLength(1) - 1];
                    else
                        Target = Slots[Origin.Row, Origin.Column - 1]; //movimenta
                    break;
                case Movement.Right:
                    //se bater na parede da direita
                    if (Origin.Column == Slots.GetLength(1) - 1)
                        Target = Slots[Origin.Row, 0];
                    else
                        Target = Slots[Origin.Row, Origin.Column + 1];
                    break;
            }

            return Target;
        }
    }

    public enum Movement
    {
        Up,
        Down,
        Left,
        Right,
        None
    }
}

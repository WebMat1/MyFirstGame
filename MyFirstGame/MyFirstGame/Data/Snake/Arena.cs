using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstGame.Data.Snake
{
    public class Arena : Data.Arena<Snake>
    {
        [JsonIgnore]
        public int Lines { get; set; }
        [JsonIgnore]
        public int Columns { get; set; }
        [JsonIgnore]
        public int NFruits { get; set; } = 0;

        [JsonIgnore]
        public IList<string> EraseNext { get; set; } = new List<string>();
        [JsonIgnore]
        public IList<Snake> NextInsert { get; set; } = new List<Snake>();

        public Arena(int l, int c) : base(l,c) { Lines = l; Columns = c; }

        public void MoveSnakes()
        {
            EraseEliminateds();

            //para cada cobra
            foreach(Snake snake in Users)
            {
                //move a cobra na sua propria arena virtual
                MoveOneSnake(snake.VirtualArena, snake);
            }

            //se pegaram alguma fruta
            bool catchfruit = false;

            // compara todas as arenas pra ver se há colisão
            foreach (Snake snk in Users)
            {
                //variavel para falar se teve ou nao colisão
                bool colision = false;
                
                //só pode bater a cabeça
                var head = snk.VirtualArena.GetSlotsByID(snk.Id).FirstOrDefault(q => q.Sequence == 0);
                if (head != null)
                {
                    //se pegou a fruta
                    if (Slots[head.Row, head.Column].IsFruit)
                    {
                        catchfruit = true;
                        snk.Score++;
                    }

                    //verifica nas outras cobras
                    foreach (Snake other in Users)
                    {
                        if (other.VirtualArena.Slots[head.Row, head.Column].ID != null && (other.VirtualArena.Slots[head.Row, head.Column].ID == snk.Id && other.VirtualArena.Slots[head.Row, head.Column].Part != Slot.SnakeParties.Head || other.VirtualArena.Slots[head.Row, head.Column].ID != snk.Id))
                        {
                            colision = true;
                            EraseNext.Add(snk.Id);
                        }
                    }

                    //se houve colisão... notificar slot
                    if (colision == true)
                    {
                        Slots[head.Row, head.Column].Colision = true;
                    }

                    #region apaga
                    //slots de cobra
                    var representation = GetSlotsByID(snk.Id);

                    //se pegou fruta apaga todos - ultimo
                    if (!catchfruit)
                    {
                        foreach (Slot realSlot in GetSlotsByID(snk.Id))
                        {
                            Slots[realSlot.Row, realSlot.Column].ID = null;
                            Slots[realSlot.Row, realSlot.Column].Part = Slot.SnakeParties.Head;
                            Slots[realSlot.Row, realSlot.Column].Sequence = 0;
                        }
                    }
                    //se nao pegou apaga todos
                    else
                    {
                        foreach (Slot realSlot in representation.OrderBy(q=>q.Sequence).Take(representation.Count -1))
                        {
                            Slots[realSlot.Row, realSlot.Column].ID = null;
                            Slots[realSlot.Row, realSlot.Column].Part = Slot.SnakeParties.Head;
                            Slots[realSlot.Row, realSlot.Column].Sequence = 0;
                        }

                        var last = representation.OrderBy(q => q.Sequence).Last();
                        last.Part = Slot.SnakeParties.Anus;
                        last.Sequence = representation.Count;

                        snk.VirtualArena.Slots[last.Row, last.Column] = (Slot)last.Clone();

                    }

                    //escreve na arena principal
                    foreach (Slot sVirtual in snk.VirtualArena.GetSlotsByID(snk.Id))
                    {
                        if (!(colision && sVirtual.Sequence == 0))
                            Slots[sVirtual.Row, sVirtual.Column] = (Slot)sVirtual.Clone();
                    }

                    #endregion
                }
            }

            InsertEliminateds();

            //verifica frutas
            Rotine();
        }

        public void Rotine()
        {
            if (!IsThereFruit())
            {
                var newPosition = NewPosition();
                Slots[newPosition[0], newPosition[1]].IsFruit = true;
                NFruits++;
            }

            if (Users.Where(q => q.Score >= 20).Count() > 0)
            {
                foreach (Snake snake in Users)
                    snake.Score = 0;

                NFruits = 0;
            }
        }

        private void EraseEliminateds()
        {
            foreach (string id in EraseNext)
            {
                //retira do visual
                Die(id);

                //retira da memoria
                var user = GetUserByID(id);
                user.VirtualArena = new Arena(Lines,Columns);
                user.Score = 0;
                NextInsert.Add(user);
            }

            EraseNext = new List<string>();
        }

        private void InsertEliminateds()
        {
            foreach (Snake snk in NextInsert)
            {
                var newPosition = NewPosition();
                Slots[newPosition[0], newPosition[1]].ID = snk.Id;
                Slots[newPosition[0], newPosition[1]].Part = Data.Slot.SnakeParties.Head;
                Slots[newPosition[0], newPosition[1]].Sequence = 0;
                snk.VirtualArena.Slots[newPosition[0], newPosition[1]] = (Slot)Slots[newPosition[0], newPosition[1]].Clone();
            }

            NextInsert = new List<Snake>();
        }

        public static void MoveOneSnake(Arena arena, Snake snake)
        {
            var SlotsSnake = arena.GetSlotsByID(snake.Id).OrderBy(q => q.Sequence); // pega a cobra virtual
            var head = SlotsSnake.FirstOrDefault(); //cabeça
            var anus = SlotsSnake.LastOrDefault(); // anus

            if (head != null && anus != null)
            {
                var target = arena.MoveTarget(head, snake.LastForce);// pra onde ela está indo

                if (target != null)
                {

                    arena.Slots[target.Row, target.Column].ID = snake.Id;
                    arena.Slots[target.Row, target.Column].Sequence = 0;
                    arena.Slots[target.Row, target.Column].Part = Slot.SnakeParties.Head;

                    foreach (Slot slot in SlotsSnake)
                    {
                        arena.Slots[slot.Row, slot.Column].Part = Slot.SnakeParties.Anus;
                        arena.Slots[slot.Row, slot.Column].Sequence++;
                    }

                    //sumir onde tem anus
                    arena.Slots[anus.Row, anus.Column].ID = null;
                    arena.Slots[anus.Row, anus.Column].Sequence = 0;
                    arena.Slots[anus.Row, anus.Column].Part = Slot.SnakeParties.Head;
                }
            }
        }
    }
}

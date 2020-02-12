using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

namespace MyFirstGame.Data.Collector
{
    public class Arena : Data.Arena<Collector>
    {
        public Arena(int l, int c) : base(l, c) { }
        public void Move(string id, Movement mov)
        {
            Slot Origin = GetSlotByID(id);
            Slot Target = MoveTarget(Origin,mov);

            //faz a substituição do movimento
            if (Target != null && Target.ID == null)
            {
                Target.ID = Origin.ID;
                Origin.ID = null;

                if (Target.IsFruit == true)
                {
                    Target.IsFruit = false;
                    GetUserByID(id).Score++;
                    CheckWins(id);
                }
                    
            }
        }

        private void CheckWins(string id)
        {
            var currentUser = GetUserByID(id);
            if (currentUser.Score >= 15)
            {
                //zerar scores
                foreach(User user in Users)
                {
                    user.Score = 0;
                }
            }
        }
    }
}

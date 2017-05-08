using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    class Coup
    {
        public Coordonnee Position { get; set; }
        public int Score { get; set; }

        public Coup(Coordonnee position,int score)
        {
            Position = position;
            Score = score;
        }
    }
}

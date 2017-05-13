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
        public double Score { get; set; }

        public Coup(Coordonnee position,double score)
        {
            Position = position;
            Score = score;
        }
    }
}

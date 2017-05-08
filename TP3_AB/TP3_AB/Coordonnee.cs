using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    [Serializable]
    public class Coordonnee
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coordonnee(int x,int y)
        {
            X = x;
            Y = y;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    public enum Couleur { Blanc, Noir }
    public class Pion
    {
        private int CouleurPion { get; set; }
        public Pion(int couleurPion)
        {
            CouleurPion = couleurPion;
        }

        public bool EstBlanc()
        {
            return (CouleurPion == (int)Couleur.Blanc);
        }

        public bool EstNoir()
        {
            return (CouleurPion == (int)Couleur.Noir);
        }

        public void InverserCouleur()
        {
            if (CouleurPion == (int)Couleur.Blanc)
            {
                CouleurPion = (int)Couleur.Noir;
            }
            else
            {
                CouleurPion = (int)Couleur.Blanc;
            }
        }
    }
}

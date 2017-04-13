using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    public class Pion
    {
        private string couleur;
        private string Couleur
        {
            get { return couleur; }
            set
            {
                if (value == "Blanc" || value == "Noir")
                {
                    couleur = value;
                }
                else
                {
                    // Le "default" d'office.
                    couleur = "Blanc";
                }
            }
        }

        public Pion(string couleurPion)
        {
            Couleur = couleurPion;
        }

        public bool EstBlanc()
        {
            return (Couleur == "Blanc");
        }

        public bool EstNoir()
        {
            return (Couleur == "Noir");
        }

        public void InverserCouleur()
        {
            if (Couleur == "Blanc")
            {
                Couleur = "Noir";
            }
            else
            {
                Couleur = "Blanc";
            }
        }
    }
}

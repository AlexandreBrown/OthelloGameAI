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
        private Couleur Couleur { get; set; }
        public Pion(Couleur couleur)
        {
            Couleur = couleur;
        }

        public bool EstBlanc()
        {
            return (Couleur == Couleur.Blanc);
        }

        public bool EstNoir()
        {
            return (Couleur == Couleur.Noir);
        }

        public void InverserCouleur()
        {
            if (Couleur == Couleur.Blanc)
            {
                Couleur = Couleur.Noir;
            }
            else
            {
                Couleur = Couleur.Blanc;
            }
        }
    }
}

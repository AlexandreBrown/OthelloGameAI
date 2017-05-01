using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    public class CaseJeu
    {
        private Pion Contenu { get; set; }

        public CaseJeu()
        {

        }

        public bool EstOccupe()
        {
            return (Contenu != null);
        }

        public bool EstCaseBlanche()
        {
            return Contenu.EstBlanc();
        }

        public bool EstCaseNoire()
        {
            return Contenu.EstNoir();
        }

        public bool AjouterPion(Couleur couleur)
        {
            if (!EstOccupe())
            {
                Contenu = new Pion(couleur);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool InverserPion()
        {
            if (EstOccupe())
            {
                Contenu.InverserCouleur();

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

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

        private bool EstOccupe()
        {
            return (Contenu != null);
        }

        public bool? EstCaseBlanche()
        {
            if (EstOccupe())
            {
                return Contenu.EstBlanc();
            }
            else
            {
                return null;
            }
        }

        public bool? EstCaseNoire()
        {
            if (EstOccupe())
            {
                return Contenu.EstNoir();
            }
            else
            {
                return null;
            }
        }

        public bool AjouterPion(string couleur)
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

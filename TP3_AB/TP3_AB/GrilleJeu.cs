using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Othello
{
    public class GrilleJeu
    {
        #region Static

        /// <summary>
        /// La taille de la grille de jeu. Assume une grille de jeu carrée (X par X).
        /// </summary>
        public const int TAILLE_GRILLE_JEU = 8;

        #endregion

        private List<List<CaseJeu>> ListeCasesJeu { get; set; }

        public GrilleJeu()
        {
            InitialiserGrille();

            AjouterPionsDepart();
        }

        private void InitialiserGrille()
        {
            List<CaseJeu> listeTemp;
            ListeCasesJeu = new List<List<CaseJeu>>();

            for (int i = 0; i < TAILLE_GRILLE_JEU; i++)
            {
                listeTemp = new List<CaseJeu>();

                for (int j = 0; j < TAILLE_GRILLE_JEU; j++)
                {
                    listeTemp.Add(new CaseJeu());
                }

                ListeCasesJeu.Add(listeTemp);
            }
        }



        public List<Coordonnee> TrouverCasesAdjacentesLibres(Coordonnee position)
        {
            List<Coordonnee> casesAdjacentesLibres = new List<Coordonnee>();

            // En haut à gauche
            TesterPositionLibre(casesAdjacentesLibres, new Coordonnee(position.X - 1, position.Y - 1));
            // En haut
            TesterPositionLibre(casesAdjacentesLibres, new Coordonnee(position.X, position.Y - 1));
            // En haut à droite
            TesterPositionLibre(casesAdjacentesLibres, new Coordonnee(position.X + 1, position.Y - 1));
            // À droite
            TesterPositionLibre(casesAdjacentesLibres, new Coordonnee(position.X + 1, position.Y));
            // En bas à droite
            TesterPositionLibre(casesAdjacentesLibres, new Coordonnee(position.X + 1, position.Y + 1));
            // En bas
            TesterPositionLibre(casesAdjacentesLibres, new Coordonnee(position.X, position.Y + 1));
            // En bas à gauche
            TesterPositionLibre(casesAdjacentesLibres, new Coordonnee(position.X - 1, position.Y + 1));
            // À gauche
            TesterPositionLibre(casesAdjacentesLibres, new Coordonnee(position.X - 1, position.Y));

            return casesAdjacentesLibres;
        }

        private void TesterPositionLibre(List<Coordonnee> casesAdjacentesLibres, Coordonnee positionEnVerification)
        {
            if ((positionEnVerification.X > 0 && positionEnVerification.Y > 0) && (positionEnVerification.X < (GrilleJeu.TAILLE_GRILLE_JEU + 1) && positionEnVerification.Y < (GrilleJeu.TAILLE_GRILLE_JEU + 1)))
            {
                if (EstCaseLibre(positionEnVerification))
                {
                    casesAdjacentesLibres.Add(positionEnVerification);
                }
            }
        }


        private void AjouterPionsDepart()
        {
            ListeCasesJeu[3][3].AjouterPion(Couleur.Blanc);
            ListeCasesJeu[4][4].AjouterPion(Couleur.Blanc);

            ListeCasesJeu[3][4].AjouterPion(Couleur.Noir);
            ListeCasesJeu[4][3].AjouterPion(Couleur.Noir);
        }

        public bool AjouterPion(Coordonnee position, Couleur couleur)
        {
            return ListeCasesJeu[position.X - 1][position.Y - 1].AjouterPion(couleur);
        }

        public bool InverserPion(Coordonnee position)
        {
            return ListeCasesJeu[position.X - 1][position.Y - 1].InverserPion();
        }

        public bool EstCaseBlanche(Coordonnee position)
        {
            return ListeCasesJeu[position.X - 1][position.Y - 1].EstCaseBlanche();
        }

        public bool EstCaseNoire(Coordonnee position)
        {
            return ListeCasesJeu[position.X - 1][position.Y - 1].EstCaseNoire();
        }

        public bool EstCaseLibre(Coordonnee position)
        {
            return ListeCasesJeu[position.X - 1][position.Y - 1].EstOccupe() == false;
        }
    }
}

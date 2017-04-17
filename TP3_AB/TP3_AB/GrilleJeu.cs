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

        public bool? EstCaseBlanche(Coordonnee position)
        {
            return ListeCasesJeu[position.X - 1][position.Y - 1].EstCaseBlanche();
        }

        public bool? EstCaseNoire(Coordonnee position)
        {
            return ListeCasesJeu[position.X - 1][position.Y - 1].EstCaseNoire();
        }
    }
}

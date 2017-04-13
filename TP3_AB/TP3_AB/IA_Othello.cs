using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Othello
{
    public class IA_Othello : IObserver<JeuOthelloControl>
    {
        #region Code relié au patron observateur

        private IDisposable unsubscriber;

        public void Subscribe(IObservable<JeuOthelloControl> provider)
        {
            unsubscriber = provider.Subscribe(this);
        }

        public void Unsubscribe()
        {
            unsubscriber.Dispose();
        }

        public void OnCompleted()
        {
            // Ne fait rien pour l'instant.
        }

        public void OnError(Exception error)
        {
            // Ne fait rien pour l'instant.
        }

        public void OnNext(JeuOthelloControl g)
        {
            JouerCoup(g);
        }
        #endregion

        private JeuOthelloControl Jeu { get; set; }

        private string CouleurIA { get; set; }

        public IA_Othello(JeuOthelloControl jeu) : this(jeu, "Blanc") { }

        public IA_Othello(JeuOthelloControl jeu, string couleur)
        {
            Jeu = jeu;
            CouleurIA = couleur;

            // Abonner l'IA à l'interface du jeu.
            jeu.Subscribe(this);
        }

        private void JouerCoup(JeuOthelloControl jeu)
        {
            List<Point> ListeCoupsPermis;
            Random rnd = new Random(DateTime.Now.Millisecond);
            
            if (jeu.TourJeu == CouleurIA)
            {
                ListeCoupsPermis = TrouverCoupsPermis(jeu.Grille);

                jeu.ExecuterChoixCase(ListeCoupsPermis[rnd.Next(0,ListeCoupsPermis.Count)]);
            }
        }

        private List<Point> TrouverCoupsPermis(GrilleJeu grille)
        {
            List<Point> listeCoups = new List<Point>();
            Point position;

            for (int i = 1; i <= GrilleJeu.TAILLE_GRILLE_JEU; i++)
            {
                for (int j = 1; j <= GrilleJeu.TAILLE_GRILLE_JEU; j++)
                {
                    position = new Point(i, j);

                    if (position.X == 8 && position.Y == 8)
                    {
                        position.X = position.X;
                    }

                    if (grille.EstCaseBlanche(position) == null)
                    {
                        // Vérifier les voisins pour voir s'il y a un pion.
                        if (position.X > 1 && position.Y > 1 
                            && grille.EstCaseBlanche(new Point(position.X - 1, position.Y - 1)) != null)
                        {
                            listeCoups.Add(position);
                        }
                        else if (position.Y > 1
                                 && grille.EstCaseBlanche(new Point(position.X, position.Y - 1)) != null)
                        {
                            listeCoups.Add(position);
                        }
                        else if (position.X < GrilleJeu.TAILLE_GRILLE_JEU && position.Y > 1
                                 && grille.EstCaseBlanche(new Point(position.X + 1, position.Y - 1)) != null)
                        {
                            listeCoups.Add(position);
                        }
                        else if (position.X > 1
                                 && grille.EstCaseBlanche(new Point(position.X - 1, position.Y)) != null)
                        {
                            listeCoups.Add(position);
                        }
                        else if (position.X < GrilleJeu.TAILLE_GRILLE_JEU
                                 && grille.EstCaseBlanche(new Point(position.X + 1, position.Y)) != null)
                        {
                            listeCoups.Add(position);
                        }
                        else if (position.X > 1 && position.Y < GrilleJeu.TAILLE_GRILLE_JEU
                                 && grille.EstCaseBlanche(new Point(position.X - 1, position.Y + 1)) != null)
                        {
                            listeCoups.Add(position);
                        }
                        else if (position.Y < GrilleJeu.TAILLE_GRILLE_JEU
                                 && grille.EstCaseBlanche(new Point(position.X, position.Y + 1)) != null)
                        {
                            listeCoups.Add(position);
                        }
                        else if (position.X < GrilleJeu.TAILLE_GRILLE_JEU && position.Y < GrilleJeu.TAILLE_GRILLE_JEU
                                 && grille.EstCaseBlanche(new Point(position.X + 1, position.Y + 1)) != null)
                        {
                            listeCoups.Add(position);
                        }
                    }
                }
            }

            return listeCoups;
        }
    }
}

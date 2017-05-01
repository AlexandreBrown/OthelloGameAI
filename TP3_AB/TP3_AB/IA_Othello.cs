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

        private Couleur CouleurIA { get; set; }

        public IA_Othello(JeuOthelloControl jeu) : this(jeu, Couleur.Blanc) { }

        public IA_Othello(JeuOthelloControl jeu, Couleur couleur)
        {
            Jeu = jeu;
            CouleurIA = couleur;

            // Abonner l'IA à l'interface du jeu.
            jeu.Subscribe(this);
        }

        private void JouerCoup(JeuOthelloControl jeu)
        {
            List<Coordonnee> ListeCoupsPermis;
            Random rnd = new Random(DateTime.Now.Millisecond);
            
            if (jeu.TourJeu == CouleurIA)
            {
                ListeCoupsPermis = TrouverCoupsPermis(jeu.Grille);
                if(ListeCoupsPermis.Count > 0)
                {
                    jeu.ExecuterChoixCase(ListeCoupsPermis[rnd.Next(0,ListeCoupsPermis.Count)]);
                }
            }
        }

        private List<Coordonnee> TrouverCoupsPermis(GrilleJeu grille)
        {
            List<Coordonnee> coupsPermis = new List<Coordonnee>();

            // Trouver pion humain
            for (int i = 1; i <= GrilleJeu.TAILLE_GRILLE_JEU; i++)
            {
                for (int j = 1; j <= GrilleJeu.TAILLE_GRILLE_JEU; j++)
                {
                    Coordonnee position = new Coordonnee(i, j);
                    // Si la case n'est pas libre et qu'elle contient une piece de l'AI
                    if (grille.EstCaseLibre(position) == false && grille.EstCaseBlanche(position))
                    {
                        List<Coordonnee> caseAdjacentesLibres = new List<Coordonnee>();
                        // On trouve les cases adjacantes libres
                        caseAdjacentesLibres = grille.TrouverCasesAdjacentesLibres(position);
                        // On élimine ceux qui n'encadre pas de pièce , nous donnant ainsi que les coups valides
                        // TODO


                        // On ajoute les coordonnées des coups légaux à notre List de Coordonnee
                        foreach (Coordonnee c in caseAdjacentesLibres)
                        {
                            coupsPermis.Add(c);
                        }
                    }
                }
            }
            return coupsPermis;
        }

        /*
        private List<Coordonnee> TrouverCoupsPermis(GrilleJeu grille)
        {
            List<Coordonnee> listeCoups = new List<Coordonnee>();
            Coordonnee position;

            for (int i = 1; i <= GrilleJeu.TAILLE_GRILLE_JEU; i++)
            {
                for (int j = 1; j <= GrilleJeu.TAILLE_GRILLE_JEU; j++)
                {
                    position = new Coordonnee(i, j);

                    if (grille.EstCaseLibre(position))
                    {
                        // Vérifier les voisins pour voir s'il y a un pion.
                        if (position.X > 1 && position.Y > 1 
                            && grille.EstCaseLibre(new Coordonnee(position.X - 1, position.Y - 1)) == false)
                        {
                            listeCoups.Add(position);
                        }
                        else if (position.Y > 1
                                 && grille.EstCaseLibre(new Coordonnee(position.X, position.Y - 1)) == false)
                        {
                            listeCoups.Add(position);
                        }
                        else if (position.X < GrilleJeu.TAILLE_GRILLE_JEU && position.Y > 1
                                 && grille.EstCaseLibre(new Coordonnee(position.X + 1, position.Y - 1)) == false)
                        {
                            listeCoups.Add(position);
                        }
                        else if (position.X > 1
                                 && grille.EstCaseLibre(new Coordonnee(position.X - 1, position.Y)) == false)
                        {
                            listeCoups.Add(position);
                        }
                        else if (position.X < GrilleJeu.TAILLE_GRILLE_JEU
                                 && grille.EstCaseLibre(new Coordonnee(position.X + 1, position.Y)) == false)
                        {
                            listeCoups.Add(position);
                        }
                        else if (position.X > 1 && position.Y < GrilleJeu.TAILLE_GRILLE_JEU
                                 && grille.EstCaseLibre(new Coordonnee(position.X - 1, position.Y + 1)) == false)
                        {
                            listeCoups.Add(position);
                        }
                        else if (position.Y < GrilleJeu.TAILLE_GRILLE_JEU
                                 && grille.EstCaseLibre(new Coordonnee(position.X, position.Y + 1)) == false)
                        {
                            listeCoups.Add(position);
                        }
                        else if (position.X < GrilleJeu.TAILLE_GRILLE_JEU && position.Y < GrilleJeu.TAILLE_GRILLE_JEU
                                 && grille.EstCaseLibre(new Coordonnee(position.X + 1, position.Y + 1)) == false)
                        {
                            listeCoups.Add(position);
                        }
                    }
                }
            }

            return listeCoups;
        }
        */
    }
}

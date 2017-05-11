using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Othello
{
    public enum NiveauDifficulte { Facile, Normal, Difficile, Professionnel = 5 }
    [Serializable]
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
            JouerCoupAsync(g);
        }
        #endregion

        private JeuOthelloControl Jeu { get; set; }

        public List<Coordonnee> CoupsPermisAI { get; set; } = new List<Coordonnee>();

        private Couleur CouleurIA { get; set; }

        private NiveauDifficulte Difficulte { get; set; }

        public IA_Othello(JeuOthelloControl jeu,NiveauDifficulte difficulte) : this(jeu, Couleur.Blanc, difficulte) { }

        public IA_Othello(JeuOthelloControl jeu, Couleur couleur,NiveauDifficulte difficulte)
        {
            Jeu = jeu;
            CouleurIA = couleur;
            Difficulte = difficulte;

            // Abonner l'IA à l'interface du jeu.
            jeu.Subscribe(this);
        }

        private async void JouerCoupAsync(JeuOthelloControl jeu)
        {
            if (jeu.TourJeu == CouleurIA)
            {
                CoupsPermisAI = jeu.TrouverCoupsPermis(CouleurIA);
                if ((int)Difficulte < (int)NiveauDifficulte.Difficile)
                {
                    await Task.Delay(1000);
                }
                else
                {
                    await Task.Delay(500);
                }
                // On exécute le coup correspondant à la meilleur position relative au niveau de difficulté
                jeu.ExecuterChoixCase(TrouverMeilleurPosition(jeu, jeu.Grille, (int)Difficulte, CoupsPermisAI), CouleurIA);
            }
        }

        private Coordonnee TrouverMeilleurPosition(JeuOthelloControl jeu, GrilleJeu grille,int nombreDeCoupsEnAvance,List<Coordonnee> lstPositionsPossibles)
        {
            if(Difficulte > NiveauDifficulte.Facile)
            {
                if(PossibiliteDeCoin(lstPositionsPossibles) == false)
                {
                    List<Coup> lstCoups = new List<Coup>();
                    // Attribut un score selon le nombre de coups en avance voulu pour chaque position , ce qui forme un coup pour chaque positions valides
                    RemplirListeCoups(lstCoups, lstPositionsPossibles, nombreDeCoupsEnAvance, jeu,grille);
                    // On attribut le coup avec le plus gros score à la variable meilleurCoup
                    Coordonnee meilleurPosition = PositionMaxCoups(lstCoups);
                    // On retourne la position du meilleur coup
                    return meilleurPosition;
                }
                else
                {
                    return TrouverCoin(lstPositionsPossibles); // Les coins sont des positions très importantes qui devraient toujours être priorisées
                }
            }
            else
            {
                Random rnd = new Random(DateTime.Now.Millisecond);
                return lstPositionsPossibles[rnd.Next(0, lstPositionsPossibles.Count)];
            }
        }

        private bool PossibiliteDeCoin(List<Coordonnee> lstPositions)
        {
            foreach (Coordonnee position in lstPositions)
            {
                if (EstCoin(position))
                {
                    return true;
                }
            }
            return false;
        }

        private Coordonnee TrouverCoin(List<Coordonnee> lstPositions)
        {
            Coordonnee coin = new Coordonnee(lstPositions[0].X, lstPositions[0].Y);
            foreach (Coordonnee position in lstPositions)
            {
                if (EstCoin(position))
                {
                    coin.X = position.X;
                    coin.Y = position.Y;
                }
            }
            return coin;
        }

        private Coordonnee PositionMaxCoups(List<Coup> lstCoups)
        {
            Coup maxCoup = lstCoups[0];
            for (int i = 1; i < lstCoups.Count; i++)
            {
                if(lstCoups[i].Score > maxCoup.Score)
                {
                    maxCoup = lstCoups[i];
                }
            }
            return maxCoup.Position;
        }

        private Coordonnee PositionMinCoups(List<Coup> lstCoups)
        {
            Coup minCoup = lstCoups[0];
            for (int i = 1; i < lstCoups.Count; i++)
            {
                if (lstCoups[i].Score < minCoup.Score)
                {
                    minCoup = lstCoups[i];
                }
            }
            return minCoup.Position;
        }

        private int MaxScoreCoups(List<Coup> lstCoups)
        {
            Coup maxCoup = lstCoups[0];
            for (int i = 1; i < lstCoups.Count; i++)
            {
                if (lstCoups[i].Score > maxCoup.Score)
                {
                    maxCoup = lstCoups[i];
                }
            }
            return maxCoup.Score;
        }

        private int MinScoreCoups(List<Coup> lstCoups)
        {
            Coup minCoup = lstCoups[0];
            for (int i = 1; i < lstCoups.Count; i++)
            {
                if (lstCoups[i].Score < minCoup.Score)
                {
                    minCoup = lstCoups[i];
                }
            }
            return minCoup.Score;
        }


        private void RemplirListeCoups(List<Coup> lstCoups,List<Coordonnee> lstPositionsPossibles,int nombreDeCoupsEnAvance,JeuOthelloControl jeu,GrilleJeu grille)
        {
            // On remplit notre liste de coups
            foreach(Coordonnee position in lstPositionsPossibles)
            {
                Coup coup = new Coup(position, ScoreApresXCoupsSimules(position, nombreDeCoupsEnAvance, jeu, grille));
                lstCoups.Add(coup);
            }
        }

        private int ScoreApresXCoupsSimules(Coordonnee positionAIPossibleEnVerif,int NbCoupsASimuler,JeuOthelloControl jeuDepart,GrilleJeu grilleDepart)
        {
            int scoreDePosition = 0;
            JeuOthelloControl jeuEnSimulation = new JeuOthelloControl(grilleDepart);
            // On joue la position en vérification sur une copie du jeu actuel
            JouerCoup(jeuEnSimulation, positionAIPossibleEnVerif, Couleur.Blanc);
            // On calcul la valeur x de cette position en vérification , x coups plus tard
            for (int i = 0; i < NbCoupsASimuler; i++)
            {
                List<Coup> lstCoups = new List<Coup>();
                // Un coup sur deux sera le tour du "minimizer" , autrement dit le joueur voulant que notre score (score AI) soit le plus faible possible
                if (i % 2 == 0) // Minimizer (Humain)
                {
                    SimulerMinMax(jeuEnSimulation, Couleur.Noir, ref lstCoups, ref scoreDePosition);
                }
                else // Maximizer (AI)
                {
                    SimulerMinMax(jeuEnSimulation, CouleurIA, ref lstCoups, ref scoreDePosition);
                }
            }
            return scoreDePosition;
        }

        private bool EstCoin(Coordonnee position)
        {
            return (EstCoinHautGauche(position) || EstCoinHautDroite(position) || EstCoinBasDroite(position) || EstCoinBasGauche(position));
        }

        private bool EstCoinHautGauche(Coordonnee position)
        {
            return (position.X == 1 && position.Y == 1);
        }
        private bool EstCoinHautDroite(Coordonnee position)
        {
            return (position.X == GrilleJeu.TAILLE_GRILLE_JEU && position.Y == 1);
        }
        private bool EstCoinBasDroite(Coordonnee position)
        {
            return (position.X == GrilleJeu.TAILLE_GRILLE_JEU && position.Y == GrilleJeu.TAILLE_GRILLE_JEU);
        }
        private bool EstCoinBasGauche(Coordonnee position)
        {
            return (position.X == 1 && position.Y == GrilleJeu.TAILLE_GRILLE_JEU);
        }

        private void SimulerMinMax(JeuOthelloControl jeuEnSimulation,Couleur couleurEnSimulation, ref List<Coup> lstCoups,ref int scoreDePosition)
        {
            // On doit évaluer chaque coups possible de ce joueur
            foreach (Coordonnee position in jeuEnSimulation.TrouverCoupsPermis(couleurEnSimulation))
            {
                int score = 0;
                if (EstCoin(position) == false ) // On ne doit pas simulé les coins , car c'est un choix clair pour le joueur même si le nombre de pions retournés n'est pas le plus haut
                {
                    // On récupère le score du AI suite au coup actuel
                    score = ScoreAIApresCoupSimule(jeuEnSimulation, position, couleurEnSimulation);
                }else
                {
                    if(couleurEnSimulation == CouleurIA)
                    {
                        score = 64; // Si l'IA à la possibilité de mettre un pion dans un coin , on donne un score très élevé à cette position
                    }
                    else
                    {
                        score = -64; // Si l'humain à la possibilité de mettre un pion dans un coin on donne un score très mauvais pour le AI
                                     // De cette façon nous prenons en considérations que l'humain va choisir les coins même si ce choix ne retourne pas le plus de pièces , 
                                     // car le but de l'humain est de minimiser le score du AI et donc de maximiser le sien
                    }
                }
                // On stock le coup (la position que nous avons évaluée ainsi que sa valeur)
                Coup coup = new Coup(position, score);
                // On ajoute ce coup à notre liste
                lstCoups.Add(coup);  
            }
            if (lstCoups.Count > 0)
            {
                Coordonnee positionChoisie = new Coordonnee(0, 0);
                // Une fois que nous avons évalué tous les coups , on choisi le coup qui avantage ou désavantage le plus le AI (dépendemment c'est à qui de jouer le tour en simulation)
                if (couleurEnSimulation == CouleurIA) // Tour à l'AI
                {
                    // On choisi donc le coup avec le score le plus élevé pour l'AI ,car c'est le meilleur choix pour lui et c'est à son tour de jouer ,donc il est logique de dire que l'AI doit
                        // prendre le meilleur des coup lorsque c'est à son tour
                    positionChoisie = PositionMaxCoups(lstCoups);
                    // On met à jour le score de la position avec le score le plus réçent que nous avons
                        // Si la simulation devait s'arrêter maintenant ce serait à l'AI de jouer donc celui-ci prendrait le coup qui l'avantagerait le plus
                            // Donc il choisirait le coup avec le score du AI le plus élevé
                    scoreDePosition = MaxScoreCoups(lstCoups);
                }
                else // Tour à l'humain
                {
                    // On choisi donc le coup avec le score le plus faible pour l'AI ,car c'est le meilleur choix pour le joueur qui doit jouer actuellement (l'humain)
                    positionChoisie = PositionMinCoups(lstCoups);
                    // On met à jour le score de la position avec le score le plus réçent que nous avons
                        // Si la simulation devait s'arrêter maintenant ce serait à l'humain de jouer donc celui-ci prendrait le coup qui avantage le moins l'AI
                            // Donc il choisirait le coup avec le score du AI le plus faible
                    scoreDePosition = MinScoreCoups(lstCoups);
                }
                // Une fois la position choisie , on joue le coup du joueur actuel
                JouerCoup(jeuEnSimulation, positionChoisie, couleurEnSimulation);
            }
        }

        private void JouerCoup(JeuOthelloControl jeu,Coordonnee position,Couleur joueurEnSimulation)
        {
            jeu.Grille.AjouterPion(position, joueurEnSimulation);
            jeu.InverserPionsAdverse(position, joueurEnSimulation);
            jeu.AjouterCerclePion(position, joueurEnSimulation);
        }

        private int ScoreAIApresCoupSimule(JeuOthelloControl jeuEnSimulation, Coordonnee position,Couleur joueurEnSimulation)
        {
            JeuOthelloControl jeuSimulation = new JeuOthelloControl(jeuEnSimulation.Grille);
            jeuSimulation.InverserPionsAdverse(position, joueurEnSimulation);
            jeuSimulation.Grille.AjouterPion(position, joueurEnSimulation);
            return jeuSimulation.Grille.CalculerNbPionsBlancs();
        }

    }
}

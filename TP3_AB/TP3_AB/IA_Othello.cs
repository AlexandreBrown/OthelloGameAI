using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Othello
{
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

        public IA_Othello(JeuOthelloControl jeu) : this(jeu, Couleur.Blanc) { }

        public IA_Othello(JeuOthelloControl jeu, Couleur couleur)
        {
            Jeu = jeu;
            CouleurIA = couleur;

            // Abonner l'IA à l'interface du jeu.
            jeu.Subscribe(this);
        }

        private async void JouerCoupAsync(JeuOthelloControl jeu)
        {

            if (jeu.TourJeu == CouleurIA)
            {
                CoupsPermisAI = jeu.TrouverCoupsPermis(CouleurIA);
                if (CoupsPermisAI.Count > 0)
                {
                    await Task.Delay(200);
                    jeu.ExecuterChoixCase(TrouverMeilleurPosition(jeu, jeu.Grille, 3, CoupsPermisAI), CouleurIA);
                }
                else
                {
                    jeu.MettreAJourTourPasse(CouleurIA);
                }
            }
        }

        private Coordonnee TrouverMeilleurPosition(JeuOthelloControl jeu, GrilleJeu grille,int nombreDeCoupsEnAvance,List<Coordonnee> lstPositionsPossibles)
        {
            List<Coup> lstCoups = new List<Coup>();
            // Attribut un score selon le nombre de coups en avance voulu pour chaque position , ce qui forme un coup pour chaque positions valides
            RemplirListeCoups(lstCoups, lstPositionsPossibles, nombreDeCoupsEnAvance, jeu,grille);
            // On attribut le coup avec le plus gros score à la variable meilleurCoup
            Coordonnee meilleurPosition = PositionMaxCoups(lstCoups);
            // On retourne la position du meilleur coup
            return meilleurPosition;
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
                if (lstCoups[i].Score > minCoup.Score)
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
            JeuOthelloControl jeuEnSimulation = new JeuOthelloControl(jeuDepart, grilleDepart);
            // On joue la position en vérification sur une copie du jeu actuel
            JouerCoup(jeuEnSimulation, positionAIPossibleEnVerif, Couleur.Blanc);
            // On calcul la valeur x de cette position en vérification , x coups plus tard
            for (int i = 0; i < NbCoupsASimuler; i++)
            {
                List<Coup> lstCoups = new List<Coup>();
                // Un coup sur deux sera le tour du "minimizer" , autrement dit le joueur voulant que notre score (score AI) soit le plus faible possible
                if (i % 2 == 0) // Minimizer (Humain)
                {
                    SimulerMinimizer(jeuEnSimulation, Couleur.Noir, ref lstCoups, ref scoreDePosition);
                }
                else // Maximizer (AI)
                {
                    SimulerMaximizer(jeuEnSimulation, CouleurIA, ref lstCoups, ref scoreDePosition);
                }
            }
            return scoreDePosition;
        }

        private void SimulerMinimizer(JeuOthelloControl jeuEnSimulation,Couleur couleurMinimizer, ref List<Coup> lstCoups,ref int scoreDePosition)
        {
            // On doit évaluer chaque coups possible de ce joueur
            foreach (Coordonnee position in jeuEnSimulation.TrouverCoupsPermis(couleurMinimizer))
            {
                int score = 0;
                // On récupère le score du AI suite au coup actuel
                score = ScoreAIApresCoupSimule(jeuEnSimulation, position, couleurMinimizer);
                // On stock le coup (la position que nous avons évaluée ainsi que sa valeur)
                Coup coup = new Coup(position, score);
                // On ajoute ce coup à notre liste
                lstCoups.Add(coup);
            }
            if (lstCoups.Count > 0)
            {
                // Une fois que nous avons évalué tous les coups , on choisi le coup qui avantagera le plus le joueur actuel (l'humain)
                // On choisi donc le coup avec le score le plus faible pour l'AI (Nous prenons en considération que l'humain jouera le meilleur coup possible)
                Coordonnee positionChoisie = PositionMinCoups(lstCoups);
                // On met à jour le score de la position
                scoreDePosition = MinScoreCoups(lstCoups);
                // Une fois la position choisie , on joue le coup du joueur actuel
                JouerCoup(jeuEnSimulation, positionChoisie, couleurMinimizer);
            }
        }

        private void SimulerMaximizer(JeuOthelloControl jeuEnSimulation,Couleur couleurMaximizer,ref List<Coup> lstCoups,ref int scoreDePosition)
        {
            // On doit évaluer chaque coups possible de ce joueur
            foreach (Coordonnee position in jeuEnSimulation.TrouverCoupsPermis(couleurMaximizer))
            {
                int score = 0;
                // On récupère le score du AI suite au coup actuel
                score = ScoreAIApresCoupSimule(jeuEnSimulation, position, couleurMaximizer);
                // On stock le coup (la position que nous avons évaluée ainsi que sa valeur)
                Coup coup = new Coup(position, score);
                // On ajoute ce coup à notre liste
                lstCoups.Add(coup);
            }
            if (lstCoups.Count > 0)
            {
                // Une fois que nous avons évalué tous les coups , on choisi le coup qui avantagera le plus le joueur actuel (l'AI)
                // On choisi donc le coup avec le score le plus fort pour l'AI
                Coordonnee positionChoisie = PositionMaxCoups(lstCoups);
                // On met à jour le score de la position
                scoreDePosition = MaxScoreCoups(lstCoups);
                // Une fois la position choisie , on joue le coup du joueur actuel
                JouerCoup(jeuEnSimulation, positionChoisie, Couleur.Blanc);
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
            JeuOthelloControl jeuSimulation = new JeuOthelloControl(jeuEnSimulation, jeuEnSimulation.Grille);
            jeuSimulation.InverserPionsAdverse(position, joueurEnSimulation);
            jeuSimulation.Grille.AjouterPion(position, joueurEnSimulation);
            return jeuSimulation.Grille.CalculerNbPionsBlancs();
        }

    }
}

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
            Random rnd = new Random(DateTime.Now.Millisecond);

            if (jeu.TourJeu == CouleurIA)
            {
                CoupsPermisAI = jeu.TrouverCoupsPermis(CouleurIA);
                if (CoupsPermisAI.Count > 0)
                {
                    await Task.Delay(3000);
                    jeu.ExecuterChoixCase(CoupsPermisAI[rnd.Next(0, CoupsPermisAI.Count)], CouleurIA);
                }
            }
        }

    }
}

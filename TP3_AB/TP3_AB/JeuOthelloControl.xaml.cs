using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Othello
{
    /// <summary>
    /// Logique d'interaction pour JeuOthelloControl.xaml
    /// </summary>
    public partial class JeuOthelloControl : UserControl
    {
        #region Code relié au patron observateur

        List<IObserver<JeuOthelloControl>> observers;

        // Oui, une classe privée (et interne).
        private class Unsubscriber : IDisposable
        {
            private List<IObserver<JeuOthelloControl>> _observers;
            private IObserver<JeuOthelloControl> _observer;

            public Unsubscriber(List<IObserver<JeuOthelloControl>> observers, IObserver<JeuOthelloControl> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }

        public IDisposable Subscribe(IObserver<JeuOthelloControl> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }

        private void Notify()
        {
            foreach (IObserver<JeuOthelloControl> ob in observers)
            {
                ob.OnNext(this);
            }
        }
        #endregion

        private int TailleCase { get; set; }

        public GrilleJeu Grille { get; private set; }

        private List<List<Ellipse>> GrillePions { get; set; }

        public Couleur TourJeu { get; set; }

        private SolidColorBrush CouleurPionHumain { get; set; }
        private SolidColorBrush CouleurPionAI { get; set; }

        private IA_Othello IA { get; set; }

        public Action SupprimerVue;
        public Action NouvellePartie { get; set; }

        public JeuOthelloControl(int tailleCase, SolidColorBrush couleurPionHumain, SolidColorBrush couleurPionAI)
        {
            TailleCase = tailleCase;
            CouleurPionHumain = couleurPionHumain;
            CouleurPionAI = couleurPionAI;
            InitializeComponent();
            DefinirGrid();
            
            // Initialise la liste d'observateurs.
            observers = new List<IObserver<JeuOthelloControl>>();

            Grille = new GrilleJeu();
            InitialiserGrillePions();
            DessinerCases();
            InitialiserQuadrillageCases();
            RafraichirAffichage();
            TourJeu = Couleur.Noir;
            // Initialiser l'IA.
            IA = new IA_Othello(this);
            
        }

        private void DefinirGrid()
        {
            InitialiserContourJeu();
            InitialiserJeu();
            grdJeuScore.RowDefinitions[1].Height = new GridLength((GrilleJeu.TAILLE_GRILLE_JEU + 1) * TailleCase);
        }

        private void InitialiserContourJeu()
        {
            // Columns (A-H)
            char lettre = 'A';
            for (int i = 1; i <= GrilleJeu.TAILLE_GRILLE_JEU; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = GridLength.Auto;
                grdJeu.ColumnDefinitions.Add(column);
                Label l = new Label();
                l.Content = (lettre).ToString();
                l.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetColumn(l, i);
                Grid.SetRow(l, 0);
                grdJeu.Children.Add(l);
                lettre++;
            }
            // Rows (1-8)
            for (int j = 1; j <= GrilleJeu.TAILLE_GRILLE_JEU; j++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                grdJeu.RowDefinitions.Add(row);
                Label l = new Label();
                l.Content = (j).ToString();
                l.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(l, 0);
                Grid.SetRow(l, j);
                grdJeu.Children.Add(l);
            }
        }

        private void InitialiserJeu()
        {
            for (int i = 1; i <= GrilleJeu.TAILLE_GRILLE_JEU; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(TailleCase);
                grdJeu.ColumnDefinitions.Add(column);
                for (int j = 1; j <= GrilleJeu.TAILLE_GRILLE_JEU; j++)
                {
                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(TailleCase);
                    grdJeu.RowDefinitions.Add(row);
                }
            }
        }

        private void InitialiserQuadrillageCases()
        {
            // Row
            for (int i = 0; i < GrilleJeu.TAILLE_GRILLE_JEU; i++)
            {
                for (int j = 0; j < GrilleJeu.TAILLE_GRILLE_JEU; j++)
                {
                    Rectangle recRow = new Rectangle();
                    recRow.Fill = recJeuBG.Fill;
                    recRow.Width = 1;
                    recRow.HorizontalAlignment = HorizontalAlignment.Right;
                    Grid.SetRow(recRow, i);
                    Grid.SetRowSpan(recRow, GrilleJeu.TAILLE_GRILLE_JEU);
                    Grid.SetColumn(recRow, j);
                    grdJeu.Children.Add(recRow);
                }
            }
            // Columns
            for (int i = 0; i < GrilleJeu.TAILLE_GRILLE_JEU; i++)
            {
                for (int j = 0; j < GrilleJeu.TAILLE_GRILLE_JEU; j++)
                {
                    Rectangle recCol = new Rectangle();
                    recCol.Fill = recJeuBG.Fill;
                    recCol.Height = 1;
                    recCol.VerticalAlignment = VerticalAlignment.Bottom;
                    Grid.SetRow(recCol, i);
                    Grid.SetColumn(recCol, j);
                    Grid.SetColumnSpan(recCol, GrilleJeu.TAILLE_GRILLE_JEU);
                    grdJeu.Children.Add(recCol);
                }
            }
        }

        private void InitialiserGrillePions()
        {
            List<Ellipse> listeTemp;
            GrillePions = new List<List<Ellipse>>();

            for (int i = 0; i < GrilleJeu.TAILLE_GRILLE_JEU; i++)
            {
                listeTemp = new List<Ellipse>();

                for (int j = 0; j < GrilleJeu.TAILLE_GRILLE_JEU; j++)
                {
                    listeTemp.Add(new Ellipse());
                }

                GrillePions.Add(listeTemp);
            }
        }

        private void DessinerCases()
        {
            Rectangle carreArrierePlan, carreClick;

            for (int i = 1; i <= GrilleJeu.TAILLE_GRILLE_JEU; i++)
            {
                for (int j = 1; j <= GrilleJeu.TAILLE_GRILLE_JEU; j++)
                {
                    // Ajouter le rectangle d'arrière plan.
                    carreArrierePlan = CreerCarre(Brushes.Green);

                    Grid.SetColumn(carreArrierePlan, i);
                    Grid.SetRow(carreArrierePlan, j);

                    grdJeu.Children.Add(carreArrierePlan);

                    // Ajouter le rectangle qui sert à recevoir les click de souris.
                    carreClick = CreerCarre(Brushes.Transparent);

                    Grid.SetColumn(carreClick, i);
                    Grid.SetRow(carreClick, j);
                    // ZIndex à 5 pour qu'il soit "au-dessus" de l'interface.
                    Grid.SetZIndex(carreClick, 5);

                    carreClick.MouseLeftButtonUp += new MouseButtonEventHandler(GrilleJeu_Click);

                    grdJeu.Children.Add(carreClick);
                }
            }
        }

        private void RafraichirAffichage()
        {
            Coordonnee position;

            // Retirer de la Grid les ellipses utilisées pour afficher des pions.
            foreach (List<Ellipse> listePion in GrillePions)
            {
                foreach (Ellipse pion in listePion)
                {
                    grdJeu.Children.Remove(pion);
                }
            }

            InitialiserGrillePions();

            for (int i = 1; i <= GrilleJeu.TAILLE_GRILLE_JEU; i++)
            {
                for (int j = 1; j <= GrilleJeu.TAILLE_GRILLE_JEU; j++)
                {
                    position = new Coordonnee(i, j);

                    if (Grille.EstCaseBlanche(position) != null)
                    {
                        if ((bool)Grille.EstCaseBlanche(position))
                        {
                            AjouterCerclePion(position, Couleur.Blanc);
                        }
                        else
                        {
                            AjouterCerclePion(position, Couleur.Noir);
                        }
                    }
                }
            }
        }
        
        private Rectangle CreerCarre(SolidColorBrush couleur)
        {
            Rectangle r;

            r = new Rectangle();
            r.Height = TailleCase - 1;
            r.Width = TailleCase - 1 ;
            r.Fill = couleur;

            return r;
        }

        private Ellipse CreerCercle(SolidColorBrush couleur)
        {
            Ellipse el ;

            el = new Ellipse();
            el.Height = 0.8 * TailleCase;
            el.Width = 0.8 * TailleCase;
            el.Fill = couleur;

            return el;
        }

        private void AjouterCerclePion(Coordonnee position,Couleur couleur)
        {
            Ellipse cerclePion;

            if (couleur == Couleur.Blanc)
            {
                cerclePion = CreerCercle(CouleurPionAI);
            }
            else
            {
                cerclePion = CreerCercle(CouleurPionHumain);
            }

            Grid.SetColumn(cerclePion, position.X);
            Grid.SetRow(cerclePion, position.Y);

            grdJeu.Children.Add(cerclePion);

            // Ajouter le pion dans la liste de l'affichage.
            GrillePions[position.X - 1][position.Y - 1] = cerclePion;
        }

        private void InverserCerclePion(Coordonnee position)
        {
            Ellipse cercle = GrillePions[position.X - 1][position.Y - 1];

            if ((bool)Grille.EstCaseBlanche(position))
            {
                cercle.Fill = CouleurPionAI;
            }
            else
            {
                cercle.Fill = CouleurPionHumain;
            }
        }

        private void GrilleJeu_Click(object sender, MouseButtonEventArgs e)
        {
            Coordonnee position = new Coordonnee(Grid.GetColumn(sender as UIElement), Grid.GetRow(sender as UIElement));

            ExecuterChoixCase(position);
        }

        public void ExecuterChoixCase(Coordonnee position)
        {
            if (Grille.EstCaseBlanche(position) == null)
            {
                // Jouer un coup.
                Grille.AjouterPion(position, TourJeu);
                AjouterCerclePion(position, TourJeu);

                if (TourJeu == Couleur.Blanc)
                {
                    TourJeu = Couleur.Noir;
                }
                else
                {
                    TourJeu = Couleur.Blanc;
                }

                Notify();
            }
            else
            {
                // Inverser une pièce.
                Grille.InverserPion(position);
                InverserCerclePion(position);
            }
        }

        private void btnAllerMenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            SupprimerVue?.Invoke();
        }



        private void btnNouvellePartie_Click(object sender, RoutedEventArgs e)
        {
            NouvellePartie?.Invoke();
        }
    }
}

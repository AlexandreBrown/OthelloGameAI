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
        private List<Rectangle> CasesCoupsPermis { get; set; } = new List<Rectangle>();
        private List<Coordonnee> CoupsPermisHumain { get; set; } = new List<Coordonnee>();

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
            DefinirCouleurJoueurs(CouleurPionHumain, CouleurPionAI);
            
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
            MettreAJourScore();
            AfficherCoupsPermisHumain();

        }

        private void DefinirCouleurJoueurs(SolidColorBrush couleurHumain,SolidColorBrush couleurAi)
        {
            imgCouleurHumain.Source = new BitmapImage(new Uri(("./Ressources/Images/" + couleurHumain.Color.ToString().Substring(3) + ".jpg"), UriKind.Relative));
            imgCouleurAI.Source = new BitmapImage(new Uri(("./Ressources/Images/" + couleurAi.Color.ToString().Substring(3) + ".jpg"), UriKind.Relative));
        }

        private void DefinirGrid()
        {
            InitialiserNomColsLignes();
            InitialiserJeu();
            grdJeuScore.RowDefinitions[1].Height = new GridLength((GrilleJeu.TAILLE_GRILLE_JEU + 1) * TailleCase);
        }

        private void InitialiserNomColsLignes()
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

                    if (Grille.EstCaseLibre(position) == false)
                    {
                        if (Grille.EstCaseBlanche(position))
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

        private bool coupEstLegal(Coordonnee positionAVerifier,Couleur couleurAppelante)
        {
            if(PositionEstValide(positionAVerifier))
            {
                if(couleurAppelante == Couleur.Blanc)
                {
                    foreach (Coordonnee c in IA.CoupsPermisAI)
                    {
                        if (positionAVerifier.X == c.X && positionAVerifier.Y == c.Y)
                        {
                            return true; // Si le coup fait partie des coups permis , le coup est donc légal
                        }
                    }
                }
                else if(couleurAppelante == Couleur.Noir)
                {
                    foreach (Coordonnee c in CoupsPermisHumain)
                    {
                        if (positionAVerifier.X == c.X && positionAVerifier.Y == c.Y)
                        {
                            return true; // Si le coup fait partie des coups permis , le coup est donc légal
                        }
                    }
                }

            }
            return false;
        }

        private List<Coordonnee> TrouverCasesValides(Coordonnee position,Couleur couleurAppelante)
        {
            List<Coordonnee> coupsPermis = new List<Coordonnee>();

            // En haut à gauche
            TesterPositionValide(coupsPermis, new Coordonnee(position.X - 1, position.Y - 1), Emplacement.TopLeft, couleurAppelante);
            // En haut
            TesterPositionValide(coupsPermis, new Coordonnee(position.X, position.Y - 1), Emplacement.Top, couleurAppelante);
            // En haut à droite
            TesterPositionValide(coupsPermis, new Coordonnee(position.X + 1, position.Y - 1), Emplacement.TopRight, couleurAppelante);
            // À droite
            TesterPositionValide(coupsPermis, new Coordonnee(position.X + 1, position.Y), Emplacement.Right, couleurAppelante);
            // En bas à droite
            TesterPositionValide(coupsPermis, new Coordonnee(position.X + 1, position.Y + 1), Emplacement.BottomRight, couleurAppelante);
            // En bas
            TesterPositionValide(coupsPermis, new Coordonnee(position.X, position.Y + 1), Emplacement.Bottom, couleurAppelante);
            // En bas à gauche
            TesterPositionValide(coupsPermis, new Coordonnee(position.X - 1, position.Y + 1), Emplacement.BottomLeft, couleurAppelante);
            // À gauche
            TesterPositionValide(coupsPermis, new Coordonnee(position.X - 1, position.Y), Emplacement.Left, couleurAppelante);

            return coupsPermis;
        }

        private void TesterPositionValide(List<Coordonnee> casesAdjacentesLibres, Coordonnee positionEnVerification, Emplacement emplacementCase, Couleur couleurAppelante)
        {
            if (PositionEstValide(positionEnVerification))
            {
                if (Grille.EstCaseLibre(positionEnVerification))
                {
                    if (TesterPositionEncadrePion(positionEnVerification,emplacementCase, couleurAppelante))
                    {
                        casesAdjacentesLibres.Add(positionEnVerification);
                    }
                }
            }
        }

        private bool PositionEstValide(Coordonnee position)
        {
            return (position.X > 0 && position.Y > 0) && (position.X < GrilleJeu.TAILLE_GRILLE_JEU + 1 && position.Y < GrilleJeu.TAILLE_GRILLE_JEU + 1);
        }

        private bool TesterPositionEncadrePion(Coordonnee position,Emplacement emplacement,Couleur couleurAppelante)
        {
            int nbPionsEncadres = 1;
            bool pieceMemeCouleurRencontree = false;
            bool caseVideRencontree = false;
            // La première case en diagonale sera toujours le pion de l'autre joueur donc il est inutile de le vérifier 
            Coordonnee positionEnVerification = new Coordonnee(position.X, position.Y);
            IncrementerPositionVersOppose(positionEnVerification,emplacement);
            while (PositionEstValide(positionEnVerification) && caseVideRencontree == false && pieceMemeCouleurRencontree == false)
            {
                if (couleurAppelante == Couleur.Blanc)
                {
                    if (Grille.EstCaseLibre(positionEnVerification))
                    {
                        caseVideRencontree = true;
                    }
                    else if (Grille.EstCaseNoire(positionEnVerification))
                    {
                        nbPionsEncadres++;
                    }
                    else if (Grille.EstCaseBlanche(positionEnVerification))
                    {
                        pieceMemeCouleurRencontree = true;
                    }
                }
                else if (couleurAppelante == Couleur.Noir)
                {
                    if (Grille.EstCaseLibre(positionEnVerification))
                    {
                        caseVideRencontree = true;
                    }
                    else if (Grille.EstCaseBlanche(positionEnVerification))
                    {
                        nbPionsEncadres++;
                    }
                    else if (Grille.EstCaseNoire(positionEnVerification))
                    {
                        pieceMemeCouleurRencontree = true;
                    }
                }
                IncrementerPositionVersOppose(positionEnVerification, emplacement);
            }
            return pieceMemeCouleurRencontree;
        }

        private void IncrementerPositionVersOppose(Coordonnee position,Emplacement emplacementCase)
        {
            switch(emplacementCase)
            {
                case Emplacement.TopLeft:
                    IncrementerTopLeft(position);
                    break;
                case Emplacement.Top:
                    IncrementerTop(position);
                    break;
                case Emplacement.TopRight:
                    IncrementerTopRight(position);
                    break;
                case Emplacement.Right:
                    IncrementerRight(position);
                    break;
                case Emplacement.BottomRight:
                    IncrementerBottomRight(position);
                    break;
                case Emplacement.Bottom:
                    IncrementerBottom(position);
                    break;
                case Emplacement.BottomLeft:
                    IncrementerBottomLeft(position);
                    break;
                case Emplacement.Left:
                    IncrementerLeft(position);
                    break;
            }
        }

        private void IncrementerTopLeft(Coordonnee position)
        {
            position.X += 1;
            position.Y += 1;
        }

        private void IncrementerTop(Coordonnee position)
        {
            position.Y += 1;
        }

        private void IncrementerTopRight(Coordonnee position)
        {
            position.X -= 1;
            position.Y += 1;
        }

        private void IncrementerRight(Coordonnee position)
        {
            position.X -= 1;
        }

        private void IncrementerBottomRight(Coordonnee position)
        {
            position.X -= 1;
            position.Y -= 1;
        }

        private void IncrementerBottom(Coordonnee position)
        {
            position.Y -= 1;
        }

        private void IncrementerBottomLeft(Coordonnee position)
        {
            position.X += 1;
            position.Y -= 1;
        }

        private void IncrementerLeft(Coordonnee position)
        {
            position.X += 1;
        }

        private void MettreAJourScore()
        {
            lblAIScore.Content = CalculerNbPiecesAI().ToString();
            lblHumainScore.Content = CalculerNbPiecesHumain().ToString();
            lblScore.Content = (CalculerNbPiecesHumain() * 100).ToString();
        }

        private int CalculerNbPiecesAI()
        {
            int compteur = 0;
            for (int i = 1; i <= GrilleJeu.TAILLE_GRILLE_JEU; i++)
            {
                for (int j = 1; j <= GrilleJeu.TAILLE_GRILLE_JEU; j++)
                {
                    Coordonnee position = new Coordonnee(i, j);
                    if(Grille.EstCaseLibre(position) == false)
                    {
                        if(Grille.EstCaseBlanche(position))
                        {
                            compteur++;
                        }
                    }
                }
            }
            return compteur;
        }

        private int CalculerNbPiecesHumain()
        {
            int compteur = 0;
            for (int i = 1; i <= GrilleJeu.TAILLE_GRILLE_JEU; i++)
            {
                for (int j = 1; j <= GrilleJeu.TAILLE_GRILLE_JEU; j++)
                {
                    Coordonnee position = new Coordonnee(i, j);
                    if (Grille.EstCaseLibre(position) == false)
                    {
                        if (Grille.EstCaseNoire(position))
                        {
                            compteur++;
                        }
                    }
                }
            }
            return compteur;
        }

        private void EffacerCasesCoupsPermis()
        {
            foreach(Rectangle rect in CasesCoupsPermis)
            {
                grdJeu.Children.Remove(rect);
            }
        }

        private void AfficherCoupsPermisHumain()
        {
            MettreAJourCoupsPermisHumain();
            EffacerCasesCoupsPermis();
            for (int i = 0; i < CoupsPermisHumain.Count; i++)
            {
                Rectangle rect = new Rectangle();
                rect.Fill = Brushes.Black;
                rect.Opacity = 1;
                rect.Height = TailleCase - 5;
                rect.Width = TailleCase - 5;
                Grid.SetColumn(rect, CoupsPermisHumain[i].X);
                Grid.SetRow(rect, CoupsPermisHumain[i].Y);
                grdJeu.Children.Add(rect);
                CasesCoupsPermis.Add(rect);
            }
        }

        private void MettreAJourCoupsPermisHumain()
        {
            CoupsPermisHumain = TrouverCoupsPermis(Couleur.Noir);
        }

        public List<Coordonnee> TrouverCoupsPermis(Couleur couleurAppelante)
        {
            List<Coordonnee> coupsPermis = new List<Coordonnee>();
            for (int i = 1; i <= GrilleJeu.TAILLE_GRILLE_JEU; i++)
            {
                for (int j = 1; j <= GrilleJeu.TAILLE_GRILLE_JEU; j++)
                {
                    Coordonnee position = new Coordonnee(i, j);
                    if (couleurAppelante == Couleur.Blanc)
                    {
                        // Si la case n'est pas libre et qu'elle contient une piece de l'humain
                        if (Grille.EstCaseLibre(position) == false && Grille.EstCaseNoire(position))
                        {
                            List<Coordonnee> coupsTrouves = new List<Coordonnee>();
                            // On trouve les cases valides
                            coupsTrouves = TrouverCasesValides(position, couleurAppelante);

                            // On ajoute les coordonnées des coups légaux à notre List de Coordonnee
                            foreach (Coordonnee c in coupsTrouves)
                            {
                                bool coordDejaPresente = false;
                                foreach (Coordonnee coordonDejaPresente in coupsPermis) // On s'assure que la coordonnée n'existe pas déjà dans notre liste de coups permis
                                {
                                    if (coordonDejaPresente == c)
                                    {
                                        coordDejaPresente = true;
                                        break;
                                    }
                                }
                                if (coordDejaPresente == false)
                                {
                                    coupsPermis.Add(c);
                                }
                            }
                        }
                    }
                    else if (couleurAppelante == Couleur.Noir)
                    {
                        // Si la case n'est pas libre et qu'elle contient une piece de l'AI
                        if (Grille.EstCaseLibre(position) == false && Grille.EstCaseBlanche(position))
                        {
                            List<Coordonnee> coupsTrouves = new List<Coordonnee>();
                            // On trouve les cases valides
                            coupsTrouves = TrouverCasesValides(position, couleurAppelante);

                            // On ajoute les coordonnées des coups légaux à notre List de Coordonnee
                            foreach (Coordonnee c in coupsTrouves)
                            {
                                bool coordDejaPresente = false;
                                foreach (Coordonnee coordonDejaPresente in coupsPermis) // On s'assure que la coordonnée n'existe pas déjà dans notre liste de coups permis
                                {
                                    if (coordonDejaPresente == c)
                                    {
                                        coordDejaPresente = true;
                                        break;
                                    }
                                }
                                if (coordDejaPresente == false)
                                {
                                    coupsPermis.Add(c);
                                }
                            }
                        }
                    }
                }
            }
            return coupsPermis;
        }



        private void InverserCerclePion(Coordonnee position)
        {
            Ellipse cercle = GrillePions[position.X - 1][position.Y - 1];

            if (Grille.EstCaseBlanche(position))
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
            ExecuterChoixCase(position,Couleur.Noir);
        }

        public void ExecuterChoixCase(Coordonnee position,Couleur couleurAppelante)
        {
            if (coupEstLegal(new Coordonnee(position.X,position.Y), couleurAppelante))
            {
                // Jouer un coup.
                Grille.AjouterPion(position, TourJeu);
                RafraichirAffichage();
                AjouterCerclePion(position, TourJeu);
                AfficherCoupsPermisHumain();

                if (TourJeu == Couleur.Blanc)
                {
                    TourJeu = Couleur.Noir;
                }
                else
                {
                    TourJeu = Couleur.Blanc;
                }

                Notify();
                MettreAJourScore();
            }
            else
            {
                // Inverser une pièce.
                /*
                 * TODO : USE THISE CODE SOMEWHERE APPROPRIATE
                Grille.InverserPion(position);
                InverserCerclePion(position);
                */
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

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
        private List<Ellipse> CasesCoupsPermis { get; set; } = new List<Ellipse>();
        private List<Coordonnee> CoupsPermisHumain { get; set; } = new List<Coordonnee>();

        public Couleur TourJeu { get; set; }

        private SolidColorBrush CouleurPionHumain { get; set; }
        private SolidColorBrush CouleurPionAI { get; set; }

        private IA_Othello IA { get; set; }

        private NiveauDifficulte Difficulte { get; set; }

        public Action SupprimerVue;
        public Action NouvellePartie { get; set; }

        public bool EnCours { get; set; } = false;


        public JeuOthelloControl(int tailleCase, SolidColorBrush couleurPionHumain, SolidColorBrush couleurPionAI,NiveauDifficulte niveauDifficulte)
        {
            TailleCase = tailleCase;
            CouleurPionHumain = couleurPionHumain;
            CouleurPionAI = couleurPionAI;
            Difficulte = niveauDifficulte;
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
            IA = new IA_Othello(this, niveauDifficulte);
            MettreAJourScore();
            AfficherCasesCoupsPermisHumain();
            SetCouleurTourJeu();
        }

        public JeuOthelloControl(GrilleJeu grilleSource)
        {
            TailleCase = EcranDemarragePartieControl.TailleCaseDefault;
            CouleurPionHumain = EcranDemarragePartieControl.CouleurHumainDefault;
            CouleurPionAI = EcranDemarragePartieControl.CouleurAIDefault;
            InitializeComponent();
            DefinirGrid();
            DefinirCouleurJoueurs(CouleurPionHumain, CouleurPionAI);

            // Initialise la liste d'observateurs.
            observers = new List<IObserver<JeuOthelloControl>>();

            Grille = grilleSource.DeepClone<GrilleJeu>();
            InitialiserGrillePions();
            DessinerCases();
            InitialiserQuadrillageCases();
            RafraichirAffichage();
            TourJeu = Couleur.Noir;
            // Initialiser l'IA.
            IA = new IA_Othello(this,EcranDemarragePartieControl.DifficulteParDefaut);
            MettreAJourScore();
            AfficherCasesCoupsPermisHumain();
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

        public void AjouterCerclePion(Coordonnee position,Couleur couleur)
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

        private bool PositionEstValide(Coordonnee position)
        {
            return (position.X > 0 && position.Y > 0) && (position.X < GrilleJeu.TAILLE_GRILLE_JEU + 1 && position.Y < GrilleJeu.TAILLE_GRILLE_JEU + 1);
        }

        private int PionsEncadresParPosition(Coordonnee positionPieceAdjacenteAdverse, Direction direction,Couleur couleurAppelante)
        {
            int nbPionsEncadres = 1; // Utilisié pour déterminer le score d'un coup
            bool pieceMemeCouleurRencontree = false; // Doit être à true pour que le coup soit valide
            bool caseVideRencontree = false; // Ne doit pas se retrouver à true pour que le coup soit valide
            // La première case sera toujours le pion de l'autre joueur donc il est inutile de la vérifier
            Coordonnee positionEnVerification = IncrementerPosition(positionPieceAdjacenteAdverse, direction);
            // On vérifie si l'encadrement de ce pion est valide et si il n'y a pas d'autre pions qui seraient encadrés de façon légal
            while (caseVideRencontree == false && PositionEstValide(positionEnVerification) && pieceMemeCouleurRencontree == false)
            {
                if (Grille.EstCaseLibre(positionEnVerification))
                {
                    caseVideRencontree = true;
                }
                else
                {
                    if (couleurAppelante == Couleur.Blanc)
                    {
                        if (Grille.EstCaseNoire(positionEnVerification))
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
                        if (Grille.EstCaseBlanche(positionEnVerification))
                        {
                            nbPionsEncadres++;
                        }
                        else if (Grille.EstCaseNoire(positionEnVerification))
                        {
                            pieceMemeCouleurRencontree = true;
                        }
                    }
                    positionEnVerification = IncrementerPosition(positionEnVerification, direction);
                }
            }
            if(pieceMemeCouleurRencontree == true) // Si le coup est valide (il encadre au moins une pièce de façon valide)
            {
                return nbPionsEncadres; // On retourne le nombre de pions qui serait encadrés
            }
            else
            {
                return 0; // Si aucune pièces ne peuvent être encadrées alors 0 est retournée
            }
        }

        private Direction IncrementerDirection(Direction d)
        {
            return (Direction)((int)d + 1);
        }

        private Coordonnee IncrementerPosition(Coordonnee position, Direction directionCase)
        {
            switch (directionCase)
            {
                case Direction.TopLeft:
                    return IncrementerTopLeft(position);
                case Direction.Top:
                    return IncrementerTop(position);
                case Direction.TopRight:
                    return IncrementerTopRight(position);
                case Direction.Right:
                    return IncrementerRight(position);
                case Direction.BottomRight:
                    return IncrementerBottomRight(position);
                case Direction.Bottom:
                    return IncrementerBottom(position);
                case Direction.BottomLeft:
                    return IncrementerBottomLeft(position);
                case Direction.Left:
                    return IncrementerLeft(position);
            }
            return position;
        }

        private Coordonnee IncrementerTopLeft(Coordonnee position)
        {
            Coordonnee positionIncrementee = new Coordonnee(position.X - 1, position.Y - 1);
            return positionIncrementee;
        }

        private Coordonnee IncrementerTop(Coordonnee position)
        {
            Coordonnee positionIncrementee = new Coordonnee(position.X,position.Y - 1);
            return positionIncrementee;
        }

        private Coordonnee IncrementerTopRight(Coordonnee position)
        {
            Coordonnee positionIncrementee = new Coordonnee(position.X + 1, position.Y - 1);
            return positionIncrementee;
        }

        private Coordonnee IncrementerRight(Coordonnee position)
        {
            Coordonnee positionIncrementee = new Coordonnee(position.X + 1, position.Y);
            return positionIncrementee;
        }

        private Coordonnee IncrementerBottomRight(Coordonnee position)
        {
            Coordonnee positionIncrementee = new Coordonnee(position.X + 1, position.Y + 1);
            return positionIncrementee;
        }

        private Coordonnee IncrementerBottom(Coordonnee position)
        {
            Coordonnee positionIncrementee = new Coordonnee(position.X, position.Y + 1);
            return positionIncrementee;
        }

        private Coordonnee IncrementerBottomLeft(Coordonnee position)
        {
            Coordonnee positionIncrementee = new Coordonnee(position.X - 1, position.Y + 1);
            return positionIncrementee;
        }

        private Coordonnee IncrementerLeft(Coordonnee position)
        {
            Coordonnee positionIncrementee = new Coordonnee(position.X - 1, position.Y);
            return positionIncrementee;
        }

        private void MettreAJourScore()
        {
            int MultiplicateurDeScore = 100;
            lblAIScore.Content = Grille.CalculerNbPionsBlancs().ToString();
            lblHumainScore.Content = Grille.CalculerNbPionsNoirs().ToString();
            lblScore.Content = (Grille.CalculerNbPionsNoirs() * MultiplicateurDeScore).ToString();
        }



        private void EffacerCasesCoupsPermisHumain()
        {
            foreach (var casePermise in CasesCoupsPermis)
            {
                grdJeu.Children.Remove(casePermise);
            }
        }

        private void AfficherCasesCoupsPermisHumain()
        {
            MettreAJourCoupsPermis(Couleur.Noir);
            for (int i = 0; i < CoupsPermisHumain.Count; i++)
            {
                Ellipse pionFantome = CreerCercle(Brushes.Black);
                pionFantome.Opacity = 0.25;
                Grid.SetColumn(pionFantome, CoupsPermisHumain[i].X);
                Grid.SetRow(pionFantome, CoupsPermisHumain[i].Y);
                grdJeu.Children.Add(pionFantome);
                CasesCoupsPermis.Add(pionFantome);
            }
        }

        private void MettreAJourCoupsPermis(Couleur couleurAppelante)
        {
            if(couleurAppelante == Couleur.Blanc)
            {
                IA.CoupsPermisAI = TrouverCoupsPermis(Couleur.Blanc);
            }else if(couleurAppelante == Couleur.Noir)
            {
                CoupsPermisHumain = TrouverCoupsPermis(Couleur.Noir);
            }
        }

        public List<Coordonnee> TrouverCoupsPermis(Couleur couleurAppelante)
        {
            List<Coordonnee> coupsPermis = new List<Coordonnee>();
            for (int i = 1; i <= GrilleJeu.TAILLE_GRILLE_JEU; i++)
            {
                for (int j = 1; j <= GrilleJeu.TAILLE_GRILLE_JEU; j++)
                {
                    Coordonnee position = new Coordonnee(i, j);                   
                    AjouterCoupSiPermit(position,coupsPermis, couleurAppelante);
                }
            }
            return coupsPermis;
        }

        private void AjouterCoupSiPermit(Coordonnee position, List<Coordonnee> coupsPermis,Couleur couleurAppelante)
        {
            // Si la case est libre
            if (Grille.EstCaseLibre(position))
            {
                List<Coordonnee> coupsValides = new List<Coordonnee>();
                // On trouve les coups permis
                coupsValides = TrouverCasesValides(position, couleurAppelante);

                if (CoupEstPresentDansListe(position, coupsValides))
                {
                    coupsPermis.Add(position);
                }

            }
        }

        private bool CoupEstPresentDansListe(Coordonnee coup,List<Coordonnee> lstCoups)
        {
            foreach(Coordonnee c in lstCoups)
            {
                if(coup == c)
                {
                    return true;
                }
            }
            return false;
        }

        private List<Coordonnee> TrouverCasesValides(Coordonnee positionInitiale, Couleur couleurAppelante)
        {
            List<Coordonnee> coupsPermis = new List<Coordonnee>();

            Direction directionEnVerification = new Direction();
            directionEnVerification = Direction.TopLeft; // On commence par vérifier la direction : en haut à gauche
            // On vérifie tous les directions possibles
            for (int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++)
            {
                // On se position à la coordonnée associée à la direction à vérifier (ex : En haut à droite du coup , en haut , en haut à droite etc)
                Coordonnee positionEnVerification = IncrementerPosition(positionInitiale, directionEnVerification);
                // Si on suit la direction actuelle à partir de la position et que : 1.la position est valide
                                                                                   //2.On encadre au moins une pièce
                if(PionsEncadresParPositionValide(positionEnVerification,couleurAppelante,directionEnVerification) > 0)
                {
                    // On ajoute le coup à notre liste de coups permis
                    coupsPermis.Add(positionInitiale);
                }
                // En demande la prochaine direction
                directionEnVerification = IncrementerDirection(directionEnVerification);
            }
            return coupsPermis;
        }

        private void InverserCerclePion(Coordonnee position)
        {
            Ellipse cercle = GrillePions[position.X - 1][position.Y - 1];

            if (Grille.EstCaseBlanche(position))
            {
                cercle.Fill = CouleurPionHumain;
            }
            else
            {
                cercle.Fill = CouleurPionAI;
            }
        }

        private void GrilleJeu_Click(object sender, MouseButtonEventArgs e)
        {
            if(TourJeu == Couleur.Noir && PartieTerminee(Couleur.Noir) == false)
            {
                    Coordonnee position = new Coordonnee(Grid.GetColumn(sender as UIElement), Grid.GetRow(sender as UIElement));
                    ExecuterChoixCase(position, Couleur.Noir);
            }
        }

        public void ExecuterChoixCase(Coordonnee position,Couleur couleurAppelante)
        {
            if (coupEstLegal(new Coordonnee(position.X, position.Y), couleurAppelante))
            {
                // Jouer un coup.
                Grille.AjouterPion(position, TourJeu);
                InverserPionsAdverse(position, couleurAppelante);
                RafraichirAffichage();
                AjouterCerclePion(position, TourJeu);
                MettreAJourScore();
                EnCours = true; 
                if (PartieTerminee(TourJeu) == false)
                {
                    if(AdversaireDoitPasserUnTour(TourJeu) == false)
                    {
                        if (TourJeu == Couleur.Blanc)
                        {
                            this.Cursor = Cursors.Arrow;
                            EffacerCasesCoupsPermisHumain();
                            AfficherCasesCoupsPermisHumain();
                            TourJeu = Couleur.Noir;
                        }
                        else
                        {
                            EffacerCasesCoupsPermisHumain();
                            this.Cursor = Cursors.Wait;
                            TourJeu = Couleur.Blanc;
                            Notify();
                        }
                        SetCouleurTourJeu();
                    }
                    else
                    {
                        AfficherMsgAdversairePasseTour(TourJeu);
                        if (TourJeu == Couleur.Blanc)
                        {
                            Notify();
                        }
                    }
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                    EnCours = false;
                    MessageBox.Show("Score final\n"+AfficherScoresEtGagant(),"Partie terminée");
                }
            }
        }

        private void SetCouleurTourJeu()
        {
            if(TourJeu == Couleur.Blanc)
            {
                lblHumain.Foreground = Brushes.Black;
                lblAI.Foreground = Brushes.Blue;
            }
            else if(TourJeu == Couleur.Noir)
            {
                lblAI.Foreground = Brushes.Black;
                lblHumain.Foreground = Brushes.Blue;
            }
        }

        private void AfficherMsgAdversairePasseTour(Couleur TourJeu)
        {
            StringBuilder msg = new StringBuilder();
            if(TourJeu == Couleur.Blanc)
            {
                msg.Append("Aucun coup possible , vous devez passer votre tour!");
            }else
            {
                msg.Append("Aucun coup possible pour l'ordinateur, il doit passer son tour!");
            }
            MessageBox.Show(msg.ToString(),"Information");
        }

        private string AfficherScoresEtGagant()
        {
            StringBuilder msg = new StringBuilder();
            int scoreAI = Grille.CalculerNbPionsBlancs();
            int scoreHumain = Grille.CalculerNbPionsNoirs();
            if(scoreAI > scoreHumain)
            {
                msg.Append("Vous avez perdu!\n");
            }else if (scoreHumain > scoreAI)
            {
                msg.Append("Vous avez gagné!\n");
            }else
            {
                msg.Append("Égalité\n");
            }
            msg.Append("Ordinateur : ").Append(scoreAI.ToString()).Append(" Vous : ").Append(scoreHumain.ToString());
            return msg.ToString();
        }

        public bool PartieTerminee(Couleur joueurActuel)
        {
            return (AdversaireDoitPasserUnTour(joueurActuel) && CoupPossible(joueurActuel) == false);
        }

        private bool AdversaireDoitPasserUnTour(Couleur joueurActuel)
        {
            if(joueurActuel == Couleur.Blanc)
            {
                return (CoupPossible(Couleur.Noir) == false);
            }
            if(joueurActuel == Couleur.Noir)
            {
                return (CoupPossible(Couleur.Blanc) == false);
            }
            return false;
           
        }

        public bool CoupPossible(Couleur couleurAppelante)
        {
            MettreAJourCoupsPermis(couleurAppelante);
            if (couleurAppelante == Couleur.Blanc)
            {
                return IA.CoupsPermisAI.Count > 0;
            }else if (couleurAppelante == Couleur.Noir)
            {
                return CoupsPermisHumain.Count > 0;
            }
            return false;
        } 

        private bool EstPionAdverse(Coordonnee position,Couleur couleurAppelante)
        {
            if(PositionEstValide(position) && Grille.EstCaseLibre(position) == false)
            {
                if(couleurAppelante == Couleur.Blanc)
                {
                    return Grille.EstCaseNoire(position);
                }else if (couleurAppelante == Couleur.Noir)
                {
                    return Grille.EstCaseBlanche(position);
                }
            }
            return false;
        }

        public void InverserPionsAdverse(Coordonnee coup, Couleur couleurAppelante)
        {
            Direction directionEnVerification = new Direction();
            directionEnVerification = Direction.TopLeft; // On commence par vérifier la direction : en haut à gauche
            // On vérifie tous les directions dans le but d'inverser toutes les pions possibles
            for (int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++)
            {
                // On se positionne a une position x du coup selon la direction actuelle ( ex : En haut à droite du coup )
                Coordonnee positionEnVerification = IncrementerPosition(coup, directionEnVerification);
                int nbPionsAInverser = PionsEncadresParPositionValide(positionEnVerification, couleurAppelante, directionEnVerification);
                // Si le nombre de pions à inverser est au moins de un
                if (nbPionsAInverser > 0)
                {
                    // On inverse le nombre de pions trouvé plus haut
                    InverserNbPionsSelonPositionDirection(nbPionsAInverser, positionEnVerification, directionEnVerification, couleurAppelante);
                }
                // On demande la prochaine direction
                directionEnVerification = IncrementerDirection(directionEnVerification);
            }
        }

        private int PionsEncadresParPositionValide(Coordonnee position,Couleur couleurAppelante,Direction direction)
        {
            // On vérifie que la nouvelle position est bien dans l'espace de jeu
            if (PositionEstValide(position))
            {
                // On vérifie que le pion à la position que nous sommes est bien un pion adverse
                if (EstPionAdverse(position, couleurAppelante))
                {
                    // Le nombre de pièces encadrées est retourné
                    return PionsEncadresParPosition(position, direction, couleurAppelante);
                }
            }
            return 0;
        }
    
        private void InverserNbPionsSelonPositionDirection(int nbPionsAInverser,Coordonnee position,Direction direction, Couleur couleurAppelante)
        {
            Coordonnee positionPionAInverser = new Coordonnee(position.X, position.Y);
            for (int i = 0; i < nbPionsAInverser; i++)
            {
                Grille.InverserPion(positionPionAInverser);
                InverserCerclePion(positionPionAInverser);
                positionPionAInverser = IncrementerPosition(positionPionAInverser, direction);
            }
        }

    }
}

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
    /// Interaction logic for ControleurJeuControl.xaml
    /// </summary>
    public partial class ControleurJeuControl : UserControl
    {
        private EcranDemarragePartieControl uctEcranDemarrage { get; set; }
        private JeuOthelloControl uctJeuOthello { get; set; }
        private UserControl ContenuEcran { get; set; }
        public Action SupprimerControleur { get; set; }

        public ControleurJeuControl()
        {
            InitializeComponent();
        }

        private void btnNouvellePartie_Click(object sender, RoutedEventArgs e)
        {

            if(PartieEnCours())
            {
                ValiderEcrasementDePartie();
            }
            else
            {
                InitialiserConfigPartie();
            }
        }

        private void ValiderEcrasementDePartie()
        {
            MessageBoxResult resultat = System.Windows.MessageBox.Show("La partie en cours sera écrasée", "Nouvelle partie?", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.Cancel);
            if (resultat == MessageBoxResult.OK)
            {
                InitialiserConfigPartie();
            }
        }

        private bool PartieEnCours()
        {
            return uctJeuOthello.EnCours;
        }

        private void btnAllerMenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            SupprimerControleur?.Invoke();
        }

        private void InitialiserConfigPartie()
        {
            DesactiverBoutonNouvellePartie();
            uctEcranDemarrage = new EcranDemarragePartieControl();
            uctEcranDemarrage.SupprimerEcranActuel = OnSupprimerEcranActuel;
            uctEcranDemarrage.NouvellePartie = OnNouvellePartie;
            ContenuEcran = uctEcranDemarrage;
            grdConteneurJeu.Children.Add(ContenuEcran);
        }

        private void DesactiverBoutonNouvellePartie()
        {
            btnNouvellePartie.IsEnabled = false;
        }

        private void ActiverBoutonNouvellePartie()
        {
            btnNouvellePartie.IsEnabled = true;
        }

        private void OnSupprimerEcranActuel()
        {
            if(grdConteneurJeu.Children != null)
            {
                grdConteneurJeu.Children.Remove(ContenuEcran);
            }
            ActiverBoutonNouvellePartie();
        }

        private void OnNouvellePartie()
        {
            InitialiserNouvellePartie(uctEcranDemarrage.TailleCase, uctEcranDemarrage.CouleurHumain, uctEcranDemarrage.CouleurAI,uctEcranDemarrage.Difficulte);
            OnSupprimerEcranActuel();
        }

        public void InitialiserNouvellePartie(int tailleCases, SolidColorBrush couleurHumain, SolidColorBrush couleurAI,NiveauDifficulte difficulte)
        {
            RetirerJeuActuel();
            CreerNouveauJeu(tailleCases, couleurHumain, couleurAI, difficulte);
            AjouterJeu();
        }

        private void RetirerJeuActuel()
        {
            if (uctJeuOthello != null)
            {
                grdJeu.Children.Remove(uctJeuOthello); // On retire le jeu actuel si il y en a un
            }
        }

        private void CreerNouveauJeu(int tailleCases, SolidColorBrush couleurHumain, SolidColorBrush couleurAI,NiveauDifficulte difficulte)
        {
            uctJeuOthello = new JeuOthelloControl(tailleCases, couleurHumain, couleurAI, difficulte);
        }

        private void AjouterJeu()
        {
            grdJeu.Children.Add(uctJeuOthello);
        }
    }
}

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
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserControl ContenuEcran { get; set; }
        private InstructionsControl uctInstructions { get; set; }
        private ControleurJeuControl uctControleurJeu { get; set; }
        private EcranDemarragePartieControl uctEcranConfig { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnQuitMainWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnJouer_Click(object sender, RoutedEventArgs e)
        {
            AfficherEcranDemarrage();
        }

        private void AfficherEcranDemarrage()
        {
            uctEcranConfig = new EcranDemarragePartieControl();
            uctEcranConfig.SupprimerEcranActuel = OnSupprimerEcranActuel;
            uctEcranConfig.NouvellePartie = InitialiserPartie;
            ContenuEcran = uctEcranConfig;
            grdConteneur.Children.Add(ContenuEcran);
        }

        private void InitialiserPartie()
        {
            uctControleurJeu = new ControleurJeuControl();
            uctControleurJeu.SupprimerControleur = OnSupprimerEcranActuel;
            uctControleurJeu.InitialiserNouvellePartie(uctEcranConfig.TailleCase, uctEcranConfig.CouleurHumain, uctEcranConfig.CouleurAI, uctEcranConfig.Difficulte);
            OnSupprimerEcranActuel();
            ContenuEcran = uctControleurJeu;
            grdConteneur.Children.Add(ContenuEcran);
        }

        private void OnSupprimerEcranActuel()
        {
            if(grdConteneur.Children != null)
            {
                grdConteneur.Children.Remove(ContenuEcran); // On retire l'écran actuel
            }
        }

        private void btnOuvrirInstructions_Click(object sender, RoutedEventArgs e)
        {
            InitialiserInstructions();
            AfficherInstructions();
        }

        private void InitialiserInstructions()
        {
            uctInstructions = new InstructionsControl();
            uctInstructions.SupprimerInstructions = OnSupprimerEcranActuel;
        }

        private void AfficherInstructions()
        {
            ContenuEcran = uctInstructions;
            grdConteneur.Children.Add(ContenuEcran);
        }
    }
}

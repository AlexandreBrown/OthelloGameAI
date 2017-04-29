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
        private JeuOthelloControl uctJeu { get; set; }
        private EcranDemarragePartieUC uctEcranDemarrage { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnQuitMainWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InitialiserNouvelEcranDemarrage()
        {
            uctEcranDemarrage = new EcranDemarragePartieUC(); // On réinitialise l'écran de démarrage
            uctEcranDemarrage.SupprimerVue = OnSupprimerVueActuelle; // On réattribut la fonction OnDelete à l'action delete
            uctEcranDemarrage.NouvellePartie = OnNouvellePartie; // On fait pointer l'action startGame sur la méthode OnStartGame();
        }

        private void InitialiserJeu()
        {
            SolidColorBrush couleurHumain = new SolidColorBrush();
            SolidColorBrush couleurAI = new SolidColorBrush();
            couleurHumain = uctEcranDemarrage.CouleurHumain;
            couleurAI = uctEcranDemarrage.CouleurAI;

            uctJeu = new JeuOthelloControl(uctEcranDemarrage.TailleCase,couleurHumain, couleurAI);
            uctJeu.SupprimerVue = OnSupprimerVueActuelle;
            uctJeu.NouvellePartie = OnChargerConfigPartie;
        }

        private void btnJouer_Click(object sender, RoutedEventArgs e)
        {
            OnChargerConfigPartie();
        }

        private void OnSupprimerVueActuelle()
        {
            grdConteneur.Children.Remove(ContenuEcran); // On retire l'écran actuel
        }

        private void OnNouvellePartie()
        {
            OnSupprimerVueActuelle(); // On retire l'écran actuel
            InitialiserJeu();
            ContenuEcran = uctJeu;
            grdConteneur.Children.Add(ContenuEcran);
        }

        private void OnChargerConfigPartie()
        {
            OnSupprimerVueActuelle();
            InitialiserNouvelEcranDemarrage();
            ContenuEcran = uctEcranDemarrage;
            grdConteneur.Children.Add(ContenuEcran);
        }
    }
}

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

        private void InitializeNewEcranDemarrage()
        {
            uctEcranDemarrage = new EcranDemarragePartieUC(); // On réinitialise l'écran de démarrage
            uctEcranDemarrage.delete = OnDeleteCurrentView; // On réattribut la fonction OnDelete à l'action delete
            uctEcranDemarrage.startGame = OnStartGame; // On fait pointer l'action startGame sur la méthode OnStartGame();
        }

        private void btnJouer_Click(object sender, RoutedEventArgs e)
        {
            InitializeNewEcranDemarrage();
            ContenuEcran = uctEcranDemarrage;
            grdConteneur.Children.Add(ContenuEcran);
        }

        private void OnDeleteCurrentView()
        {
            grdConteneur.Children.Remove(ContenuEcran); // On retire l'écran actuel
        }

        private void OnStartGame()
        {
            OnDeleteCurrentView(); // On retire l'écran actuel
            uctJeu = new JeuOthelloControl(); // On construit le jeu
            ContenuEcran = uctJeu;
            grdConteneur.Children.Add(ContenuEcran);
        }
    }
}

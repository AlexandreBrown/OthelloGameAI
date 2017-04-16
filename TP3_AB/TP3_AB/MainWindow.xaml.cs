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
        private JeuOthelloControl Jeu { get; set; }
        private EcranDemarragePartieUC EcranDemarrage { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
            Jeu = new JeuOthelloControl();
            EcranDemarrage = new EcranDemarragePartieUC();
        }

        private void btnQuitMainWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnJouer_Click(object sender, RoutedEventArgs e)
        {

            ContenuEcran = EcranDemarrage;

            grdConteneur.Children.Add(ContenuEcran);
        }
    }
}

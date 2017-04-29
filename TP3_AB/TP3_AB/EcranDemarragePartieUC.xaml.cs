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
    /// Interaction logic for EcranDemarragePartieUC.xaml
    /// </summary>
    public partial class EcranDemarragePartieUC : UserControl
    {
        public Action SupprimerVue { get; set; }
        public Action NouvellePartie { get; set; }
        public int TailleCase { get; set; }
        public SolidColorBrush CouleurHumain { get; set; }
        public SolidColorBrush CouleurAI { get; set; }
        private const int TailleCaseDefault = 50;

        public EcranDemarragePartieUC()
        {
            InitializeComponent();
            InitialiserValeursDefaut();
        }

        private void SetTailleCasePreview(int newValue)
        {
            recCasePreview.Height = newValue;
            recCasePreview.Width = newValue;
            brdCasePreview.Height = newValue;
            brdCasePreview.Width = newValue;
            txbPixels.Text = newValue.ToString();
            sldTailleCase.Value = newValue;
        }

        private void InitialiserValeursDefaut()
        {
            SetTailleCasePreview(TailleCaseDefault);
            rdbCouleur01.IsChecked = true;
        }

        private void sldTailleCase_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetTailleCasePreview((int)((Slider)sender).Value);
        }

        private void btnDefaultValues_Click(object sender, RoutedEventArgs e)
        {
            InitialiserValeursDefaut();
        }

        private void btnAnnulerConfigPartie_Click(object sender, RoutedEventArgs e)
        {
            SupprimerVue?.Invoke(); // Si Delete n'est pas null on call la méthode sur laquelle l'action delete pointe
        }

        private void btnDebutPartie_Click(object sender, RoutedEventArgs e)
        {
            TailleCase = (int)sldTailleCase.Value;
            NouvellePartie?.Invoke(); // On vérifie que StartGame n'est pas null , si StartGame n'est pas null alors la méthode est appelé
        }

        private void rdbCouleur01_Checked(object sender, RoutedEventArgs e)
        {
            CouleurHumain = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
            CouleurAI = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
        }

        private void rdbCouleur02_Checked(object sender, RoutedEventArgs e)
        {
            CouleurHumain = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0000"));
            CouleurAI = (SolidColorBrush)(new BrushConverter().ConvertFrom("#0000FF"));
        }

        private void rdbCouleur03_Checked(object sender, RoutedEventArgs e)
        {
            CouleurHumain = (SolidColorBrush)(new BrushConverter().ConvertFrom("#EC008C"));
            CouleurAI = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00BFF3"));
        }

        private void rdbCouleur04_Checked(object sender, RoutedEventArgs e)
        {
            CouleurHumain = (SolidColorBrush)(new BrushConverter().ConvertFrom("#603913"));
            CouleurAI = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ABA000"));
        }

    }
}

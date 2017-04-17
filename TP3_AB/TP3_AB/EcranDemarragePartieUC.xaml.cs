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
        public Action delete;
        public Action startGame;
        private const int TailleCaseDefault = 50;

        public EcranDemarragePartieUC()
        {
            InitializeComponent();
            InitializeDefaultValue();
        }

        private void SetTailleCasePreviewValue(int newValue)
        {
            recCasePreview.Height = newValue;
            recCasePreview.Width = newValue;
            brdCasePreview.Height = newValue;
            brdCasePreview.Width = newValue;
            txbPixels.Text = newValue.ToString();
            sldTailleCase.Value = newValue;
        }

        private void InitializeDefaultValue()
        {
            SetTailleCasePreviewValue(TailleCaseDefault);
            rdbCouleur01.IsChecked = true;
        }

        private void sldTailleCase_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetTailleCasePreviewValue((int)((Slider)sender).Value);
        }

        private void btnDefaultValues_Click(object sender, RoutedEventArgs e)
        {
            InitializeDefaultValue();
        }

        private void btnAnnulerConfigPartie_Click(object sender, RoutedEventArgs e)
        {
            delete?.Invoke(); // Si delete n'est pas null on call la méthode sur laquelle l'action delete pointe
        }

        private void btnDebutPartie_Click(object sender, RoutedEventArgs e)
        {
            startGame?.Invoke(); // On vérifie que startGame n'est pas null , si newGame n'est pas null alors la méthode est appelé
        }
    }
}

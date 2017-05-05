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
    /// Interaction logic for InstructionsControl.xaml
    /// </summary>
    public partial class InstructionsControl : UserControl
    {
        public Action SupprimerInstructions { get; set; }
        public InstructionsControl()
        {
            InitializeComponent();
        }
        private void btnQuitter_Click(object sender, RoutedEventArgs e)
        {
            SupprimerInstructions?.Invoke();
        }
    }
}

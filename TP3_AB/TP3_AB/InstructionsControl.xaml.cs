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
            DefinirInstructions();
        }

        private void DefinirInstructions()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("• Le jeu se joue sur une grille de 64 cases (8 par 8).");
            text.AppendLine("\to Les coordonnées des cases vont de A à H de gauche à droite et de 1 à 8 du haut vers le bas.");
            text.Append("\n");
            text.AppendLine("• Au début de la partie :");
            text.AppendLine("\to Deux pions blancs sont placés en D4 et E5.");
            text.AppendLine("\to Deux pions noirs sont placés en E4 et D5.");
            text.Append("\n");
            text.AppendLine("• Le premier joueur est toujours celui qui contrôle les pions noirs.");
            text.Append("\n");
            text.AppendLine("• Il y a deux conditions à vérifier pour qu’un coup soit considéré comme étant légal.");
            text.AppendLine("\to Le pion doit être placé sur une case vide qui est adjacente à un pion adverse.");
            text.AppendLine("\to Le pion doit de plus encadrer au moins un pion de l’adversaire.");
            text.AppendLine("\t\to Par « encadrer », on entend qu’au moins un pion de la couleur de l’adversaire se retrouvera positionné entre le nouveau");
            text.AppendLine("\t\t  pion du joueur et un pion de ce même joueur déjà présent en jeu.Ceci peut être fait sur un axe horizontal, vertical ou");
            text.AppendLine("\t\t  diagonal.");
            text.Append("\n");
            text.AppendLine("• Quand un coup est joué, tous les pions de l’adversaire qui sont encadrés changent de couleur et prennent celle du joueur qui vient de jouer son coup.");
            text.Append("\n");
            text.AppendLine("• Si aucun coup légal n’existe pour un joueur, il passe son tour.");
            text.AppendLine("\to Si les deux joueurs passent leur tour l’un après l’autre, la partie se termine.");
            text.Append("\n");
            text.AppendLine("• Lorsqu’une partie est terminée, le joueur ayant la couleur avec le plus de pions en jeu gagne.");
            txbInstructions.Text = text.ToString();
        }

        private void btnQuitter_Click(object sender, RoutedEventArgs e)
        {
            SupprimerInstructions?.Invoke();
        }

        private void mediaExempleJeuOthello_MediaEnded(object sender, RoutedEventArgs e)
        {
            ((MediaElement)sender).Position = TimeSpan.FromMilliseconds(1);
        }
    }
}

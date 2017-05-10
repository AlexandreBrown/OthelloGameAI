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
    public enum NiveauDifficulte { Facile,Normal,Difficile,Professionnel }
    /// <summary>
    /// Interaction logic for EcranDemarragePartieControl.xaml
    /// </summary>
    public partial class EcranDemarragePartieControl : UserControl
    {
        public Action SupprimerEcranActuel { get; set; }
        public Action NouvellePartie { get; set; }
        public int TailleCase { get; set; }
        public SolidColorBrush CouleurHumain { get; set; }
        public SolidColorBrush CouleurAI { get; set; }
        public NiveauDifficulte Difficulte { get; set; }
        public static NiveauDifficulte DifficulteParDefaut { get; set; } = NiveauDifficulte.Normal;
        public static int TailleCaseDefault = 50;
        public static SolidColorBrush CouleurHumainDefault = (SolidColorBrush) (new BrushConverter().ConvertFrom("#000000"));
        public static SolidColorBrush CouleurAIDefault = (SolidColorBrush) (new BrushConverter().ConvertFrom("#FFFFFF"));

        public EcranDemarragePartieControl()
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
            SetSelectedColor(1);
            rdbCouleur01.IsChecked = true;
            Difficulte = DifficulteParDefaut;
            MettreAJourDifficulteAfficher();
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
            SupprimerEcranActuel?.Invoke(); // Si Delete n'est pas null on call la méthode sur laquelle l'action delete pointe
        }

        private void btnDebutPartie_Click(object sender, RoutedEventArgs e)
        {
            TailleCase = (int)sldTailleCase.Value;
            NouvellePartie?.Invoke(); // On vérifie que StartGame n'est pas null , si StartGame n'est pas null alors la méthode est appelé
        }

        private void SetSelectedColor(int index)
        {
            switch (index)
            {
                case 1:
                    CouleurHumain = CouleurHumainDefault;
                    CouleurAI = CouleurAIDefault;
                    break;
                case 2:
                    CouleurHumain = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0000"));
                    CouleurAI = (SolidColorBrush)(new BrushConverter().ConvertFrom("#0000FF"));
                    break;
                case 3:
                    CouleurHumain = (SolidColorBrush)(new BrushConverter().ConvertFrom("#EC008C"));
                    CouleurAI = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00BFF3"));
                    break;
                case 4:
                    CouleurHumain = (SolidColorBrush)(new BrushConverter().ConvertFrom("#603913"));
                    CouleurAI = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ABA000"));
                    break;
            }
        }

        private void stpCouleur01_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            rdbCouleur01.IsChecked = true;
            SetSelectedColor(1);
        }

        private void stpCouleur02_PreviewMouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            rdbCouleur02.IsChecked = true;
            SetSelectedColor(2);
        }
        private void stpCouleur03_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            rdbCouleur03.IsChecked = true;
            SetSelectedColor(3);
        }

        private void stpCouleur4_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            rdbCouleur04.IsChecked = true;
            SetSelectedColor(4);
        }

        private void AugmenterDifficulte()
        {
            int augmentation= 1;
            if((int)Difficulte < Enum.GetNames(typeof(NiveauDifficulte)).Length -1 )
            {
                Difficulte = (NiveauDifficulte)((int)Difficulte + augmentation);
            }
        }

        private void ReduireDifficulte()
        {
            int reduction = 1;
            if ((int)Difficulte > (int)NiveauDifficulte.Facile )
            {
                Difficulte = (NiveauDifficulte)((int)Difficulte - reduction);
            }
        }

        private void MettreAJourDifficulteAfficher()
        {
            string nouvelleDiff = "";
            switch((int)Difficulte)
            {
                case (int)NiveauDifficulte.Facile:
                    nouvelleDiff = "Facile";
                    break;
                case (int)NiveauDifficulte.Normal:
                    nouvelleDiff = "Normal";
                    break;
                case (int)NiveauDifficulte.Difficile:
                    nouvelleDiff = "Difficile";
                    break;
                case (int)NiveauDifficulte.Professionnel:
                    nouvelleDiff = "Professionnel";
                    break;
            }
            lblDifficulte.Content = nouvelleDiff;
        }

        private void viewBoxReduireDifficulte_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReduireDifficulte();
            MettreAJourDifficulteAfficher();
        }

        private void viewBoxAugmenterDifficulte_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AugmenterDifficulte();
            MettreAJourDifficulteAfficher();
        }
    }
}

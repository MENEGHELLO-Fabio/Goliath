using System;
using System.Collections.Generic;
using System.IO;
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

namespace Goliath
{
    /// <summary>
    /// Logica di interazione per cardAllenamento.xaml
    /// </summary>
    public partial class cardAllenamento : UserControl
    {
        private routine selectedRoutine = null;

        public cardAllenamento(routine routineScelta)
        {
            InitializeComponent();

            selectedRoutine = routineScelta;

            grid3.Visibility = Visibility.Visible;

            visualizzaAllenamento();
        }

    
     


        private void visualizzaAllenamento()
        {
            panel.Children.Clear();



            foreach (var esercizio in selectedRoutine.GetEsercizi())
            {
                var card = new EsercizioCard();
                card.nomeEsercizioBlock.Text = esercizio.NomeEsercizio;
                card.serieBlock.Text = esercizio.Serie.ToString();
                card.repBlock.Text = esercizio.Ripetizioni.ToString();
                //card.videoBox.Source = new Uri(esercizio.VideoPath, UriKind.RelativeOrAbsolute);
                string fullPath = System.IO.Path.GetFullPath(esercizio.VideoPath);
                card.videoBox.Source = new Uri(fullPath, UriKind.Absolute);
                card.videoBox.Play();

                panel.Children.Add(card);
            }
        }
    }
}

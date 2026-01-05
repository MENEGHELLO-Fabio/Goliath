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

            // iterate exercises in reverse order so last appears first
            var esercizi = selectedRoutine.GetEsercizi();
            for (int i = esercizi.Count - 1; i >= 0; i--)
            {
                var esercizio = esercizi[i];

                // Passo la lista delle serie al costruttore
                var card = new EsercizioCard(esercizio.getSerie());

                // Nome esercizio
                card.nomeEsercizioBlock.Text = esercizio.NomeEsercizio;

                // Ripetizioni della prima serie (se esiste)
                if (esercizio.getSerie().Count > 0)
                    card.repBlock.Text = esercizio.getSerie()[0].Ripetizioni.ToString();
                else
                    card.repBlock.Text = "-";

                // Video
                string fullPath = System.IO.Path.GetFullPath(esercizio.VideoPath);
                card.videoBox.Source = new Uri(fullPath, UriKind.Absolute);
                card.videoBox.Play();

                panel.Children.Add(card);
            }
        }

        private static string FindProjectFolder(string projectFolderName)
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null && !string.Equals(dir.Name, projectFolderName, StringComparison.OrdinalIgnoreCase))
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}

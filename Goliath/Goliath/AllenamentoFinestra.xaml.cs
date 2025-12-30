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
using System.Windows.Shapes;

namespace Goliath
{
    /// <summary>
    /// Logica di interazione per AllenamentoFinestra.xaml
    /// </summary>
    public partial class AllenamentoFinestra : Window
    {
        private readonly List<routine> loadedRoutines = new();
        private routine selectedRoutine = null;
        public routine allenamentoDaTornare;

        public AllenamentoFinestra()
        {
            InitializeComponent();
            selectedRoutine = null;

            grid2.Visibility = Visibility.Visible;
            grid3.Visibility = Visibility.Collapsed;

            caricaRoutines();

        }

        private void caricaRoutines()
        {
            const string filePath = "routines.csv";


            loadedRoutines.Clear();

            if (!File.Exists(filePath))
            {
                // non esiste file: pulisci UI
                RefreshRoutinesListView();
                return;
            }

            try
            {
                routine current = null;

                foreach (var raw in File.ReadLines(filePath))
                {
                    var line = (raw ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Trova la riga che contiene l'header che caratterizza l'inizio della routine
                    if (line.StartsWith("ROUTINE;", StringComparison.OrdinalIgnoreCase))
                    {

                        var parts = line.Split(new[] { ';' }, 2);
                        var nome = parts.Length > 1 ? parts[1].Trim() : "UnnamedRoutine";
                        current = new routine { NomeRoutine = nome };
                        loadedRoutines.Add(current);
                        continue;
                    }

                    // Trova una riga contenente le informazioni dell'esercizio
                    if (line.StartsWith("EX;", StringComparison.OrdinalIgnoreCase))
                    {

                        if (current == null)
                        {
                            // esercizio senza routine: ignora
                            continue;
                        }

                        var fields = line.Substring(3).Split(';').Select(p => p.Trim()).ToArray();
                        if (fields.Length < 6) continue;

                        string nomeEsercizio = fields[0];
                        int serie = int.TryParse(fields[1], out var s) ? s : 0;
                        int ripetizioni = int.TryParse(fields[2], out var r) ? r : 0;
                        int carico = int.TryParse(fields[3], out var c) ? c : 0;
                        int rpe = int.TryParse(fields[4], out var ppe) ? ppe : 0;
                        string videoPath = fields[5];


                        var ex = new esercizio(nomeEsercizio, serie, ripetizioni, carico, rpe)
                        {
                            VideoPath = videoPath
                        };

                        current.AddEsercizio(ex);
                        continue;
                    }


                }

                // aggiorna la UI
                RefreshRoutinesListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante il caricamento delle routine: " + ex.Message, "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshRoutinesListView()
        {
            routinesList.DisplayMemberPath = "NomeRoutine";
            routinesList.ItemsSource = null;
            routinesList.ItemsSource = loadedRoutines;
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            if (routinesList.SelectedItem == null)
            {
                return;
            }

            selectedRoutine = (routine)routinesList.SelectedItem;

            grid2.Visibility = Visibility.Collapsed;
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

        private void buttonIndietro_Click(object sender, RoutedEventArgs e)
        {

            if (selectedRoutine!= null)
            {
                //salva allenamento su csv

                const string filePath = "allenamenti.csv";

                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, "");
                }

                using (StreamWriter writer = new StreamWriter(filePath, append: true))
                {
                    writer.WriteLine($"ALLENAMENTO;{selectedRoutine.NomeRoutine};{DateTime.Now:dd-MM-yyyy}");
                    foreach (var esercizio in selectedRoutine.GetEsercizi())
                    {
                        writer.WriteLine(
                            $"EXE;{esercizio.NomeEsercizio};{esercizio.Serie};{esercizio.Ripetizioni};{esercizio.Carico};{esercizio.RPE};{esercizio.VideoPath}"
                        );
                    }
                }
            }
           

            //ritorno a main window

            MainWindow main = new MainWindow();
            main.Show();
            if (selectedRoutine != null)
            {
               allenamentoDaTornare = selectedRoutine;
                this.DialogResult = true;
            }
            
            this.Close();


        }

    }
    
}

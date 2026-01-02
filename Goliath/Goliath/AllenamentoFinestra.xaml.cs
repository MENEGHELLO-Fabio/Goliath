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
                routine currentRoutine = null;
                esercizio currentExercise = null;

                foreach (var raw in File.ReadLines(filePath))
                {
                    var line = (raw ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Trova la riga che contiene l'header che caratterizza l'inizio della routine
                    if (line.StartsWith("ROUTINE;", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = line.Split(';');
                        var nome = parts.Length > 1 ? parts[1].Trim() : "UnnamedRoutine";

                        currentRoutine = new routine { NomeRoutine = nome };
                        loadedRoutines.Add(currentRoutine);
                        currentExercise = null;
                        continue;
                    }
                    // Trova una riga contenente le informazioni dell'esercizio
                    if (line.StartsWith("EX;", StringComparison.OrdinalIgnoreCase))
                    {
                        if (currentRoutine == null) continue;

                        var parts = line.Split(';');
                        if (parts.Length < 3) continue;

                        string nomeEsercizio = parts[1];
                        string videoPath = parts[2];

                        currentExercise = new esercizio(nomeEsercizio, new List<serie>());
                        currentExercise.VideoPath = videoPath;

                        currentRoutine.AddEsercizio(currentExercise);
                        continue;
                    }

                    // Serie dell'esercizio
                    if (line.StartsWith("SERIE;", StringComparison.OrdinalIgnoreCase))
                    {
                        if (currentExercise == null) continue;

                        var parts = line.Split(';');
                        if (parts.Length < 3) continue;

                        int rip = int.Parse(parts[1]);
                        int car = int.Parse(parts[2]);
              

                        currentExercise.addSerie(new serie(rip, car) );
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
                var card = new EsercizioCard(esercizio.getSerie());

                card.nomeEsercizioBlock.Text = esercizio.NomeEsercizio;

                
                // ripetizioni della prima serie (se esiste)
                if (esercizio.getSerie().Count > 0)
                    card.repBlock.Text = esercizio.getSerie()[0].Ripetizioni.ToString();
                else
                    card.repBlock.Text = "-";

                string fullPath = System.IO.Path.GetFullPath(esercizio.VideoPath);
                card.videoBox.Source = new Uri(fullPath, UriKind.Absolute);
                card.videoBox.Play();

                panel.Children.Add(card);
            }
        }

        private void buttonIndietro_Click(object sender, RoutedEventArgs e)
        {

            if (selectedRoutine != null)
            {
                //salva allenamento su csv
                const string filePath = "allenamenti.csv";

                if (!File.Exists(filePath))
                    File.WriteAllText(filePath, "");

                using (StreamWriter writer = new StreamWriter(filePath, append: true))
                {
                    writer.WriteLine($"ALLENAMENTO;{selectedRoutine.NomeRoutine};{DateTime.Now:dd-MM-yyyy}");

                    foreach (var esercizio in selectedRoutine.GetEsercizi())
                    {
                        writer.WriteLine($"EXE;{esercizio.NomeEsercizio};{esercizio.VideoPath}");

                        foreach (var s in esercizio.getSerie())
                        {
                            writer.WriteLine($"SERIE;{s.Ripetizioni};{s.Carico};");
                        }
                    }

                    writer.WriteLine(""); // separatore
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

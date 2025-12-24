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
        routine selectedRoutine = null;
        public AllenamentoFinestra()
        {
            InitializeComponent();
            grid2.Visibility = Visibility.Visible;
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
            if (routinesList.SelectedItem==null)
            {
                return;
            }

            selectedRoutine = (routine)routinesList.SelectedItem;
            
            grid2.Visibility = Visibility.Collapsed;
        }

        private void visualizzaAllenamento()
        {
            panel.Add
        }
    }
}

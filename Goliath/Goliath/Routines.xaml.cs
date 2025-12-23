using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;

namespace Goliath
{
    /// <summary>
    /// Logica di interazione per Routines.xaml
    /// </summary>
    public partial class Routines : Window
    {
        // lista interna per mantenere le routine caricate
        private readonly List<routine> loadedRoutines = new();

        public Routines()
        {
            InitializeComponent();
            caricaRoutines();
        }

        private void ButtonAggiungiRoutine_Click(object sender, RoutedEventArgs e)
        {
            CreaRoutine creaRoutineWindow = new CreaRoutine();
            if (creaRoutineWindow.ShowDialog() == true)
            {
                routine r = creaRoutineWindow.currentRoutine;
                // aggiungi alla lista interna e aggiorna la ListView
                loadedRoutines.Add(r);
                RefreshRoutinesListView();
            }
        }

        private void buttonHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Close();
            main.ShowDialog();
        }

        // legge il file "routines.csv" e costruisce oggetti routine con esercizi interni
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

        // aggiorna la ListView mostrando i nomi delle routine
        private void RefreshRoutinesListView()
        {
            routinesList.DisplayMemberPath = "NomeRoutine";
            routinesList.ItemsSource = null;
            routinesList.ItemsSource = loadedRoutines;
        }

        private void buttonRimuoviRoutine1_Click(object sender, RoutedEventArgs e)   // cerca nel file il titolo della routine selezionata, quando lo trova rimuove tutte le linee sottostanti fino al prossimo titolo di routine permettendo cosi di cancellarla singolarmente
        {
            if (routinesList.SelectedItem == null)
            {
                MessageBox.Show("Seleziona una routine da rimuovere.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var selectedRoutine = routinesList.SelectedItem as routine;
            if (selectedRoutine == null)
            {
                MessageBox.Show("Elemento selezionato non valido.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            const string filePath = "routines.csv";
            if (!File.Exists(filePath))
            {
                MessageBox.Show("File routines.csv non trovato.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var allLines = File.ReadAllLines(filePath).ToList();
                var lineeDaTenere = new List<string>();
                bool removedBlock = false;

                for (int i = 0; i < allLines.Count; i++)
                {
                    var raw = allLines[i];
                    var line = (raw ?? string.Empty).Trim();

                    if (!removedBlock && line.StartsWith("ROUTINE;", StringComparison.OrdinalIgnoreCase))
                    {
                        // Trova il nome della routine selezionata
                        var parts = line.Split(new[] { ';' }, 2);
                        var nome = parts.Length > 1 ? parts[1].Trim() : string.Empty;

                        if (string.Equals(nome, selectedRoutine.NomeRoutine, StringComparison.OrdinalIgnoreCase))
                        {
                            // salta il blocco: dalla riga corrente fino alla riga precedente alla prossima ROUTINE
                            removedBlock = true;
                            // avanza l'indice fino al prossimo header ROUTINE oppure fino alla fine delle righe scritte
                            i++;
                            for (; i < allLines.Count; i++)
                            {
                                var l = (allLines[i] ?? string.Empty).Trim();
                                if (l.StartsWith("ROUTINE;", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Riesaminazione della stessa riga
                                    i--;
                                    break;
                                }
                            }
                            continue;
                        }
                    }

                    // nel caso in cui stiamo in una riga che va mantenuta viene aggiunta alle LineeDaTenere
                    lineeDaTenere.Add(allLines[i]);
                }

                if (!removedBlock)
                {
                    MessageBox.Show("Routine non trovata nel file.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // riscrivi il file senza il blocco rimosso
                File.WriteAllLines(filePath, lineeDaTenere);

                // rimuovi dalla lista in memoria e aggiorna UI
                loadedRoutines.Remove(selectedRoutine);
                RefreshRoutinesListView();

                MessageBox.Show("Routine rimossa correttamente.", "Rimozione", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante la rimozione della routine: " + ex.Message, "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
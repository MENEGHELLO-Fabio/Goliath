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
                RefreshRoutinesListView();
                return;
            }

            try
            {
                routine currentRoutine = null;
                esercizio currentExercise = null;

                foreach (var raw in File.ReadLines(filePath))
                {
                    var line = (raw ?? "").Trim();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Nuova routine
                    if (line.StartsWith("ROUTINE;", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = line.Split(';');
                        string nome = parts.Length > 1 ? parts[1] : "UnnamedRoutine";

                        currentRoutine = new routine { NomeRoutine = nome };
                        loadedRoutines.Add(currentRoutine);
                        currentExercise = null;
                        continue;
                    }

                    // Nuovo esercizio
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
                        if (parts.Length < 4) continue;

                        int rip = int.Parse(parts[1]);
                        int car = int.Parse(parts[2]);


                        currentExercise.addSerie(new serie(rip, car));
                        continue;
                    }
                }

                RefreshRoutinesListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante il caricamento delle routine: " + ex.Message,
                                "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
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
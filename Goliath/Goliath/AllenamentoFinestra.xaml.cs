using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        // variabili per tenere traccia della card e della serie attiva
        private EsercizioCard cardAttiva;
        private serie serieAttiva;

        public AllenamentoFinestra()
        {
            InitializeComponent();
            selectedRoutine = null;

            grid2.Visibility = Visibility.Visible;
            grid3.Visibility = Visibility.Collapsed;
            grid4.Visibility = Visibility.Collapsed;

            caricaRoutines();
        }

        // Legge routines da routines.csv e carica solo quelle del profilo loggato
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
                string currentProfile = ProfileHelper.GetCurrentProfileUsername()?.Trim();

                routine currentRoutine = null;
                esercizio currentExercise = null;

                foreach (var raw in File.ReadLines(filePath))
                {
                    var line = (raw ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.StartsWith("ROUTINE;", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = line.Split(';');
                        if (parts.Length >= 3)
                        {
                            var owner = parts[1].Trim();
                            var nome = parts.Length > 2 ? parts[2].Trim() : "UnnamedRoutine";

                            if (!string.IsNullOrEmpty(currentProfile))
                            {
                                if (!string.Equals(owner, currentProfile, StringComparison.OrdinalIgnoreCase))
                                {
                                    currentRoutine = null;
                                    currentExercise = null;
                                    continue;
                                }
                            }
                            else
                            {
                                // se non loggato non caricare routine con owner
                                currentRoutine = null; currentExercise = null; continue;
                            }

                            currentRoutine = new routine { NomeRoutine = nome };
                            loadedRoutines.Add(currentRoutine);
                            currentExercise = null;
                            continue;
                        }

                        // legacy: ROUTINE;RoutineName
                        var nomeLegacy = parts.Length > 1 ? parts[1].Trim() : "UnnamedRoutine";
                        if (!string.IsNullOrEmpty(currentProfile))
                        {
                            currentRoutine = null; currentExercise = null; continue;
                        }
                        currentRoutine = new routine { NomeRoutine = nomeLegacy };
                        loadedRoutines.Add(currentRoutine);
                        currentExercise = null;
                        continue;
                    }

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

                    if (line.StartsWith("SERIE;", StringComparison.OrdinalIgnoreCase))
                    {
                        if (currentExercise == null) continue;

                        var parts = line.Split(';');
                        if (parts.Length < 3) continue;

                        if (!int.TryParse(parts[1], out int rip)) continue;
                        if (!int.TryParse(parts[2], out int car)) continue;

                        currentExercise.addSerie(new serie(rip, car));
                        continue;
                    }
                }

                // aggiorna la UI
                RefreshRoutinesListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante il caricamento delle routine");
            }
        }

        // Aggiorna la ListView mostrando i nomi delle routine
        private void RefreshRoutinesListView()
        {
            routinesList.DisplayMemberPath = "NomeRoutine";
            routinesList.ItemsSource = null;
            routinesList.ItemsSource = loadedRoutines;
        }

        // Avvia l'allenamento selezionato e mostra la UI di esecuzione
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            if (routinesList.SelectedItem == null) return;

            selectedRoutine = (routine)routinesList.SelectedItem;

            grid2.Visibility = Visibility.Collapsed;
            grid3.Visibility = Visibility.Visible;
            grid4.Visibility = Visibility.Visible;

            visualizzaAllenamento();
        }

        // Costruisce le card dell'allenamento per ogni esercizio della routine selezionata
        private void visualizzaAllenamento()
        {
            panel.Children.Clear();

            // iterate exercises in reverse so last appears first
            var esercizi = selectedRoutine.GetEsercizi();
            for (int i = esercizi.Count - 1; i >= 0; i--)
            {
                var esercizio = esercizi[i];

                var card = new EsercizioCard(esercizio.getSerie());
                card.SerieSelezionata += Card_SerieSelezionata;

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

        // Salva l'allenamento corrente su allenamenti.csv e ritorna alla Main
        private void buttonIndietro_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRoutine != null)
            {
                // salva allenamento su csv
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

            // ritorno a main window
            MainWindow main = new MainWindow();
            main.Show();
            if (selectedRoutine != null)
            {
                allenamentoDaTornare = selectedRoutine;
                this.DialogResult = true;
            }

            this.Close();
        }

        // Imposta i valori della serie selezionata (update UI e modello)
        private void buttonImposta_Click(object sender, RoutedEventArgs e)
        {
            if (serieAttiva == null) return;

            if (!int.TryParse(boxRepFatte.Text, out int nuoveRep)) return;
            if (!int.TryParse(boxCaricoUsato.Text, out int nuovoCarico)) return;

            // aggiorno i valori della serie
            serieAttiva.Ripetizioni = nuoveRep;
            serieAttiva.Carico = nuovoCarico;

            // aggiorno la UI della card
            cardAttiva.repBlock.Text = nuoveRep.ToString();

            // aggiorno la ListBox della card 
            cardAttiva.SerieList.Items.Refresh();
        }

        // Metodo per comunicare dalla card quale serie è stata selezionata
        private void Card_SerieSelezionata(EsercizioCard card, serie serie)
        {
            cardAttiva = card;
            serieAttiva = serie;
        }
    }
}

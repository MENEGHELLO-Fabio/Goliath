using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Goliath
{
    /// <summary>
    /// Logica di interazione per aggiungiEsercizio.xaml
    /// </summary>
    public partial class aggiungiEsercizio : Window
    {
        private readonly string videosFolder;
        private readonly List<esercizio> exercises = new();
        public string esercizioSelezionato = "";
        public aggiungiEsercizio()
        {
            InitializeComponent();
            videosFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "imgs\\img-esercizi");

            CaricaEsercizi();
            if (exercises.Count == 0)
            {
                LoadVideoList();
            }
            else
            {
                VideosListBox.DisplayMemberPath = "NomeEsercizio";
                VideosListBox.ItemsSource = exercises;
            }
        }

        private void LoadVideoList()
        {
            VideosListBox.DisplayMemberPath = null;
            if (!Directory.Exists(videosFolder))
            {
                Directory.CreateDirectory(videosFolder);
            }

            string[] extensions = new[] { ".mp4", ".wmv", ".avi", ".mov", ".mkv" };
            var files = Directory.EnumerateFiles(videosFolder)
                                 .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                                 .Select(Path.GetFileName)
                                 .ToList();

            VideosListBox.ItemsSource = files;
        }

        private void buttonIndietro_Click(object sender, RoutedEventArgs e)
        {

            this.Close();

        }

        private void VideosListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VideosListBox.SelectedItem == null) return;

            // se abbiamo caricato esercizi dal CSV
            if (exercises.Count > 0)
            {
                var selected = VideosListBox.SelectedItem as esercizio;
                if (selected == null) return;

                string path = selected.VideoPath ?? string.Empty;
                if (!string.IsNullOrEmpty(path))
                {
                    if (!Path.IsPathRooted(path))
                    {
                        path = Path.Combine(videosFolder, path);
                    }

                    if (File.Exists(path))
                    {
                        videoPlayer.Source = new Uri(path, UriKind.Absolute);
                        videoPlayer.Play();
                    }
                    else
                    {
                        MessageBox.Show("Video non trovato per l'esercizio selezionato.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Nessun video associato a questo esercizio.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                return;
            }

            // fallback: la ListBox contiene nomi di file video
            string fileName = VideosListBox.SelectedItem.ToString();
            string fullPath = Path.Combine(videosFolder, fileName);

            if (File.Exists(fullPath))
            {
                videoPlayer.Source = new Uri(fullPath, UriKind.Absolute);
                videoPlayer.Play();
            }
            else
            {
                MessageBox.Show($"File non trovato: {fullPath}", "Errore", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            esercizioSelezionato= fileName;

        }
        private void CaricaEsercizi()
        {
            exercises.Clear();

            if (!File.Exists("exercises.csv")) return;
            var info = new FileInfo("exercises.csv");
            if (info.Length == 0) return;

            try
            {
                var lines = File.ReadLines("exercises.csv")
                                .Select(l => l.Trim())
                                .Where(l => !string.IsNullOrWhiteSpace(l));

                foreach (var line in lines)
                {
                    var parts = line.Split(';').Select(p => p.Trim()).ToArray();
                    if (parts.Length < 6) continue;

                    string nome = parts[0];
                    if (!int.TryParse(parts[1], out int serie)) serie = 0;
                    if (!int.TryParse(parts[2], out int ripetizioni)) ripetizioni = 0;
                    if (!int.TryParse(parts[3], out int carico)) carico = 0;
                    if (!int.TryParse(parts[4], out int rpe)) rpe = 0;
                    string videoField = parts[5];

                    var ex = new esercizio(nome, serie, ripetizioni, carico, rpe);

                    string videoPath = string.Empty;
                    if (!string.IsNullOrWhiteSpace(videoField))
                    {
                        if (Path.IsPathRooted(videoField))
                        {
                            videoPath = videoField;
                        }
                        else
                        {
                            // conserva percorso relativo (risolto al momento della riproduzione)
                            videoPath = videoField;
                        }
                    }

                    ex.VideoPath = videoPath;
                    exercises.Add(ex);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante il caricamento degli esercizi: " + ex.Message, "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonReturn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;//serve per capire se finestra si è chiusa correttamente
            this.Close();
        }
    }
}

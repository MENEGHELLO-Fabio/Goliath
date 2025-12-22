using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Goliath
{
    public partial class Exercises : Window
    {
        private readonly string videosFolder;

        public Exercises()
        {
            InitializeComponent();
            // Cartella "Videos" nella directory dell'eseguibile
            videosFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "imgs\\img-esercizi");
            LoadVideoList();
        }

        // Carica i nomi dei file video nella ListBox
        private void LoadVideoList()
        {
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

        // Handler chiamato quando l'utente seleziona un elemento nella ListBox
        private void VideosListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VideosListBox.SelectedItem == null) return;

            string fileName = VideosListBox.SelectedItem.ToString();
            string fullPath = Path.Combine(videosFolder, fileName);

            if (File.Exists(fullPath))
            {
                // Imposta la Source e avvia la riproduzione
                videoPlayer.Source = new Uri(fullPath, UriKind.Absolute);
                videoPlayer.Play();
            }
            else
            {
                MessageBox.Show($"File non trovato: {fullPath}", "Errore", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Pulsanti Play / Pause / Stop
        private void Play_Click(object sender, RoutedEventArgs e) => videoPlayer.Play();
        private void Pause_Click(object sender, RoutedEventArgs e) => videoPlayer.Pause();
        private void Stop_Click(object sender, RoutedEventArgs e) => videoPlayer.Stop();
    }
}

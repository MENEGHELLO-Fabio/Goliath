using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Goliath
{
    /// <summary>
    /// Logica di interazione per aggiungiEsercizio.xaml
    /// </summary>
    public partial class aggiungiEsercizio : Window
    {
        videoCollection collezioneVideo = new videoCollection();
        eserciziCollection collezioneEsercizi;

        public esercizio esercizioSelezionato = null;

        public aggiungiEsercizio()
        {
            InitializeComponent();
            collezioneVideo.LoadVideos();
            collezioneEsercizi = new eserciziCollection(collezioneVideo);

            collezioneEsercizi.caricaDaCsv();

            if (collezioneEsercizi.getEsercizi().Count == 0)
            {
                collezioneEsercizi.compilaListaEsercizi();
            }

            VideosListBox.DisplayMemberPath = "NomeEsercizio";
            VideosListBox.ItemsSource = collezioneEsercizi.getEsercizi();
        }

        // Gestore della selezione nella ListBox: avvia il video dell'esercizio selezionato e memorizza la selezione
        private void VideosListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VideosListBox.SelectedItem == null) return;

            esercizio ex = (esercizio)VideosListBox.SelectedItem;
            string fullPath = ex.VideoPath;

            videoPlayer.Source = new Uri(fullPath);
            videoPlayer.Play();

            esercizioSelezionato = ex;
        }

        // Pulsante ritorno: chiude la finestra restituendo DialogResult = true
        private void buttonReturn_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true; // serve per capire se finestra si è chiusa correttamente
            this.Close();
        }

        // Evento chiamato quando il media finisce: riavvia la riproduzione (loop)
        private void videoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            videoPlayer.Position = TimeSpan.Zero;
            videoPlayer.Play();
        }
    }
}

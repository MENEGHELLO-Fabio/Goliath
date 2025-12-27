using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static System.Net.WebRequestMethods;

namespace Goliath
{
    /// <summary>
    /// Logica di interazione per aggiungiEsercizio.xaml
    /// </summary>
    public partial class aggiungiEsercizio : Window
    {
        videoCollection collezioneVideo= new videoCollection();
        eserciziCollection collezioneEsercizi;

        public string esercizioSelezionato = "";

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


      

        private void VideosListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            if (VideosListBox.SelectedItem == null)
            {
                return;
            }

            esercizio ex = (esercizio)VideosListBox.SelectedItem; 
            string fullPath = ex.VideoPath;

            videoPlayer.Source = new Uri(fullPath);
            videoPlayer.Play();

            esercizioSelezionato= ex.NomeEsercizio;

        }
        private void buttonReturn_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;//serve per capire se finestra si è chiusa correttamente
            this.Close();
        }
    }
}

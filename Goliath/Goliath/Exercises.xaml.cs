using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Goliath
{
    public partial class Exercises : Window
    {
        videoCollection collezioneVideo = new videoCollection();
        eserciziCollection collezioneEsercizi;

        public Exercises()
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

        }

        private void buttonHome_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (main != null)
            {
                main.Show();
                main.Activate();
            }
            else
            {
                main = new MainWindow();
                Application.Current.MainWindow = main;
                main.Show();
            }

            this.Close();
        }

        private void videoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            videoPlayer.Position = TimeSpan.Zero; 
            videoPlayer.Play();
        }
    }                                                           
}

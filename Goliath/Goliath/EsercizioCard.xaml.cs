using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Goliath
{
    /// <summary>
    /// Logica di interazione per EsercizioCard.xaml
    /// </summary>
    public partial class EsercizioCard : UserControl
    {
        // evento personalizzato per segnalare la selezione di una serie
        public event Action<EsercizioCard, serie> SerieSelezionata;

        List<serie> serieDiEsercizio = new List<serie>();
        public EsercizioCard(List<serie> serieDiEsercizio)
        {
            InitializeComponent();
            this.serieDiEsercizio = serieDiEsercizio;
            foreach (var item in serieDiEsercizio)
            {
                SerieList.Items.Add(item);
            }
        }

        // Evento chiamato quando il video della card finisce: riavvia in loop
        private void videoBox_MediaEnded(object sender, RoutedEventArgs e)
        {
            videoBox.Position = TimeSpan.Zero;
            videoBox.Play();
        }

        // Gestore selezione serie: aggiorna dettagli e segnala la selezione esternamente
        private void SerieList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedSerie = SerieList.SelectedItem as serie;
            if (selectedSerie != null)
            {
                repBlock.Text = selectedSerie.Ripetizioni.ToString();
                caricoBlock.Text = selectedSerie.Carico.ToString();
                // avviso la finestra principale della selezione
                SerieSelezionata?.Invoke(this, selectedSerie);
            }
        }
    }
}

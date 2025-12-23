using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Goliath
{
    /// <summary>
    /// Logica di interazione per CreaRoutine.xaml
    /// </summary>
    public partial class CreaRoutine : Window
    {

        //private readonly routine currentRoutine;
        public routine currentRoutine { get; }

        private string esercizioSelezionato = "";

        public CreaRoutine()
        {
            InitializeComponent();
            currentRoutine = new routine();
            // opzionale: mostra la lista degli esercizi correnti della routine nella UI (se presente)
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
          
        }

        private void ButtonAggiungiEsercizio_Click(object sender, RoutedEventArgs e)
        {
            if (nomeEsercizioBlock.Text=="" || serieBox.Text == "" || ripetizioniBox.Text == "" || caricoBox.Text == "") { 
                MessageBox.Show("Compila tutti i campi prima di aggiungere l'esercizio.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error); return; 
            }
            esercizio nuovoEsercizio = new esercizio(nomeEsercizioBlock.Text, int.Parse(serieBox.Text), int.Parse(ripetizioniBox.Text), int.Parse(caricoBox.Text), 0);
            nomeEsercizioBlock.Text="";
            serieBox.Text = "";
            ripetizioniBox.Text = "";
            caricoBox.Text = "";
            currentRoutine.AddEsercizio(nuovoEsercizio);
            eserciziPresenti.Items.Add(nuovoEsercizio.ToString());
        }

        private void buttonCercaEsercizio_Click(object sender, RoutedEventArgs e)
        {
            aggiungiEsercizio exercisesWindow = new aggiungiEsercizio();
            if (exercisesWindow.ShowDialog() == true)
            {
                esercizioSelezionato = exercisesWindow.esercizioSelezionato;
                string temp = "";
                for (int i = 0; i < esercizioSelezionato.Length; i++)
                {
                    if (esercizioSelezionato[i].Equals('.'))
                    {
                        break;
                    }
                    temp += esercizioSelezionato[i];
                }
                esercizioSelezionato = temp;
                nomeEsercizioBlock.Text = esercizioSelezionato;

            }
        }

        public void salvaRoutine()
        {
            // file destinazione
            const string filePath = "routines.csv";

            // assicurati che la routine abbia un nome
            if (string.IsNullOrWhiteSpace(currentRoutine.NomeRoutine))
            {
                currentRoutine.NomeRoutine = "UnnamedRoutine";
            }

            var lines = new List<string>();

            // header della routine (una sola riga)
            lines.Add($"ROUTINE;{currentRoutine.NomeRoutine}");

            // per ogni esercizio aggiungi una riga EX;Nome;Serie;Ripetizioni;Carico;RPE;VideoPath
            foreach (var ex in currentRoutine.GetEsercizi())
            {
                // proteggi dal carattere ';' nei campi sostituendolo con spazio
                string safeName = (ex.getNomeEsercizio() ?? "").Replace(";", " ");
                string safeVideo = "";
                // se la classe esercizio espone VideoPath, prova a leggerlo (se non presente rimane vuoto)
                try
                {
                    var videoProp = ex.GetType().GetProperty("VideoPath");
                    if (videoProp != null)
                    {
                        var val = videoProp.GetValue(ex) as string;
                        safeVideo = (val ?? "").Replace(";", " ");
                    }
                }
                catch
                {
                    safeVideo = "";
                }

                var line = $"EX;{safeName};{ex.getSerie()};{ex.getRipetizioni()};{ex.getCarico()};0;{safeVideo}";
                lines.Add(line);
            }

            // aggiungi una riga vuota come separatore (opzionale)
            lines.Add(string.Empty);

            // append delle linee: ogni elemento della lista diventa una riga separata nel file
            // File.AppendAllLines aggiunge automaticamente i terminatori di riga
            File.AppendAllLines(filePath, lines);
        }

        private void buttonSalvaRoutine_Click(object sender, RoutedEventArgs e)
        {
            if (nomeRoutineBox != null)
            {
                currentRoutine.NomeRoutine = nomeRoutineBox.Text;
            }
            this.salvaRoutine();
            this.DialogResult = true;//serve per capire se finestra si è chiusa correttamente
            this.Close();
        }

        
    }
}

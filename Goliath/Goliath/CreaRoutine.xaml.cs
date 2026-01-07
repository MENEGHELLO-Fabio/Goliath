using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Goliath
{
    /// <summary>
    /// Logica di interazione per CreaRoutine.xaml
    /// </summary>
    public partial class CreaRoutine : Window
    {
        // Oggetto routine corrente su cui si lavora
        public routine currentRoutine { get; }

        public esercizio esercizioSelezionato = null;

        // Percorso video selezionato (nascosto nella UI)
        private string videoPathSelezionato = "";

        public CreaRoutine()
        {
            InitializeComponent();
            currentRoutine = new routine();
           
        }

        // Chiude la finestra di creazione routine
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Aggiunge un esercizio creato manualmente alla routine corrente (recupera le serie dalla UI)
        private void ButtonAggiungiEsercizio_Click(object sender, RoutedEventArgs e)
        {
            if (nomeEsercizioBlock.Text == "" || SerieList == null)
            {
                MessageBox.Show("Compila tutti i campi prima di aggiungere l'esercizio.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error); return;
            }
            List<serie> serieEsercizio = new List<serie>();
            foreach (var item in SerieList.Items)
            {
                if (item is serie s)
                {
                    serieEsercizio.Add(s);
                }
            }
            esercizio nuovoEsercizio = new esercizio(nomeEsercizioBlock.Text, serieEsercizio);
            nuovoEsercizio.VideoPath = videoPathSelezionato;

            nomeEsercizioBlock.Text = "";
            SerieList.Items.Clear();
            ripetizioniBox.Text = "";
            caricoBox.Text = "";
            currentRoutine.AddEsercizio(nuovoEsercizio);
            eserciziPresenti.Items.Add(nuovoEsercizio.ToString());
        }

        // Apre la finestra di ricerca esercizi e popola i campi con l'esercizio selezionato
        private void buttonCercaEsercizio_Click(object sender, RoutedEventArgs e)
        {
            aggiungiEsercizio exercisesWindow = new aggiungiEsercizio();
            if (exercisesWindow.ShowDialog() == true)
            {
                esercizio ex = exercisesWindow.esercizioSelezionato;
                nomeEsercizioBlock.Text = ex.NomeEsercizio;
                
                videoPathSelezionato = ex.VideoPath;
            }
        }

        // Salva la routine corrente su file CSV (formato: header ROUTINE;[profile];NomeRoutine, poi EX/SERIE)
        public void salvaRoutine()
        {
            // file destinazione
            const string filePath = "routines.csv";

            if (string.IsNullOrWhiteSpace(currentRoutine.NomeRoutine))
            {
                currentRoutine.NomeRoutine = "UnnamedRoutine";
            }

            var lines = new List<string>();

            // header della routine: include username del profilo corrente se disponibile
            string currentProfile = ProfileHelper.GetCurrentProfileUsername();
            if (!string.IsNullOrWhiteSpace(currentProfile))
            {
                lines.Add($"ROUTINE;{currentProfile};{currentRoutine.NomeRoutine}");
            }
            else
            {
                // backward compatibility: legacy header without profile
                lines.Add($"ROUTINE;{currentRoutine.NomeRoutine}");
            }

            // per ogni esercizio aggiungi una riga EX;Nome;VideoPath
            foreach (var ex in currentRoutine.GetEsercizi())
            {
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

                // Riga esercizio
                lines.Add($"EX;{safeName};{safeVideo}");

                // Righe serie
                foreach (var s in ex.getSerie())
                {
                    lines.Add($"SERIE;{s.Ripetizioni};{s.Carico}");
                }

                // Riga vuota per separare
                lines.Add("");
            }

            File.AppendAllLines(filePath, lines);
        }

        // Handler per il pulsante Salva: imposta il nome e salva la routine su file
        private void buttonSalvaRoutine_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nomeRoutineBox.Text))
            {
                MessageBox.Show("Compila tutti i campi prima di salvare la routine");
                return;
            }

            // Imposto il nome della routine
            currentRoutine.NomeRoutine = nomeRoutineBox.Text.Trim();

            this.salvaRoutine();
            this.DialogResult = true;
            this.Close();
        }

        // Aggiunge una nuova serie alla lista di serie dell'esercizio in creazione
        private void ButtonAggiungiSerie_Click(object sender, RoutedEventArgs e)
        {
            if (ripetizioniBox.Text == "" || caricoBox.Text == "")
            {
                MessageBox.Show("Compila tutti i campi prima di aggiungere l'esercizio.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error); return;
            }
            serie nuovaSerie = new serie(int.Parse(ripetizioniBox.Text), int.Parse(caricoBox.Text));
            SerieList.Items.Add(nuovaSerie);
            ripetizioniBox.Text= "";    
            caricoBox.Text= "";
        }
    }
}
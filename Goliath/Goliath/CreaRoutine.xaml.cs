using System;
using System.Collections.Generic;
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
            if (nomeRoutineBox!=null)
            {
                currentRoutine.NomeRoutine = nomeRoutineBox.Text;
            }
            this.DialogResult = true;//serve per capire se finestra si è chiusa correttamente
            this.Close();
          
        }

        private void ButtonAggiungiEsercizio_Click(object sender, RoutedEventArgs e)
        {
            if (nomeEsecizioBlock.Text=="" || serieBox.Text == "" || ripetizioniBox.Text == "" || caricoBox.Text == "") { 
                MessageBox.Show("Compila tutti i campi prima di aggiungere l'esercizio.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error); return; 
            }
            esercizio nuovoEsercizio = new esercizio(nomeEsecizioBlock.Text, int.Parse(serieBox.Text), int.Parse(ripetizioniBox.Text), int.Parse(caricoBox.Text), 0);
            nomeEsecizioBlock.Text="";
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
                nomeEsecizioBlock.Text = esercizioSelezionato;

            }
        }
    }
}

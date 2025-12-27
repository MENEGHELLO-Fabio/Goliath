using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.DirectoryServices;
using System.Linq;

namespace Goliath
{
    /// <summary>
    /// Logica di interazione per Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        public Profile()
        {
            InitializeComponent();
            Carica();
        }

        private void buttonCreaProfilo_Click(object sender, RoutedEventArgs e)
        {
            
            if (!int.TryParse(textBoxNumeroAllenamenti.Text, out int numeroAllenamenti))
            {
                MessageBox.Show("Inserisci un numero di allenamenti valido.");
                return;
            }

            utente utente1 = new utente(textBoxNome.Text.Trim(), textBoxCognome.Text.Trim(), textBoxUsername.Text.Trim());
            utente1.setNumeroAllenamenti(numeroAllenamenti);

            
            string record = $"{textBoxNome.Text.Trim()};{textBoxCognome.Text.Trim()};{textBoxUsername.Text.Trim()};{numeroAllenamenti}";
            
            bool hasContent = File.Exists("profiles.csv") && new FileInfo("profiles.csv").Length > 0;
            File.AppendAllText("profiles.csv", (hasContent ? Environment.NewLine : "") + record);   //controllo se il file contiene già un profilo di modo da scrivere nella riga successiva

            textBoxNome.Clear();
            textBoxNumeroAllenamenti.Clear();
            textBoxCognome.Clear();
            textBoxUsername.Clear();

            Carica();
        }

        private void Carica()
        {
            if (!File.Exists("profiles.csv"))
            {
                return;
            }

            var info = new FileInfo("profiles.csv");
            if (info.Length == 0)
            {
                textBoxNome.Text = "";
                textBoxCognome.Text = "";
                textBoxUsername.Text = "";
                return;
            }

            
            string ultimaRiga;
            try
            {
                ultimaRiga = File.ReadLines("profiles.csv").Last();   //carico l'ultima riga del file csv
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante la lettura del file: " + ex.Message);
                return;
            }

            string[] campi = ultimaRiga.Split(';');
            if (campi.Length == 4)
            {


                textBoxNome.Text = campi[0];
                textBoxCognome.Text = campi[1];
                textBoxUsername.Text = campi[2];

                // controlla se campi[3] è un int
                if (int.TryParse(campi[3].Trim(), out int numeroAllenamenti))
                {
                    textBoxNumeroAllenamenti.Text = numeroAllenamenti.ToString();
                }
                else
                {
                    MessageBox.Show("Errore nel formato del numero degli allenamenti nell'ultima riga.");
                    textBoxNumeroAllenamenti.Text = string.Empty;
                }
            }
            else
            {
                MessageBox.Show("Formato del file CSV non valido (ultima riga).");
            }
        }

        private void buttonFormattaProfili_Click(object sender, RoutedEventArgs e)
        {
            File.Delete("profiles.csv");
            MessageBox.Show("Tutti i profili sono stati cancellati.");
            Carica();
        }

        private void buttonHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Close();
            main.ShowDialog();
        }
    }
}

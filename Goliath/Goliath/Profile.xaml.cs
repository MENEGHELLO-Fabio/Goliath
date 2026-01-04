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
            // wire up login button if present
            var accediBtn = this.FindName("buttonAccedi") as Button;
            if (accediBtn != null)
                accediBtn.Click += buttonAccedi_Click;

            Carica();
        }

        private string GetTextBoxText(string nameWithoutSuffix)
        {
            var tb = this.FindName(nameWithoutSuffix) as TextBox;
            if (tb != null) return tb.Text;
            // try with suffix '1'
            tb = this.FindName(nameWithoutSuffix + "1") as TextBox;
            return tb?.Text ?? string.Empty;
        }

        private void SetTextBoxText(string nameWithoutSuffix, string value)
        {
            var tb = this.FindName(nameWithoutSuffix) as TextBox;
            if (tb != null) { tb.Text = value; return; }
            tb = this.FindName(nameWithoutSuffix + "1") as TextBox;
            if (tb != null) tb.Text = value;
        }

        private PasswordBox GetPasswordBox()
        {
            var pb = this.FindName("passwordBoxPassword") as PasswordBox;
            if (pb != null) return pb;
            return this.FindName("passwordBoxPassword1") as PasswordBox;
        }

        private Button GetButton(string name)
        {
            var b = this.FindName(name) as Button;
            if (b != null) return b;
            return this.FindName(name + "1") as Button;
        }

        private bool ProfileExistsByUsername(string username)
        {
            if (!File.Exists("profiles.csv")) return false;
            try
            {
                foreach (var raw in File.ReadLines("profiles.csv"))
                {
                    if (string.IsNullOrWhiteSpace(raw)) continue;
                    var parts = raw.Split(';').Select(p => p.Trim()).ToArray();
                    if (parts.Length >= 3)
                    {
                        if (string.Equals(parts[2], username, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
            }
            catch { }
            return false;
        }

        private (string raw, string[] parts)? FindProfileByUsername(string username)
        {
            if (!File.Exists("profiles.csv")) return null;
            try
            {
                foreach (var raw in File.ReadLines("profiles.csv"))
                {
                    if (string.IsNullOrWhiteSpace(raw)) continue;
                    var parts = raw.Split(';').Select(p => p.Trim()).ToArray();
                    if (parts.Length >= 3)
                    {
                        if (string.Equals(parts[2], username, StringComparison.OrdinalIgnoreCase))
                            return (raw, parts);
                    }
                }
            }
            catch { }
            return null;
        }

        private void buttonCreaProfilo_Click(object sender, RoutedEventArgs e)
        {
            string nome = GetTextBoxText("textBoxNome");
            string cognome = GetTextBoxText("textBoxCognome");
            string username = GetTextBoxText("textBoxUsername");
            string numeroAllenamentiText = GetTextBoxText("textBoxNumeroAllenamenti");
            var pb = GetPasswordBox();
            string password = pb?.Password ?? string.Empty;

            if (!int.TryParse(numeroAllenamentiText, out int numeroAllenamenti))
            {
                MessageBox.Show("Inserisci un numero di allenamenti valido.");
                return;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Inserisci un username.");
                return;
            }

            // controllo se esiste già username
            if (ProfileExistsByUsername(username))
            {
                MessageBox.Show("Username già presente. Scegli un altro username.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // crea record con password (ultimo campo)
            string record = $"{nome.Trim()};{cognome.Trim()};{username.Trim()};{numeroAllenamenti};{password}";

            bool hasContent = File.Exists("profiles.csv") && new FileInfo("profiles.csv").Length > 0;
            File.AppendAllText("profiles.csv", (hasContent ? Environment.NewLine : "") + record);

            SetTextBoxText("textBoxNome", string.Empty);
            SetTextBoxText("textBoxNumeroAllenamenti", string.Empty);
            SetTextBoxText("textBoxCognome", string.Empty);
            SetTextBoxText("textBoxUsername", string.Empty);
            if (pb != null) pb.Password = string.Empty;

            Carica();

            MessageBox.Show("Profilo creato con successo!");
        }

        private void buttonAccedi_Click(object sender, RoutedEventArgs e)
        {
            string username = GetTextBoxText("textBoxUsername");
            var pb = GetPasswordBox();
            string password = pb?.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Inserisci l'username.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var found = FindProfileByUsername(username);
            if (found == null)
            {
                MessageBox.Show("Profilo non trovato.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var parts = found.Value.parts;
            // parts: nome;cognome;username;numeroAllenamenti;password (password optional if older lines)
            string storedPassword = parts.Length >= 5 ? parts[4] : string.Empty;

            if (!string.Equals(storedPassword, password, StringComparison.Ordinal))
            {
                MessageBox.Show("Password errata.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // login ok: append this profile as last line so Carica() will pick it as the active profile
            try
            {
                string raw = found.Value.raw;
                var lastLine = File.ReadLines("profiles.csv").LastOrDefault() ?? string.Empty;
                if (!string.Equals(lastLine, raw, StringComparison.Ordinal))
                {
                    File.AppendAllText("profiles.csv", Environment.NewLine + raw);
                }
            }
            catch { }

            Carica();
            MessageBox.Show("Accesso effettuato.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Carica()
        {
            // read last profile line and populate fields
            if (!File.Exists("profiles.csv"))
            {
                return;
            }

            var info = new FileInfo("profiles.csv");
            if (info.Length == 0)
            {
                SetTextBoxText("textBoxNome", string.Empty);
                SetTextBoxText("textBoxCognome", string.Empty);
                SetTextBoxText("textBoxUsername", string.Empty);
                SetTextBoxText("textBoxNumeroAllenamenti", string.Empty);
                var pb = GetPasswordBox(); if (pb != null) pb.Password = string.Empty;
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
            // accept both formats: name;cognome;username;numero OR name;cognome;username;numero;password
            if (campi.Length >= 4)
            {
                SetTextBoxText("textBoxNome", campi[0]);
                SetTextBoxText("textBoxCognome", campi[1]);
                SetTextBoxText("textBoxUsername", campi[2]);

                if (int.TryParse(campi[3].Trim(), out int numeroAllenamenti))
                {
                    SetTextBoxText("textBoxNumeroAllenamenti", numeroAllenamenti.ToString());
                }
                else
                {
                    MessageBox.Show("Errore nel formato del numero degli allenamenti nell'ultima riga.");
                    SetTextBoxText("textBoxNumeroAllenamenti", string.Empty);
                }

                var pb = GetPasswordBox();
                if (pb != null)
                {
                    pb.Password = campi.Length >= 5 ? campi[4] : string.Empty;
                }
            }
            else
            {
                MessageBox.Show("Formato del file CSV non valido (ultima riga).", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
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

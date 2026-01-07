using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Configuration.Internal;

namespace Goliath
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            caricaAllenamenti();
        }

        // Apre la finestra Routines e chiude la Main corrente
        private void tempButtonRoutines_Click(object sender, RoutedEventArgs e)
        {
            Routines routinesWindow = new Routines();
            this.Close();
            routinesWindow.ShowDialog();
        }

        // Apre la finestra Exercises e chiude la Main corrente
        private void tempButtonExercises_Click(object sender, RoutedEventArgs e)
        {
            Exercises routinesExercises = new Exercises();
            this.Close();
            routinesExercises.ShowDialog();
        }

        // Apre la finestra Profile e chiude la Main corrente
        private void tempButtonProfiles_Click(object sender, RoutedEventArgs e)
        {
            Profile profileWindow = new Profile();
            this.Close();
            profileWindow.ShowDialog();
        }

        // Apre la finestra per l'allenamento e chiude la Main corrente
        private void tempButtonAllenamento_Click(object sender, RoutedEventArgs e)
        {
            AllenamentoFinestra allenamentoWindow = new AllenamentoFinestra();
            this.Close();
            allenamentoWindow.ShowDialog();
        }

        // Legge il file allenamenti.csv e popola l'interfaccia con le card degli allenamenti fatti
        private void caricaAllenamenti()
        {
            // leggo da csv
            const string filePath = "allenamenti.csv";
            if (!File.Exists(filePath))
            {
                // file inesistente 
                return;
            }

            panel.Children.Clear();

            routine routineCorrente = null;
            esercizio esercizioCorrente = null;
            List<routine> allenamentiFatti = new List<routine>();

            var righe = File.ReadLines(filePath);

            foreach (var riga in righe)
            {
                if (string.IsNullOrWhiteSpace(riga))
                    continue; // salta righe vuote

                var parti = riga.Split(';');

                if (parti[0] == "ALLENAMENTO")
                {
                    // Se c'era già una routine in costruzione, la salvo
                    if (routineCorrente != null)
                    {
                        allenamentiFatti.Add(routineCorrente);
                    }

                    string nomeRoutine = parti[1];
                    string data = parti[2];

                    routineCorrente = new routine();
                    routineCorrente.NomeRoutine = nomeRoutine;
                    routineCorrente.DataAllenamento = data;
                }
                else if (parti[0] == "EXE")
                {
                    // Crea un esercizio con videoPath e lo aggiunge alla routine corrente
                    string nome = parti[1];
                    string videoPath = parti[2];

                    esercizioCorrente = new esercizio(nome, new List<serie>());
                    esercizioCorrente.VideoPath = videoPath;
                    routineCorrente.AddEsercizio(esercizioCorrente);
                }
                else if (parti[0] == "SERIE")
                {
                    // Aggiunge una serie all'ultimo esercizio corrente
                    int rip = int.Parse(parti[1]);
                    int carico = int.Parse(parti[2]);
                    esercizioCorrente.addSerie(new serie(rip, carico));
                }
            }

            if (routineCorrente != null)
                allenamentiFatti.Add(routineCorrente);

            // carico allenamenti dall'ultimo al primo e creo le card UI
            for (int i = allenamentiFatti.Count - 1; i >= 0; i--)
            {
                var routineI = allenamentiFatti[i];

                // Titolo
                var cardTitolo = new cardDescrizioneAllenamento();
                cardTitolo.impostaTitolo(routineI.NomeRoutine);
                cardTitolo.impostaData(routineI.DataAllenamento);

                panel.Children.Add(cardTitolo);

                // esercizi
                var card = new cardAllenamento  (routineI);
                panel.Children.Add(card);
            }
        }
    }
}

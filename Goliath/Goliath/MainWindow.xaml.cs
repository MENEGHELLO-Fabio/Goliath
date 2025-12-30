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

        private void tempButtonRoutines_Click(object sender, RoutedEventArgs e)
        {
            Routines routinesWindow = new Routines();
            this.Close();
            routinesWindow.ShowDialog();
            
        }

        private void tempButtonExercises_Click(object sender, RoutedEventArgs e)
        {
            Exercises routinesExercises = new Exercises();
            this.Close();
            routinesExercises.ShowDialog();

        }

        private void tempButtonProfiles_Click(object sender, RoutedEventArgs e)
        {
            Profile profileWindow = new Profile();
            this.Close();
            profileWindow.ShowDialog();
            

        }
        private void tempButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            this.Close();
            settingsWindow.ShowDialog();
            

        }

        private void tempButtonAllenamento_Click(object sender, RoutedEventArgs e)
        {
            AllenamentoFinestra allenamentoWindow = new AllenamentoFinestra();

            this.Close();
            allenamentoWindow.ShowDialog();
            
        }

        private void caricaAllenamenti()
        {
            
            //leggo da csv
            const string filePath = "allenamenti.csv";
            if (!File.Exists(filePath))
            {
                // file inesistente 
                return;
            }

            panel.Children.Clear();

            routine routineCorrente = null; 
            List<routine> allenamentiFatti = new List<routine>();

            var righe = File.ReadAllLines(filePath);

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
                    string nome = parti[1];
                    int serie = int.Parse(parti[2]);
                    int ripetizioni = int.Parse(parti[3]); 
                    int carico = int.Parse(parti[4]);
                    int rpe = int.Parse(parti[5]);
                    string videoPath = parti[6];
                    esercizio ex = new esercizio(nome, serie, ripetizioni, carico, rpe);
                    ex.VideoPath = videoPath;
                    routineCorrente.AddEsercizio(ex);
                }



            }

            if (routineCorrente != null) 
                allenamentiFatti.Add(routineCorrente);


            //carico allenamenti fatti
            foreach (var routineI in allenamentiFatti)
            {

                // Titolo
                var cardTitolo = new cardDescrizioneAllenamento();
                cardTitolo.impostaTitolo(routineI.NomeRoutine);
                cardTitolo.impostaData(routineI.DataAllenamento);

                panel.Children.Add(cardTitolo);

                //  esercizi

                var card = new cardAllenamento(routineI);

                panel.Children.Add(card);
            }
        }
    }
}
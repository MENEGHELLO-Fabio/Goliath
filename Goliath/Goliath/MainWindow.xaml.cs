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
    }
}
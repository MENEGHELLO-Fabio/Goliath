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

        private void tempButtonHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow profileWindow = new MainWindow();
            profileWindow.ShowDialog();
        }

        private void tempButtonRoutines_Click(object sender, RoutedEventArgs e)
        {
            Routines routinesWindow = new Routines();
            routinesWindow.ShowDialog();
        }

        private void tempButtonExercises_Click(object sender, RoutedEventArgs e)
        {
            Exercises routinesExercises = new Exercises();
            routinesExercises.ShowDialog();
        }

        private void tempButtonProfiles_Click(object sender, RoutedEventArgs e)
        {
            Profile profileWindow = new Profile();
            profileWindow.ShowDialog();
        }
        private void tempButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
    }
}
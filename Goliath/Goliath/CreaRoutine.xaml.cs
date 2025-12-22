using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Goliath
{
    /// <summary>
    /// Logica di interazione per CreaRoutine.xaml
    /// </summary>
    public partial class CreaRoutine : Window
    {
        public CreaRoutine()
        {
            InitializeComponent();
        }

        private void ButtonAggiungiEsercizio_Click(object sender, RoutedEventArgs e)
        {
            Exercises exercisesWindow = new Exercises();
            exercisesWindow.ShowDialog();
        }
    }
}

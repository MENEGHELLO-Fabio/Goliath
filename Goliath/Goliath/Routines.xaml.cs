using System;
using System.Collections.Generic;
using System.Text;
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
    /// Logica di interazione per Routines.xaml
    /// </summary>
    public partial class Routines : Window
    {
        public Routines()
        {
            InitializeComponent();
        }

        private void ButtonAggiungiRoutine_Click(object sender, RoutedEventArgs e)
        {
            CreaRoutine creaRoutineWindow = new CreaRoutine();
            if (creaRoutineWindow.ShowDialog() == true)
            {
                routine r = creaRoutineWindow.currentRoutine;
                routinesList.Items.Add(r);
            }
        }

        private void buttonHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Close();
            main.ShowDialog();
        }
    }
}

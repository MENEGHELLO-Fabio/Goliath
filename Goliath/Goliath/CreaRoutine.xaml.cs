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
        private readonly routine currentRoutine;

        public CreaRoutine()
        {
            InitializeComponent();
            currentRoutine = new routine();
            // opzionale: mostra la lista degli esercizi correnti della routine nella UI (se presente)
        }

        private void ButtonAggiungiEsercizio_Click(object sender, RoutedEventArgs e)
        {
            aggiungiEsercizio exercisesWindow = new aggiungiEsercizio();
            exercisesWindow.ShowDialog();

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

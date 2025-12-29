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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Goliath
{
    /// <summary>
    /// Logica di interazione per cardDescrizioneAllenamento.xaml
    /// </summary>
    public partial class cardDescrizioneAllenamento : UserControl
    {
        public cardDescrizioneAllenamento()
        {
            InitializeComponent();
        }

        public void impostaTitolo(string titolo)
        {
            tipoRoutine.Text = titolo;
        }

        public void impostaData(string data)
        {
            dataAllenamento.Text = data;
        }
    }
}

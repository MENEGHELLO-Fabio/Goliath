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
    /// Logica di interazione per EsercizioCard.xaml
    /// </summary>
    public partial class EsercizioCard : UserControl
    {
        public EsercizioCard()
        {
            InitializeComponent();
        }

        private void videoBox_MediaEnded(object sender, RoutedEventArgs e)
        {
            videoBox.Position = TimeSpan.Zero;
            videoBox.Play();
        }
    }
}

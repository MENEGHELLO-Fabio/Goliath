using System.Configuration;
using System.Data;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;

namespace Goliath
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static MediaPlayer clickPlayer;
        private static MediaPlayer homePlayer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // RegisterClassHandler per intercettare tutti i clic sui pulsanti
            EventManager.RegisterClassHandler(typeof(Button), Button.ClickEvent, new RoutedEventHandler(OnAnyButtonClick));

            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;

                // Cerca il suono per i clic dei pulsanti
                var clickPath = Path.Combine(baseDir, "buttonClick.mp3");
                if (!File.Exists(clickPath))
                {
                    // also support sounds/buttonClick.mp3
                    var alt = Path.Combine(baseDir, "sounds", "buttonClick.mp3");
                    if (File.Exists(alt)) clickPath = alt;
                }

                if (File.Exists(clickPath))
                {
                    clickPlayer = new MediaPlayer();
                    clickPlayer.Open(new System.Uri(clickPath, System.UriKind.Absolute));
                    clickPlayer.Volume = 0.9;
                    // caricamento del suono in memoria
                }

                // Carica il suono per i pulsanti "home" o "indietro"
                var homePath = Path.Combine(baseDir, "sounds", "buttonHome.mp3");
                if (!File.Exists(homePath))
                {
                    var altHome = Path.Combine(baseDir, "buttonHome.mp3");
                    if (File.Exists(altHome)) homePath = altHome;
                }

                if (File.Exists(homePath))
                {
                    homePlayer = new MediaPlayer();
                    homePlayer.Open(new System.Uri(homePath, System.UriKind.Absolute));
                    homePlayer.Volume = 0.95;
                }

                // parte il suono di avvio se presente
                var startupPath = Path.Combine(baseDir, "sounds", "startupSound.mp3");
                if (!File.Exists(startupPath))
                {
                    // supporta anche startupSound.mp3 in app folder
                    var altStartup = Path.Combine(baseDir, "startupSound.mp3");
                    if (File.Exists(altStartup)) startupPath = altStartup;
                }

                if (File.Exists(startupPath))
                {
                    var startupPlayer = new MediaPlayer();
                    startupPlayer.Open(new System.Uri(startupPath, System.UriKind.Absolute));
                    startupPlayer.Volume = 0.9;
                    bool opened = false;
                    startupPlayer.MediaOpened += (s, ev) => { opened = true; startupPlayer.Play(); };
                    startupPlayer.MediaFailed += (s, ev) => { startupPlayer.Close(); };
                    startupPlayer.Open(new System.Uri(startupPath, System.UriKind.Absolute));

                    // timeout per concludere il suono se non parte entro 3 secondi
                    _ = System.Threading.Tasks.Task.Run(async () =>
                    {
                        await System.Threading.Tasks.Task.Delay(3000);
                        if (!opened)
                        {
                            Application.Current.Dispatcher.Invoke(() => startupPlayer.Close());
                        }
                    });
                }
            }
            catch
            {
                //gestione errore ignorata
            }
        }

        private static void OnAnyButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // check sender per comprendere se è un 
                // Se il mittente è un Button, controlla il suo nome

                if (sender is Button btn)
                {
                    var name = (btn.Name ?? string.Empty).Trim();
                    if (!string.IsNullOrEmpty(name))
                    {
                        // Controllo se il nome del pulsante contiene "home" o "indietro" 
                        if (name.IndexOf("home", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                            name.IndexOf("indietro", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            if (homePlayer != null)
                            {
                                homePlayer.Position = System.TimeSpan.Zero;
                                homePlayer.Play();
                                return;
                            }

                            // prova a riprodurre il file direttamente come fallback
                            var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                            var fallbackHome = System.IO.Path.Combine(baseDir, "buttonHome.mp3");
                            if (!System.IO.File.Exists(fallbackHome))
                                fallbackHome = System.IO.Path.Combine(baseDir, "sounds", "buttonHome.mp3");

                            if (System.IO.File.Exists(fallbackHome))
                            {
                                var tempHome = new MediaPlayer();
                                tempHome.Open(new System.Uri(fallbackHome, System.UriKind.Absolute));
                                tempHome.Volume = 0.95;
                                tempHome.Play();
                                tempHome.MediaEnded += (s, ev) => { tempHome.Close(); };
                                return;
                            }
                        }
                    }
                }

                if (clickPlayer != null)
                {
                    // Restarta il suono del clic
                    clickPlayer.Position = System.TimeSpan.Zero;
                    clickPlayer.Play();
                }
                else
                {
                    // fallback: prova a riprodurre il file direttamente
                    var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                    var path = System.IO.Path.Combine(baseDir, "buttonClick.mp3");
                    if (!System.IO.File.Exists(path))
                        path = System.IO.Path.Combine(baseDir, "sounds", "buttonClick.mp3");

                    if (System.IO.File.Exists(path))
                    {
                        var temp = new MediaPlayer();
                        temp.Open(new System.Uri(path, System.UriKind.Absolute));
                        temp.Volume = 0.9;
                        temp.Play();
                        // assicurarsi di chiudere il MediaPlayer dopo la riproduzione
                        temp.MediaEnded += (s, ev) => { temp.Close(); };
                    }
                }
            }
            catch
            {
                // ignora gli errori
            }
        }
    }
}

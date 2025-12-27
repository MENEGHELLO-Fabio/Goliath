using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Goliath
{
    public class videoCollection
    {
        //percorso della cartella dei video
        private string videoFolder;
        //elenco dei file video
        public string[] files;

        public videoCollection()
        {
            videoFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "imgs\\img-esercizi");
        }

        public void LoadVideos()
        {
            if (!Directory.Exists(videoFolder)) { 

               return; 

            }
            files = System.IO.Directory.GetFiles(videoFolder, "*.mp4");
        }

        public string getVideoFolder()
        {
            return videoFolder;
        }
    }
}

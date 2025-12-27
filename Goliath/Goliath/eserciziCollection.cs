using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goliath
{
    public class eserciziCollection
    {
        List<esercizio> esercizi;
        videoCollection collezioneVideo = new videoCollection();
        public eserciziCollection(videoCollection videos)
        {
            esercizi = new List<esercizio>();
            collezioneVideo = videos;
        }

        public void compilaListaEsercizi()
        {
            esercizi.Clear();

            collezioneVideo.LoadVideos();

            if (collezioneVideo.files.Length == 0)
            {
                return;
            }

            foreach (string file in collezioneVideo.files)
            {
                string nomeEsercizio = Path.GetFileNameWithoutExtension(file);
                esercizio nuovoEsercizio = new esercizio(nomeEsercizio, 0, 0, 0, 0);
                nuovoEsercizio.VideoPath = file;
                esercizi.Add(nuovoEsercizio);
            }

        }

        public void CreaCsvSeNonEsiste()
        {
            if (!File.Exists("exercises.csv"))
            {
                // crea un file vuoto con intestazione
                File.WriteAllText("exercises.csv", "Nome;Serie;Ripetizioni;Carico;RPE;VideoPath\n");
            }
        }


        public void caricaDaCsv()
        {
            esercizi.Clear();

            CreaCsvSeNonEsiste();

             var lines = File.ReadLines("exercises.csv");

            foreach (var line in lines.Skip(1))
            {
                var part = line.Split(';');
                if (part.Length < 6) continue;

                string nome = part[0];
                int.TryParse(part[1], out int serie);
                int.TryParse(part[2], out int ripetizioni);
                int.TryParse(part[3], out int carico);
                int.TryParse(part[4], out int rpe);
                string videoPath = part[5];

                esercizio ex = new esercizio(nome, serie, ripetizioni, carico, rpe);

                ex.VideoPath = videoPath;

                esercizi.Add(ex);

            }
        }
        public void SalvaSuCSV()
        {
            List<string> lines = new List<string>();

            foreach (var ex in esercizi)
            {
                string line = $"{ex.NomeEsercizio};{ex.Serie};{ex.Ripetizioni};{ex.Carico};{ex.RPE};{ex.VideoPath}";
                lines.Add(line);
            }

            File.WriteAllLines("exercises.csv", lines);
        }

        public List<esercizio> getEsercizi()
        {
             return esercizi;
        }
    }
}

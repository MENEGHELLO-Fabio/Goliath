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

            if (collezioneVideo.files == null || collezioneVideo.files.Length == 0)
            {
                return;
            }

            foreach (string file in collezioneVideo.files)
            {
                string nomeEsercizio = Path.GetFileNameWithoutExtension(file);
                esercizio nuovoEsercizio = new esercizio(nomeEsercizio, new List<serie>());
                nuovoEsercizio.VideoPath = file;
                esercizi.Add(nuovoEsercizio);
            }

        }

        public void CreaCsvSeNonEsiste()
        {
            if (!File.Exists("exercises.csv"))
            {
                // crea un file vuoto con intestazione
                File.WriteAllText("exercises.csv", "Nome;VideoPath\n");
            }
        }


        public void caricaDaCsv()
        {
            esercizi.Clear();

            CreaCsvSeNonEsiste();

             var lines = File.ReadLines("exercises.csv");
            esercizio current = null;


            foreach (var line in lines)
            {

                if (string.IsNullOrWhiteSpace(line)) continue;

                var part = line.Split(';');

                if (part[0] == "EX")
                {
                    string nome = part[1];
                    string video = part[2];

                    current = new esercizio(nome, new List<serie>());
                    current.VideoPath = video;

                    esercizi.Add(current);
                }
                else if (part[0] == "SERIE" && current != null)
                {
                    int rip = int.Parse(part[1]);
                    int car = int.Parse(part[2]);

                    current.addSerie(new serie(rip, car));
                }

            }
        }
        public void SalvaSuCSV()
        {
            List<string> lines = new List<string>();
      
            foreach (var ex in esercizi)
            {
                string safeName = ex.NomeEsercizio.Replace(";", " ");
                string safeVideo = (ex.VideoPath ?? "").Replace(";", " ");

                // Riga esercizio
                lines.Add($"EX;{safeName};{safeVideo}");

                // Righe serie
                foreach (var s in ex.getSerie())
                {
                    lines.Add($"SERIE;{s.Ripetizioni};{s.Carico}");
                }

                lines.Add(""); 
            }

            File.WriteAllLines("exercises.csv", lines);
        }

        public List<esercizio> getEsercizi()
        {
             return esercizi;
        }
    }
}

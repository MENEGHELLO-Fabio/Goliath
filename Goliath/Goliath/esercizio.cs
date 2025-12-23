using System;
using System.Collections.Generic;
using System.Text;

namespace Goliath
{
    public class esercizio
    {
       

        public string NomeEsercizio { get; }
        public int Serie { get; }
        public int Ripetizioni { get; }
        public int Carico { get; private set; }
        public int RPE { get; }
        public string VideoPath { get; set; }

        public esercizio(string nomeEsercizio, int serie, int ripetizioni, int carico, int RPE)
        {
            NomeEsercizio = nomeEsercizio;
            Serie = serie;
            Ripetizioni = ripetizioni;
            Carico = carico;
            this.RPE = RPE;
            VideoPath = string.Empty;
        }

        public string getNomeEsercizio()
        {
            return NomeEsercizio;
        }

        public int getSerie()
        {
            return Serie;
        }

        public int getRipetizioni()
        {
            return Ripetizioni;
        }

        public int getCarico()
        {
            return Carico;
        }

        public void setCarico(int carico)
        {
            this.Carico = carico;
        }

        public override string ToString()
        {
            return NomeEsercizio;
        }
    }
}

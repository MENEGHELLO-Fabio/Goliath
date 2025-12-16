using System;
using System.Collections.Generic;
using System.Text;

namespace Goliath
{
    class esercizio
    {
        private String nomeEsercizio;
        private int serie;
        private int ripetizioni;
        private int carico;
        private int RPE;

        public esercizio(String nomeEsercizio, int serie, int ripetizioni, int carico, int RPE)
        {
            this.nomeEsercizio = nomeEsercizio;
            this.serie = serie;
            this.ripetizioni = ripetizioni;
            this.carico = carico;
            this.RPE = RPE;
        }

        public String getNomeEsercizio()
        {
            return nomeEsercizio;
        }

        public int getSerie()
        {
            return serie;
        }

        public int getRipetizioni()
        {
            return ripetizioni;
        }

        public int getCarico()
        {
            return carico;
        }

        public void setCarico(int carico)
        {
            this.carico = carico;
        }

        
    }
}

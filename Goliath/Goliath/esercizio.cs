using System;
using System.Collections.Generic;
using System.Text;

namespace Goliath
{
    public class esercizio
    {
       

        public string NomeEsercizio { get; }

        private List<serie> Serie;

        public string VideoPath { get; set; }

        public esercizio(string nomeEsercizio, List<serie> s)
        {
            NomeEsercizio = nomeEsercizio;
            this.Serie = s;
            VideoPath = string.Empty;
        }
        
        public void addSerie(serie S)
        {
            if (S!=null)
            {
                Serie.Add(S);
            }
          
        }
        public string getNomeEsercizio()
        {
            return NomeEsercizio;
        }

        public List<serie> getSerie()
        {
            return Serie;
        }


        public serie getSeriePos(int pos)
        {
            return Serie[pos];
        }

        public override string ToString()
        {
            return NomeEsercizio;
        }
    }
}

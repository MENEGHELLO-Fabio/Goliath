using System;
using System.Collections.Generic;
using System.Text;

namespace Goliath
{
    class utente
    {
        private String nome;
        private String cognome;
        private String username;
        private int numeroAllenamenti;
        private List<routine> allenamenti;

        public utente(String nome, String cognome, String username)
        {
            this.nome = nome;
            this.cognome = cognome;
            this.username = username;
            this.numeroAllenamenti = 0;
            this.allenamenti = new List<routine>(numeroAllenamenti);
        }

        public void rimuoviAllenamento(routine allenamento)
        {
            allenamenti.Remove(allenamento);
            numeroAllenamenti--;
        }

        public void rimuoviAllenamentoPerIndice(int indiceAllenamento)
        {
            allenamenti.RemoveAt(indiceAllenamento);
            numeroAllenamenti--;
        }

        public void aggiungiAllenamento(routine allenamento)
        {
            allenamenti.Add(allenamento);
            numeroAllenamenti++;
        }

        public String getNome()
        {
            return nome;
        }

        public String getCognome()
        {
            return cognome;
        }

        public String getUsername()
        {
            return username;
        }

        public int getNumeroAllenamenti()
        {
            return numeroAllenamenti;
        }

        public List<routine> getAllenamenti()
        {
            return allenamenti;
        }
    }

}

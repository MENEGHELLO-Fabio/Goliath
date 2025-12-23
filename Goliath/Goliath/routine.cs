using System;
using System.Collections.Generic;
using System.Text;

namespace Goliath
{
    public  class routine
    {
        public int NumeroEsercizi => esercizi?.Count ?? 0;
        public string NomeRoutine { get; set; }
        private readonly List<esercizio> esercizi;

        public routine()
        {
            esercizi = new List<esercizio>();
            NomeRoutine = "Nuova Routine";
        }

        public IReadOnlyList<esercizio> GetEsercizi() => esercizi.AsReadOnly();

        public void AddEsercizio(esercizio ex)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));
            esercizi.Add(ex);
        }

        public void RemoveEsercizio(esercizio ex)
        {
            if (ex == null) return;
            esercizi.Remove(ex);
        }

        public override string? ToString()
        {
            return NomeRoutine;
        }
    }
}

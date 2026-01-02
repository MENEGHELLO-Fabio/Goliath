using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goliath
{
    public class serie
    {
        public int Ripetizioni { get; set; }
        public int Carico { get;  set; }

        public serie(int Ripetizioni, int Carico)
        {
            this.Ripetizioni= Ripetizioni;
            this.Carico= Carico;
        }


    }
}

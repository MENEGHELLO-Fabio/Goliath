using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goliath
{
    public class videoItem
    {
        public string Nome { get; set; }
        public string VideoPath { get; set; }
        public videoItem(string nome, string videoPath) { 
            Nome = nome; 
            VideoPath = videoPath; 
        }
        public override string ToString() { return Nome; }
    }
}


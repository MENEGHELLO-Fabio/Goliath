using System;
using System.IO;
using System.Linq;

namespace Goliath
{
    public static class ProfileHelper
    {

        //oggetto che ritorna il nome utente del profilo corrente 
        public static string GetCurrentProfileUsername()
        {
            try
            {
                const string file = "profiles.csv";
                if (!File.Exists(file)) return string.Empty;
                var info = new FileInfo(file);
                if (info.Length == 0) return string.Empty;
                var last = File.ReadLines(file).LastOrDefault();
                if (string.IsNullOrWhiteSpace(last)) return string.Empty;
                var parts = last.Split(';');
                if (parts.Length >= 3)
                {
                    return parts[2].Trim();
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}

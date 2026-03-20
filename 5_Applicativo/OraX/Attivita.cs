using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OraX
{
    public class Attivita
    {
        public string Titolo { get; set; }
        public DateTime Data { get; set; }
        public DateTime? DataFine { get; set; } //opzionale, se non c'è mette nullo
        public Color Colore { get; set; }
    }
}

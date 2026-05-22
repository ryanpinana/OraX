using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OraX.Models
{
    public class Calendario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nome { get; set; } = "";

        public string CreatoreUsername { get; set; } = "";

        public bool Condiviso { get; set; }

        public override string ToString()
        {
            return Nome;
        }

    }
}

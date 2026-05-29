using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OraX.Models
{
    public class CalendarioUtente
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int CalendarioId { get; set; }

        public string Username { get; set; } = "";
    }
}

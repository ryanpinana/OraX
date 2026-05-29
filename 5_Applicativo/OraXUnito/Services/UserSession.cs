using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OraX.Models;

namespace OraX.Services;

public static class UserSession
{
    public static User CurrentUser { get; set; }
}

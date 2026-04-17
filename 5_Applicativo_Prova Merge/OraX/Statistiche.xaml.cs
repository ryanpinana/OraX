using OraX.Services;
using OraX.Models;

namespace OraX;

public partial class Statistiche : ContentPage
{
    User user;

    public Statistiche()
    {
        InitializeComponent();

        user = UserSession.CurrentUser;
    }
}
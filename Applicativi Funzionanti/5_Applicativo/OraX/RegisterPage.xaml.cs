using OraX.Models;
using OraX.Services;

namespace OraX;

public partial class RegisterPage : ContentPage
{
    DatabaseService database;

    public RegisterPage(DatabaseService db)
    {
        InitializeComponent();
        database = db;
    }

    private bool passwordVisibile = false;
    private bool password2Visibile = false;

    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        passwordVisibile = !passwordVisibile;

        PasswordEntry.IsPassword = !passwordVisibile;

        if (passwordVisibile)
        {
            OcchioPasswordButton.Source = "occhio_aperto.png";
        }
        else
        {
            OcchioPasswordButton.Source = "occhio_chiuso.png";
        }
    }

    private void OnTogglePassword2Clicked(object sender, EventArgs e)
    {
        password2Visibile = !password2Visibile;

        PasswordEntry2.IsPassword = !password2Visibile;

        if (password2Visibile)
        {
            OcchioPasswordButton2.Source = "occhio_aperto.png";
        }
        else
        {
            OcchioPasswordButton2.Source = "occhio_chiuso.png";
        }
    }


    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NomeEntry.Text) ||
            string.IsNullOrWhiteSpace(CognomeEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(TelefonoEntry.Text) ||
            string.IsNullOrWhiteSpace(UsernameEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry2.Text))
        {
            await DisplayAlert("Errore", "Compila tutti i campi", "OK");
            return;
        }

        if (PasswordEntry.Text != PasswordEntry2.Text)
        {
            await DisplayAlert("Errore", "Le password non coincidono", "OK");
            return;
        }

        var userEsistente = await database.GetUserByUsername(UsernameEntry.Text);

        if (userEsistente != null)
        {
            await DisplayAlert("Errore", "Username già esistente", "OK");
            return;
        }

        User nuovoUser = new User
        {
            Nome = NomeEntry.Text,
            Cognome = CognomeEntry.Text,
            Email = EmailEntry.Text,
            Telefono = TelefonoEntry.Text,
            DataNascita = DataNascitaPicker.Date,
            Username = UsernameEntry.Text,
            PasswordHash = PasswordHelper.HashPassword(PasswordEntry.Text),
            DataRegistrazione = DateTime.Now
        };

        await database.RegistraUser(nuovoUser);

        await DisplayAlert("Successo", "Account creato", "OK");

        await Navigation.PopAsync();
    }
}
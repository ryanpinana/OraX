using OraX.Models;
using OraX.Services;
using Microsoft.Maui.Storage;

namespace OraX;

public partial class Modifiche : ContentPage
{
    User user;
    DatabaseService database;

    public Modifiche()
    {
        InitializeComponent();

        database = new DatabaseService();
        user = UserSession.CurrentUser;

        if (user != null)
        {
            NameEntry.Text = user.Nome;
            SurnameEntry.Text = user.Cognome;
            UsernameEntry.Text = user.Username;
            EmailEntry.Text = user.Email;
            PhoneEntry.Text = user.Telefono;

            if (!string.IsNullOrEmpty(user.FotoProfiloPath))
            {
                ImgProfilo.Source = user.FotoProfiloPath;
            }
        }
    }

    private async void OnChangePhotoClicked(object sender, EventArgs e)
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Scegli una foto",
            FileTypes = FilePickerFileType.Images
        });

        if (result == null)
            return;

        string localPath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);

        using (var stream = await result.OpenReadAsync())
        using (var newStream = File.OpenWrite(localPath))
        {
            await stream.CopyToAsync(newStream);
        }

        user.FotoProfiloPath = localPath;
        ImgProfilo.Source = localPath;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (user == null)
        {
            await DisplayAlert("Errore", "Utente non trovato", "OK");
            return;
        }

        user.Nome = NameEntry.Text;
        user.Cognome = SurnameEntry.Text;
        user.Email = EmailEntry.Text;
        user.Telefono = PhoneEntry.Text;

        await database.UpdateUser(user);

        await DisplayAlert("OK", "Salvato nel database", "OK");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadTheme();
    }

    void LoadTheme()
    {
        int index = Preferences.Get("AppTheme", 0);

        switch (index)
        {
            case 1: ThemeManager.SetDarkTheme(); break;
            case 2: ThemeManager.SetBlueTheme(); break;
            case 3: ThemeManager.SetPinkTheme(); break;
            case 4: ThemeManager.SetPurpleTheme(); break;
            case 5: ThemeManager.SetYellowTheme(); break;
            case 6: ThemeManager.SetRedTheme(); break;
            case 7: ThemeManager.SetBrownTheme(); break;
            default: ThemeManager.SetLightTheme(); break;
        }

        ApplyTheme();
    }

    void ApplyTheme()
    {
        this.BackgroundColor = ThemeManager.BackgroundColor;

        frameInfo.BackgroundColor = ThemeManager.FrameBackgroundColor;
        frameInfo.BorderColor = ThemeManager.FrameBorderColor;

        frameInizio.BackgroundColor = ThemeManager.FrameBackgroundColor;
        frameInizio.BorderColor = ThemeManager.FrameBorderColor;

        frameContatti.BackgroundColor = ThemeManager.FrameBackgroundColor;
        frameContatti.BorderColor = ThemeManager.FrameBorderColor;

        frameExtra.BackgroundColor = ThemeManager.FrameBackgroundColor;
        frameExtra.BorderColor = ThemeManager.FrameBorderColor;

        labelInfo.TextColor = ThemeManager.TextColor;
        labelContatti.TextColor = ThemeManager.TextColor;
        labelExtra.TextColor = ThemeManager.TextColor;
    }
}

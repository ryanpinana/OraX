using Microsoft.Maui.ApplicationModel;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace OraX;

public partial class Modifiche : ContentPage
{
	public Modifiche()
	{
		InitializeComponent();

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadTheme();
    }
    void LoadTheme()
    {
        string theme = Preferences.Get("AppTheme", "Default");

        switch (theme)
        {
            case "Dark":
                ThemeManager.SetDarkTheme();
                break;

            case "Blue":
                ThemeManager.SetBlueTheme();
                break;
            case "Pink":
                ThemeManager.SetPinkTheme();
                break;
            case "Purple":
                ThemeManager.SetPurpleTheme();
                break;
            case "Yellow":
                ThemeManager.SetYellowTheme();
                break;
            case "Red":
                ThemeManager.SetRedTheme();
                break;
            case "Brown":
                ThemeManager.SetBrownTheme();
                break;
            default:
                ThemeManager.SetDefaultTheme();
                break;
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
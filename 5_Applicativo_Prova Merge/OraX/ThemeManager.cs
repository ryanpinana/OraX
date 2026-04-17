using Microsoft.Maui.Graphics;

namespace OraX
{
    public static class ThemeManager
    {
        public static event Action ThemeChanged;
        public static Color BackgroundColor { get; private set; }
        public static Color TextColor { get; private set; }
        public static Color FrameBackgroundColor { get; private set; }
        public static Color FrameBorderColor { get; private set; }

        // Tema chiaro
        public static void SetLightTheme()
        {
            BackgroundColor = Colors.White;
            TextColor = Colors.Black;
            FrameBackgroundColor = Colors.White;
            FrameBorderColor = Color.FromArgb("#EEEEEE");
            ThemeChanged?.Invoke();
        }

        // Tema “default viola”
        public static void SetDefaultTheme()
        {
            BackgroundColor = Colors.White;
            TextColor = Color.FromArgb("#a31aff");
            FrameBackgroundColor = Color.FromArgb("#e0b3ff");
            FrameBorderColor = Color.FromArgb("#d1a3ff");
            ThemeChanged?.Invoke();
        }

        // Tema scuro ma leggibile
        public static void SetDarkTheme()
        {
            BackgroundColor = Color.FromArgb("#2E2E3A");       // grigio scuro/bluastro
            TextColor = Colors.White;
            FrameBackgroundColor = Color.FromArgb("#3A3A4E");  // frame leggermente più chiaro
            FrameBorderColor = Color.FromArgb("#50506A");
            ThemeChanged?.Invoke();
        }

        // Tema blu
        public static void SetBlueTheme()
        {
            BackgroundColor = Color.FromArgb("#cce6ff");
            TextColor = Color.FromArgb("#0077e6");
            FrameBackgroundColor = Color.FromArgb("#e6f2ff");
            FrameBorderColor = Color.FromArgb("#0077e6");
            ThemeChanged?.Invoke();
        }

        // Tema rosa
        public static void SetPinkTheme()
        {
            BackgroundColor = Color.FromArgb("#ffccff");
            TextColor = Color.FromArgb("#cc00cc");
            FrameBackgroundColor = Color.FromArgb("#ffe6ff");
            FrameBorderColor = Color.FromArgb("#ff99ff");
            ThemeChanged?.Invoke();
        }

        // Tema viola
        public static void SetPurpleTheme()
        {
            BackgroundColor = Color.FromArgb("#cc99ff");
            TextColor = Color.FromArgb("#9900cc");
            FrameBackgroundColor = Color.FromArgb("#f3e6ff");
            FrameBorderColor = Color.FromArgb("#d1b3ff");
            ThemeChanged?.Invoke();
        }
        // Tema giallo
        public static void SetYellowTheme()
        {
            BackgroundColor = Color.FromArgb("#ffffcc");
            TextColor = Color.FromArgb("#ff9900");
            FrameBackgroundColor = Color.FromArgb("#ffcc99");
            FrameBorderColor = Color.FromArgb("#ff9900");
            ThemeChanged?.Invoke();
        }
        // Tema rosso
        public static void SetRedTheme()
        {
            BackgroundColor = Color.FromArgb("#ffe6e6");
            TextColor = Color.FromArgb("#cc0000");
            FrameBackgroundColor = Color.FromArgb("#ffb3b3");
            FrameBorderColor = Color.FromArgb("#cc0000");
            ThemeChanged?.Invoke();
        }
        // Tema marrone
        public static void SetBrownTheme()
        {
            BackgroundColor = Color.FromArgb("#e6ccb3");
            TextColor = Color.FromArgb("#996433");
            FrameBackgroundColor = Color.FromArgb("#d9b18c");
            FrameBorderColor = Color.FromArgb("#996433");
            ThemeChanged?.Invoke();
        }
    }
}
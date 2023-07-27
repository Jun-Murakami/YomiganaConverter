using Avalonia;
using Avalonia.Controls;
using System.Text.Json;
using System.IO;
using System;

namespace YomiganaConverter.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            LoadWindowSizeAndPosition();

            this.Closing += (sender, e) => SaveWindowSizeAndPosition();
        }

        private void LoadWindowSizeAndPosition()
        {
            string appDataPath = GetAppDataDirectory();
            if (File.Exists(Path.Combine(appDataPath, "Config.json")))
            {
                var jsonString = File.ReadAllText(Path.Combine(appDataPath, "Config.json"));

                var jsonObject = JsonSerializer.Deserialize<WindowSizeAndPosition>(jsonString);

                this.Width = jsonObject!.Width;
                this.Height = jsonObject.Height;
                this.Position = new PixelPoint(jsonObject.X, jsonObject.Y);
            }
            else
            {
                var screen = Screens.Primary;
                var workingArea = screen!.WorkingArea;

                double dpiScaling = screen.Scaling!;
                this.Width = (workingArea.Height / 5) * 4 * dpiScaling;
                this.Height = (workingArea.Height / 5) * 3 * dpiScaling;

                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        private void SaveWindowSizeAndPosition()
        {
            var windowSizeAndPosition = new
            {
                Width = this.Width,
                Height = this.Height,
                X = this.Position.X,
                Y = this.Position.Y
            };

            var jsonString = JsonSerializer.Serialize(windowSizeAndPosition);

            string appDataPath = GetAppDataDirectory();

            File.WriteAllText(Path.Combine(appDataPath, "Config.json"), jsonString);
        }

        public class WindowSizeAndPosition
        {
            public double Width { get; set; }
            public double Height { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        private string GetAppDataDirectory()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "YomiganaConverter"
            );

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            return appDataPath;
        }
    }
}
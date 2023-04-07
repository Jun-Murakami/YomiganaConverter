using Avalonia;
using Avalonia.Controls;
using System.Text.Json;
using System.IO;

namespace YomiganaConverter.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Load window size and position from a JSON file
            LoadWindowSizeAndPosition();

            // Save window size and position to a JSON file when the window is closing
            this.Closing += (sender, e) => SaveWindowSizeAndPosition();
        }

        private void LoadWindowSizeAndPosition()
        {
            // Check if the JSON file exists
            if (File.Exists("Config.json"))
            {
                // Read the JSON file
                var jsonString = File.ReadAllText("Config.json");

                // Deserialize the JSON string into a WindowSizeAndPosition object
                var jsonObject = JsonSerializer.Deserialize<WindowSizeAndPosition>(jsonString);

                // Set the window size and position based on the deserialized object
                this.Width = jsonObject.Width;
                this.Height = jsonObject.Height;
                this.Position = new PixelPoint(jsonObject.X, jsonObject.Y);
            }
            else
            {
                // Set the default window size and position if the JSON file does not exist
                var screen = Screens.Primary; // Get the primary screen
                var workingArea = screen.WorkingArea; // Get the working area of the primary screen

                // Calculate the window size based on the active area and DPI scaling
                double dpiScaling = screen.PixelDensity;
                this.Width = (workingArea.Height / 5) * 4;
                this.Height = (workingArea.Height  / 5) * 3;

                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            }
        }

        private void SaveWindowSizeAndPosition()
        {
            // Create an anonymous object containing the window size and position
            var windowSizeAndPosition = new
            {
                Width = this.Width,
                Height = this.Height,
                X = this.Position.X,
                Y = this.Position.Y
            };

            // Serialize the object into a JSON string
            var jsonString = JsonSerializer.Serialize(windowSizeAndPosition);

            // Write the JSON string to a file
            File.WriteAllText("Config.json", jsonString);
        }

        public class WindowSizeAndPosition
        {
            public double Width { get; set; }
            public double Height { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

    }
}
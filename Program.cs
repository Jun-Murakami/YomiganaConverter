using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.Diagnostics;

namespace YomiganaConverter
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            // Other initialization code
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

            // Add the unhandled exception handler
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Handle the unhandled exception here
            Debug.WriteLine("Unhandled exception occurred: " + e.ExceptionObject);
        }

    }
}
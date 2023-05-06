using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia;
using Avalonia.VisualTree;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Styling;
using MessageBox.Avalonia.Models;
using YomiganaConverter.ViewModels;
using YomiganaConverter;

namespace YomiganaConverter.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {

            EnglishIsChecked = true;
            KatakanaIsChecked = true;
            YouonIsChecked = true;
            SokuonIsChecked = true;
            SpaceIsChecked = true;

            ButtonTextHawa = "[は]→わ";
            ButtonTextHeE = "[へ]→え";

            ClearTextCommand = ReactiveCommand.Create(ClearText);
            PasteFromClipboardCommand = ReactiveCommand.CreateFromTask(async () => { await PasteFromClipboard(); });
            CopyToClipboardCommand = ReactiveCommand.CreateFromTask(async () => { await CopyToClipboard(); });

            MainGoCommand = ReactiveCommand.CreateFromTask(async () => { await MainGo(); });

            RemoveLineBreaksCommand = ReactiveCommand.CreateFromTask(async () => { await RemoveLineBreaks(); });
            HawaWaConvertCommand = ReactiveCommand.CreateFromTask(async () => { await Convert("は", "わ"); });
            HeEConvertCommand = ReactiveCommand.CreateFromTask(async () => { await Convert("へ", "え"); });

        }

        public Window GetWindow()
        {
            return (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
                .MainWindow;
        }
        public ReactiveCommand<Unit, Unit> ShowMessageBoxCommand { get; }
        public ReactiveCommand<Unit, Unit> MainGoCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveLineBreaksCommand { get; }
        public ReactiveCommand<Unit, Unit> HawaWaConvertCommand { get; }
        public ReactiveCommand<Unit, Unit> HeEConvertCommand { get; }

        public ReactiveCommand<Unit, Unit> PasteFromClipboardCommand { get; }
        public ReactiveCommand<Unit, Unit> CopyToClipboardCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearTextCommand { get; }

        private string _buttonTextHawa;
        public string ButtonTextHawa
        {
            get => _buttonTextHawa;
            set => this.RaiseAndSetIfChanged(ref _buttonTextHawa, value);
        }

        private string _buttonTextHeE;
        public string ButtonTextHeE
        {
            get => _buttonTextHeE;
            set => this.RaiseAndSetIfChanged(ref _buttonTextHeE, value);
        }


        private async Task PasteFromClipboard()
        {
            EditorText1 = await ApplicationExtensions.GetTopLevel(Avalonia.Application.Current).Clipboard.GetTextAsync();
        }

        private async Task CopyToClipboard()
        {
            await ApplicationExtensions.GetTopLevel(Avalonia.Application.Current).Clipboard.SetTextAsync(EditorText2);
        }

        private void ClearText()
        {
            EditorText1 = string.Empty;
        }

        private string _editorText1;

        public string EditorText1
        {
            get => _editorText1;
            set => this.RaiseAndSetIfChanged(ref _editorText1, value);
        }
        private string _editorText2;

        public string EditorText2
        {
            get => _editorText2;
            set => this.RaiseAndSetIfChanged(ref _editorText2, value);
        }

        private bool _isChecked1;
        public bool EnglishIsChecked
        {
            get => _isChecked1;
            set => this.RaiseAndSetIfChanged(ref _isChecked1, value);
        }

        private bool _isChecked2;
        public bool KatakanaIsChecked
        {
            get => _isChecked2;
            set => this.RaiseAndSetIfChanged(ref _isChecked2, value);
        }

        private bool _isChecked3;
        public bool YouonIsChecked
        {
            get => _isChecked3;
            set => this.RaiseAndSetIfChanged(ref _isChecked3, value);
        }

        private bool _isChecked4;
        public bool SokuonIsChecked
        {
            get => _isChecked4;
            set => this.RaiseAndSetIfChanged(ref _isChecked4, value);
        }

        private bool _isChecked5;
        public bool SpaceIsChecked
        {
            get => _isChecked5;
            set => this.RaiseAndSetIfChanged(ref _isChecked5, value);
        }
    }

    public static class ApplicationExtensions
    {
        public static TopLevel? GetTopLevel(this Application app)
        {
            if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }
            if (app.ApplicationLifetime is ISingleViewApplicationLifetime viewApp)
            {
                var visualRoot = viewApp.MainView?.GetVisualRoot();
                return visualRoot as TopLevel;
            }
            return null;
        }
    }
}
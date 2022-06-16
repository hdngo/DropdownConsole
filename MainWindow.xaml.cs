using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using NHotkey;
using NHotkey.Wpf;

namespace DropdownConsole
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool visible;
        private enum TYPES { SUCCESS, INFO, WARNING, ERROR, OTHER }
        public class TextEntry
        {
            public string Text { get; set; }
            public Brush Color { get; set; }
        }

        private void Output(TYPES type, string text)
        {
            Brush color;
            BrushConverter converter = new BrushConverter();
            switch (type)
            {
                case TYPES.SUCCESS:
                    color = (Brush)converter.ConvertFrom("#5dde54");
                    break;
                case TYPES.INFO:
                    color = (Brush)converter.ConvertFrom("#5c9eab");
                    break;
                case TYPES.WARNING:
                    color = (Brush)converter.ConvertFrom("#fffc33");
                    break;
                case TYPES.ERROR:
                    color = (Brush)converter.ConvertFrom("#eb4034");
                    break;
                default:
                    color = (Brush)converter.ConvertFrom("#ffffff");
                    break;
            }

            OutputItems.Items.Add(new TextEntry() { Text = text, Color = color });
        }

        public MainWindow()
        {
            InitializeComponent();
            visible = true;
            Loaded += MainWindow_Loaded;
            TextInput.KeyDown += TextInput_KeyDown;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HotkeyManager.Current.AddOrReplace("Toggle", Key.F12, ModifierKeys.Control, Toggle);
            TextInput.Focus();
        }

        private void TextInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                Output(TYPES.OTHER, $"[{DateTime.Now.ToString("hh:mm:ss tt")}] {TextInput.Text}");
                Handle(TextInput.Text);
                TextInput.Text = "";
                OutputScroller.ScrollToEnd();
            }
        }

        private void Toggle(object sender, HotkeyEventArgs e)
        {
            if (visible)
            {
                Storyboard sb = MainGrid.FindResource("SlideOut") as Storyboard;
                sb.Begin();
            }
            else {
                Storyboard sb = MainGrid.FindResource("SlideIn") as Storyboard;
                sb.Begin();
                Activate();
                TextInput.Focus();
            }
            visible = !visible;
            e.Handled = true;
        }

        private void Handle(string input)
        {
            int i = input.IndexOf(" ");
            string command = i == -1 ? input : input.Substring(0, i);
            command = command.ToLower();
            string args = i == -1 ? "" : input.Substring(i + 1);

            switch (command)
            {
                case "success":
                    Output(TYPES.SUCCESS, "This is a success!");
                    break;
                case "error":
                    Output(TYPES.ERROR, "This is an error!");
                    break;
                case "info":
                    Output(TYPES.INFO, "This is an info!");
                    break;
                case "warning":
                    Output(TYPES.WARNING, "This is a warning!");
                    break;
                case "exit":
                case "quit":
                    Exit();
                    break;
                case "clear":
                    Clear();
                    break;
                default:
                    Output(TYPES.OTHER, $"This is '{command}', an unhandled command. Its argument(s) are '{args}'");
                    break;
            }
        }

        private void Exit() 
        {
            Application.Current.Shutdown();
        }

        private void Clear() 
        {
            OutputItems.Items.Clear();
        }
    }
}

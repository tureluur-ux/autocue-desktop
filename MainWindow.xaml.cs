using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AutoCue.Core;
using Microsoft.Win32;

namespace AutoCue;

public partial class MainWindow : Window
{
    private readonly UserSettingsStore _settingsStore = new();
    private readonly DispatcherTimer _scrollTimer;
    private bool _isLoadingSettings = true;
    private bool _isPlaying;
    private string? _scriptPath;

    public MainWindow()
    {
        InitializeComponent();

        _scrollTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _scrollTimer.Tick += ScrollTimer_Tick;

        try
        {
            var settings = _settingsStore.Load();
            SpeedSlider.Value = settings.ScrollSpeed;
            FontSizeSlider.Value = settings.FontSize;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or System.Text.Json.JsonException or InvalidDataException)
        {
            MessageBox.Show(
                $"AutoCue couldn't read your saved preferences, so the defaults will be used.\n\n{exception.Message}",
                "Preferences couldn't be loaded",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        _isLoadingSettings = false;
        UpdateSpeedLabel();
        UpdateFontSize();
    }

    private void OpenScript_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Open a script",
            Filter = "Text files (*.txt;*.md)|*.txt;*.md|All files (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) == true)
        {
            LoadScript(dialog.FileName);
        }
    }

    private void LoadScript(string path)
    {
        var extension = Path.GetExtension(path);
        if (!string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(extension, ".md", StringComparison.OrdinalIgnoreCase))
        {
            StatusText.Text = "Choose a .txt or .md file to open a script.";
            return;
        }

        try
        {
            ScriptText.Text = File.ReadAllText(path);
            _scriptPath = path;
            FileNameText.Text = Path.GetFileName(path);
            EmptyState.Visibility = Visibility.Collapsed;
            ScriptScrollViewer.Visibility = Visibility.Visible;
            ScriptHeader.Visibility = Visibility.Visible;
            ScriptScrollViewer.ScrollToVerticalOffset(0);
            SetPlaying(false);
            StatusText.Text = $"{new FileInfo(path).Length:N0} bytes · script ready";
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            StatusText.Text = "Couldn't open the script.";
            MessageBox.Show(
                $"AutoCue couldn't open this file.\n\n{exception.Message}",
                "Couldn't open script",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void PlayButton_Click(object sender, RoutedEventArgs e) => TogglePlayback();

    private void TogglePlayback()
    {
        if (string.IsNullOrEmpty(_scriptPath))
        {
            StatusText.Text = "Open a text file before starting the teleprompter.";
            return;
        }

        if (ScriptScrollViewer.ScrollableHeight <= 0)
        {
            StatusText.Text = "This script fits on one screen; increase the text size to scroll.";
            return;
        }

        SetPlaying(!_isPlaying);
    }

    private void SetPlaying(bool playing)
    {
        _isPlaying = playing;
        if (_isPlaying)
        {
            _scrollTimer.Start();
            PlayButton.Content = "Ⅱ  Pause";
            PlaybackStatusText.Text = "SCROLLING";
            PlaybackStatusText.Foreground = new SolidColorBrush(Color.FromRgb(71, 215, 172));
            StatusDot.Fill = new SolidColorBrush(Color.FromRgb(71, 215, 172));
        }
        else
        {
            _scrollTimer.Stop();
            PlayButton.Content = "▶  Start reading";
            PlaybackStatusText.Text = "PAUSED";
            PlaybackStatusText.Foreground = new SolidColorBrush(Color.FromRgb(149, 163, 186));
            StatusDot.Fill = new SolidColorBrush(Color.FromRgb(101, 115, 138));
        }
    }

    private void ScrollTimer_Tick(object? sender, EventArgs e)
    {
        var nextOffset = ScriptScrollViewer.VerticalOffset + SpeedSlider.Value * _scrollTimer.Interval.TotalSeconds;
        if (nextOffset >= ScriptScrollViewer.ScrollableHeight)
        {
            ScriptScrollViewer.ScrollToVerticalOffset(ScriptScrollViewer.ScrollableHeight);
            SetPlaying(false);
            StatusText.Text = "End of script";
            return;
        }

        ScriptScrollViewer.ScrollToVerticalOffset(nextOffset);
    }

    private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        UpdateSpeedLabel();
        SaveSettings();
    }

    private void FontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        UpdateFontSize();
        SaveSettings();
    }

    private void UpdateSpeedLabel()
    {
        if (SpeedValueText is not null)
        {
            SpeedValueText.Text = $"{SpeedSlider.Value:0} px/s";
        }
    }

    private void UpdateFontSize()
    {
        if (ScriptText is not null && FontSizeText is not null)
        {
            ScriptText.FontSize = FontSizeSlider.Value;
            ScriptText.LineHeight = FontSizeSlider.Value * 1.6;
            FontSizeText.Text = $"{FontSizeSlider.Value:0} px";
        }
    }

    private void SaveSettings()
    {
        if (_isLoadingSettings || SpeedSlider is null || FontSizeSlider is null)
        {
            return;
        }

        try
        {
            _settingsStore.Save(new UserSettings(SpeedSlider.Value, FontSizeSlider.Value));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            StatusText.Text = $"Couldn't save preferences: {exception.Message}";
        }
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) is string[] { Length: > 0 } paths)
        {
            LoadScript(paths[0]);
        }
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space && Keyboard.Modifiers == ModifierKeys.None)
        {
            TogglePlayback();
            e.Handled = true;
        }
    }

    private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _scrollTimer.Stop();
        SaveSettings();
    }
}

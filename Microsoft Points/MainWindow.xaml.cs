using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsInput;
using WindowsInput.Events;

namespace Microsoft_Points;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    //private readonly DispatcherTimer _dispatcherTimer;
    /// <summary>
    ///     Character to insert
    /// </summary>
    private string _characters = "";

    /// <summary>
    ///     Delay between each enter
    /// </summary>
    private int _delay = 200;

    /// <summary>
    ///     Initial delay
    /// </summary>
    private int _initDelay;

    /// <summary>
    ///     Letters to insert
    /// </summary>
    private int _letterCount = 34;
    //private DispatcherTimer _dispatcherTimer;
    private bool _isRunning = false;

    private volatile bool _stopThread = false;

    public MainWindow()
    {
        InitializeComponent();

        /*_dispatcherTimer = new DispatcherTimer();
        _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(16); // 60 frames per second
        _dispatcherTimer.Tick += Timer_Tick;
        _dispatcherTimer.Start();
        */
    }

    /*
     private void Timer_Tick(object? sender, EventArgs e)
    {
        using (var keyboard = WindowsInput.Capture.Global.KeyboardAsync())
        {
            keyboard.KeyEvent += Keyboard_KeyEvent;
        }
    }

    private void Keyboard_KeyEvent(object? sender, EventSourceEventArgs<KeyboardEvent> e)
    {
        if (e.Data?.KeyDown?.Key == WindowsInput.Events.KeyCode.Control)
        {
            _isRunning = false;
        }
    }*/
    private void DelayTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        var textBox = (sender as TextBox)!;
        if (e.Key != Key.Enter) return;
        try
        {
            if (Convert.ToInt32(textBox.Text) > 0) _delay = Convert.ToInt32(textBox.Text);
        }
        catch (Exception)
        {
            MessageBox.Show("Enter a number");
        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        if (_isRunning && StartButtonText.Text == "Start")
        {
            MessageBox.Show("Already running");
        }
        
        if (CharacterBox.IsEnabled)
            try
            {
                var temp = CharacterBox.Text;
                if (temp.Length != 1) throw new Exception();
                for (var i = 0; i < 34; i++) _characters = string.Concat(_characters, temp);
            }
            catch (Exception)
            {
                MessageBox.Show("Enter a single character");
            }

        if (DelayTextBox.Text != "")
            try
            {
                if (Convert.ToInt32(DelayTextBox.Text) > 0) _delay = Convert.ToInt32(DelayTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Enter a number");
            }
        else
            _delay = 500;

        if (InitDelayTextBox.Text != "")
            try
            {
                if (Convert.ToInt32(InitDelayTextBox?.Text) > 0) _initDelay = Convert.ToInt32(InitDelayTextBox?.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Enter a number");
            }
        else
            _initDelay = 5;

        if (Count.Text != "")
            try
            {
                if (Convert.ToInt32(Count?.Text) > 0) _letterCount = Convert.ToInt32(Count?.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Enter a number");
            }
        else
            _letterCount = 34;

        WaitingText.Visibility = Visibility.Visible;
        var arguments = new Arguments(_delay, _characters, _letterCount);
        if (StartButtonText.Text == "Start")
        {
            _stopThread = false;
            var thread = new Thread(PointsInput);
            StartButtonText.Text = "Stop";
            _isRunning = true;
            thread.Start(arguments);
        }
        else
        {
            _stopThread = true;
            StartButtonText.Text = "Start";
            // Add code to wait for the thread to stop, if desired
        }
    }

    /// <summary>
    /// Function that handles inserting the characters
    /// </summary>
    /// <param name="arg">Arguments for the thread <see cref="Arguments"/></param>
    private void PointsInput(object? arg)
    {
        if (arg is not Arguments arguments) return;
        var delay = arguments.Delay;
        var remainingTime = 0;
        var characters = arguments.Characters;
        var letterCount = arguments.LetterCount;
        var temp = true;
        var startSeconds = DateTime.Now.Second;
        var endSeconds = startSeconds + _initDelay;
        while (temp)
            if (endSeconds - startSeconds > 0)
            {
                if (_stopThread)
                {
                    Dispatcher.Invoke(() =>
                    {
                        StartButtonText.Text = "Start";
                    });
                    break;
                }
                remainingTime = endSeconds - startSeconds;
                var time = remainingTime;
                Dispatcher.BeginInvoke(() =>
                {
                    if (WaitingText.Visibility == Visibility.Visible)
                        WaitingText.Text = time.ToString(CultureInfo.CurrentCulture);
                });
                Thread.Sleep(1000);
                endSeconds--;
            }
            else
            {
                temp = false;
            }

        var random = new Random();
        for (var i = 0; i < letterCount; i++)
        {
            if (_stopThread)
            {
                Dispatcher.Invoke(() =>
                {
                    StartButtonText.Text = "Start";
                });
                break;
            }
            if (characters == "")

                Simulate.Events().Click((char)('a' + random.Next(0, 26))).Wait(100).Click(KeyCode.Enter).Wait(delay)
                    .Invoke();

            else

                Simulate.Events().Click(characters).Wait(100).Click(KeyCode.Enter).Wait(delay).Invoke();


            Thread.Sleep(delay);
            Simulate.Events().Click(ButtonCode.Left).WaitToPreventDoubleClicking().Invoke();
        }
        Dispatcher.Invoke(() =>
        {
            StartButtonText.Text = "Start";
        });
        _isRunning = false;
    }

    private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
    {
        var radioButton = sender as RadioButton;
        if (radioButton?.Name == "Char")
        {
            CharacterBox.IsEnabled = true;
        }
        else
        {
            CharacterBox.IsEnabled = false;
            CharacterBox.Text = "";
        }
    }

    private void CharacterBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        var textBox = (sender as TextBox)!;
        if (e.Key != Key.Enter) return;
        try
        {
            var temp = textBox.Text;
            if (temp.Length != 1) throw new Exception();
            for (var i = 0; i < 34; i++) _characters = string.Concat(_characters, temp);
        }
        catch (Exception)
        {
            MessageBox.Show("Enter a single character");
        }
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        RandomChar.IsChecked = true;
    }

    private void InitDelayTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        var initDelay = sender as TextBox;
        if (e.Key != Key.Enter) return;
        try
        {
            if (Convert.ToInt32(initDelay?.Text) > 0) _initDelay = Convert.ToInt32(initDelay?.Text);
        }
        catch (Exception)
        {
            MessageBox.Show("Enter a number");
        }
    }

    private void Count_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        var count = sender as TextBox;
        if (e.Key != Key.Enter) return;
        try
        {
            if (Convert.ToInt32(count?.Text) > 0) _letterCount = Convert.ToInt32(count?.Text);
        }
        catch (Exception)
        {
            MessageBox.Show("Enter a number");
        }
    }

    /// <summary>
    ///     Arguments for the thread
    /// </summary>
    private class Arguments
    {
        /// <summary>
        ///     <see cref="_characters" />
        /// </summary>
        public readonly string Characters;

        /// <summary>
        ///     <see cref="_delay" />
        /// </summary>
        public readonly int Delay;

        /// <summary>
        ///     <see cref="_letterCount" />
        /// </summary>
        public readonly int LetterCount;

        public Arguments(int delay, string characters, int letterCount)
        {
            Delay = delay;
            Characters = characters;
            LetterCount = letterCount;
        }
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft_Points;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private string _characters = "";
    private int _delay;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void DelayTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        var textBox = (sender as TextBox)!;
        if (e.Key != Key.Enter) return;
        try
        {
            _delay = Convert.ToInt32(textBox.Text);
        }
        catch (Exception exception)
        {
            MessageBox.Show("Enter a number");
        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
    }

    private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
    {
        var radioButton = sender as RadioButton;
        if (radioButton.Name == "Char")
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
        string temp;
        if (e.Key != Key.Enter) return;
        try
        {
            temp = textBox.Text;
            if (temp.Length != 1) throw new Exception();
            for (var i = 0; i < 34; i++) _characters = string.Concat(_characters, temp);
        }
        catch (Exception exception)
        {
            MessageBox.Show("Enter a single character");
        }
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        RandomChar.IsChecked = true;
    }
}
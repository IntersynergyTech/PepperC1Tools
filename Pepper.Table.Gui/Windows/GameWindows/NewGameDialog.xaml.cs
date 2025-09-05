using System.Windows;

namespace Pepper.Table.Gui.Windows;

public partial class NewGameDialog : Window
{
    public NewGameDialog()
    {
        InitializeComponent();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void StartGame_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}


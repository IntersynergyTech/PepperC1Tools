using System.Windows;
using Pepper.Table.Core;
using Pepper.Table.Core.Helpers;

namespace Pepper.Table.Gui;

public partial class TableWindow : Window
{
    private readonly TableState _state;

    public TableWindow(TableState tableState)
    {
        InitializeComponent();

        _state = tableState;
        this.DataContext = tableState;
        
        _state.CardMovementDetected += CardMovementDetected;
    }

    private void CardMovementDetected(object? sender, CardMovement e)
    {
        Console.WriteLine($"CardMovementDetected: {e.Card} {e.Type} to {e.NewPosition.Description} from {e.PreviousPosition?.Description} at {e.Timestamp}");
    }

    private void StartHand_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            _state.StartHand();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);

            MessageBox.Show(exception.Message);
        }
    }

    private void EndHand_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            _state.EndHand();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);

            MessageBox.Show(exception.Message);
        }
    }
}

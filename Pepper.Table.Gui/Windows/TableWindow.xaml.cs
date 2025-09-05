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
    }

    private void CardMovementDetected(object? sender, CardMovement e)
    {
        Console.WriteLine($"CardMovementDetected: {e.Card} {e.Type} to {e.NewPosition.Description} from {e.PreviousPosition?.Description} at {e.Timestamp}");
    }
}

using System.ComponentModel;
using Pepper.Cards.Data.DbModels;
using Pepper.Table.Core;

namespace Pepper.Table.Gui.Windows;

class GameListContext : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    public ICollection<Game> Games { get; set; }
}

class NewGameContext : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    public IEnumerable<GameType> GameTypes { get; set; }
    public string NewGameNotes { get; set; }
    public GameType? SelectedGameType { get; set; }
}

public class GameManagerContext : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    public Game Game { get; set; }
    public GameHand? CurrentHand { get; set; }
    public int? HandNumber { get; set; }
    public TimeSpan AverageTimePerHand { get; set; }
    public TableState TableState { get; set; }
    public bool GameRunning { get; set; } = true;
}

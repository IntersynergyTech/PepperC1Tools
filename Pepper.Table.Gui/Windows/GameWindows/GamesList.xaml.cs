using System.Windows;
using Pepper.Cards.Data.DbModels;
using Pepper.Table.Core;

namespace Pepper.Table.Gui.Windows;

public partial class GamesList : Window
{
    public GamesList()
    {
        InitializeComponent();
    }

    private void NewGame_Click(object sender, RoutedEventArgs e)
    {
        var newGameWindow = new NewGameDialog();
        var newGameContext = new NewGameContext();

        newGameContext.GameTypes = App.CardsDbContext.GameTypes.OrderBy(x => x.Name).ToList();

        newGameWindow.DataContext = newGameContext;
        var result = newGameWindow.ShowDialog();

        if (result is true)
        {
            var game = new Game()
            {
                Type = newGameContext.SelectedGameType,
                Notes = newGameContext.NewGameNotes,
                StartedUtc = DateTime.UtcNow,
            };

            App.CardsDbContext.Games.Add(game);
            App.CardsDbContext.SaveChanges();

            var gameManagerContext = new GameManagerContext()
            {
                Game = game,
                TableState = new TableState(
                    App.CardsDbContext.TablePositions.ToList(),
                    App.CardsDbContext.Cards.ToList(),
                    MainWindow.Reader
                ),
                HandNumber = null
            };

            var gameManagerWindow = new GameManager(gameManagerContext);
            gameManagerWindow.ShowDialog();
        }
    }
}

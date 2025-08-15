using Pepper.Cards.Data.Models;
using Pepper.Cli.Windows.Database;
using Terminal.Gui.App;
using Terminal.Gui.Views;

namespace Pepper.Cli.Windows;

public class MainMenu : Window
{
    public MenuBar menuBar { get; set; }

    public MainMenu()
    {
        Title = "Pepper C# Cli";

        menuBar = new MenuBar();

        Add(menuBar);

        // Raw reader:
        var readerLabel = new Label(){
            X = 3,
            Y = 2,
            Text = "Raw Reader"
        };
        Add(readerLabel);
        var readerButton = new Button {
            X = 4,
            Y = 3,
            Text = "Open Raw Reader"
        }; 
        Add(readerButton);

        var databaseLabel = new Label{
            X = 3,
            Y = 5,
            Text = "Database"
        };
        Add(databaseLabel);

        // Database:
        var cardsDatabaseButton = new Button
        {
            X = 4,
            Y = 6,
            Text = "View Cards Database"
        };

        cardsDatabaseButton.Accepting += (_, args) =>
        {
            args.Handled = true;
            // Show database UI
            var dbWindow = new DatabaseTableList<Card>();
            Application.Run(dbWindow);
        };

        // Show Deck styles UI
        var deckStylesDatabaseButton = new Button
        {
            X = 4,
            Y = 7,
            Text = "View Deck Styles Database"
        };

        deckStylesDatabaseButton.Accepting += (_, args) =>
        {
            args.Handled = true;
            // Show deck styles UI
            var deckStylesWindow = new DatabaseTableList<DeckStyle>();
            Application.Run(deckStylesWindow);
        };

        Add(cardsDatabaseButton, deckStylesDatabaseButton);

        // Show Cards UI
    }
}

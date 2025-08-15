using System.Data;
using Pepper.Cards.Data.Models;
using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Pepper.Cli.Windows.Database;

public class DatabaseTableList<TType> : Window where TType : class
{
    public MenuBar WindowMenuBar { get; set; }
    public TableView MainTable { get; set; }

    public DatabaseTableList()
    {
        WindowMenuBar = new MenuBar();
        Title = "Database Table List - " + typeof(TType).Name;

        var dbset = Program.CardsDbContext.Set<TType>();

        if (typeof(TType) == typeof(DeckStyle))
        {
            DatabaseDeckStylesList();
        }
        else if (typeof(TType) == typeof(Card))
        {
            DatabaseCardsList();
        }
        else
        {
            throw new NotSupportedException($"The type {typeof(TType).Name} is not supported in this context.");
        }

        MainTable.Width = Dim.Fill();
        MainTable.Height = Dim.Fill();
        Add(MainTable);

        Add(WindowMenuBar);
    }

    public void DatabaseDeckStylesList()
    {
        var dataTable = new EnumerableTableSource<DeckStyle>(
            Program.CardsDbContext.DeckStyles,
            new Dictionary<string, Func<DeckStyle, object>>
            {
                { nameof(DeckStyle.Id), ds => ds.Id },
                { nameof(DeckStyle.Name), ds => ds.Name },
                { nameof(DeckStyle.IsFourColour), ds => ds.IsFourColour },
                { nameof(DeckStyle.DarkMode), ds => ds.DarkMode },
                { nameof(DeckStyle.LargePrint), ds => ds.LargePrint }
            }
        );

        void NewDeckStyleButtonPressed(object? sender, CommandEventArgs args)
        {
            var newDeckDialog = new AddDeckStyleDialog();
            Application.Run(newDeckDialog);
        }

        var newDeckStyleButton = new MenuBarItemv2("New");
        newDeckStyleButton.Accepting += NewDeckStyleButtonPressed;
        WindowMenuBar.Add(newDeckStyleButton);

        MainTable = new TableView() { Table = dataTable };
    }

    public void DatabaseCardsList()
    {
        var dataTable = new EnumerableTableSource<Card>(
            Program.CardsDbContext.Cards,
            new Dictionary<string, Func<Card, object>>
            {
                { nameof(Card.Id), c => c.Id },
                { nameof(Card.TagUid), c => BitConverter.ToString(c.TagUid) },
                { nameof(Card.Value), c => c.Value },
                { nameof(Card.Suit), c => c.Suit },
                { nameof(Card.DeckStyle), c => c.DeckStyle?.Name ?? "No Deck Style" }
            }
        );

        void NewCardButtonPressed(object? sender, CommandEventArgs args)
        {
            var addCardWindow = new AddCardWindow();
            Application.Run(addCardWindow);
        }

        var newCardButton = new MenuBarItemv2("New");
        newCardButton.Accepting += NewCardButtonPressed;
        WindowMenuBar.Add(newCardButton);

        MainTable = new TableView() { Table = dataTable };
    }
}

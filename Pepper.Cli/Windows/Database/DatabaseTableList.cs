using System.Data;
using Microsoft.EntityFrameworkCore;
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

    MenuBarItemv2 NewButton { get; set; }
    MenuBarItemv2 DeleteButton { get; set; }

    DbSet<TType> DbSet { get; set; }

    public DatabaseTableList()
    {
        WindowMenuBar = new MenuBar();
        WindowMenuBar.HotKey = Key.M;

        Title = "Database Table List - " + typeof(TType).Name;

        DbSet = Program.CardsDbContext.Set<TType>();

        MainTable = new TableView();
        MainTable.Width = Dim.Fill();
        MainTable.Height = Dim.Fill();
        MainTable.MultiSelect = false;
        MainTable.FullRowSelect = true;

        NewButton = new MenuBarItemv2("_New");
        NewButton.HotKey = Key.N;
        //NewButton.PopoverMenu = new PopoverMenu();

        DeleteButton = new MenuBarItemv2("_Delete");
        DeleteButton.Accepting += DeleteButtonAccepting;
        DeleteButton.X = Pos.Right(NewButton) + 1;

        WindowMenuBar.Add(NewButton);
        WindowMenuBar.Add(DeleteButton);

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

        Add(MainTable);

        Add(WindowMenuBar);
    }

    private void DeleteButtonAccepting(object? sender, CommandEventArgs e)
    {
        e.Handled = true;

        if (MainTable.SelectedRow <= 0)
        {
            MessageBox.ErrorQuery("No selection", "Please select a row to delete.", "OK");
            return;
        }

        var selectedRow = MainTable.SelectedRow;
        var dataTable = (EnumerableTableSource<TType>) MainTable.Table;

        var item = dataTable.GetObjectOnRow(selectedRow);

        if (item == null)
        {
            MessageBox.ErrorQuery("Error", "No item selected to delete.", "OK");
            return;
        }

        var confirmation = MessageBox.Query(
            "Confirm Deletion",
            $"Are you sure you want to delete the selected item: {item}?",
            "Yes",
            "No"
        );

        if (confirmation == 0) // User selected "No"
        {
            DbSet.Remove(item);
            Program.CardsDbContext.SaveChanges();
        }

        ReloadTable();
    }

    public void DatabaseDeckStylesList()
    {
        LoadDeckStyles();

        void NewDeckStyleButtonPressed(object? sender, CommandEventArgs args)
        {
            var newDeckDialog = new AddDeckStyleDialog();
            Application.Run(newDeckDialog);
        }

        NewButton.Accepting += NewDeckStyleButtonPressed;
    }

    private void ReloadTable()
    {
        if (typeof(TType) == typeof(DeckStyle))
        {
            LoadDeckStyles();
        }
        else if (typeof(TType) == typeof(Card))
        {
            LoadCards();
        }
    }

    private void LoadDeckStyles()
    {
        var deckStyles = Program.CardsDbContext.DeckStyles.ToList();

        MainTable.Table = new EnumerableTableSource<DeckStyle>(
            deckStyles,
            new Dictionary<string, Func<DeckStyle, object>>
            {
                { nameof(DeckStyle.Id), ds => ds.Id },
                { nameof(DeckStyle.Name), ds => ds.Name },
                { nameof(DeckStyle.IsFourColour), ds => ds.IsFourColour },
                { nameof(DeckStyle.DarkMode), ds => ds.DarkMode },
                { nameof(DeckStyle.LargePrint), ds => ds.LargePrint }
            }
        );
    }

    private void LoadCards()
    {
        var cards = Program.CardsDbContext.Cards.Include(c => c.DeckStyle).ToList();

        MainTable.Table = new EnumerableTableSource<Card>(
            cards,
            new Dictionary<string, Func<Card, object>>
            {
                { nameof(Card.Id), c => c.Id },
                { nameof(Card.TagUid), c => BitConverter.ToString(c.TagUid) },
                { nameof(Card.Value), c => c.Value },
                { nameof(Card.Suit), c => c.Suit },
                { nameof(Card.DeckStyle), c => c.DeckStyle?.Name ?? "No Deck Style" }
            }
        );
    }

    public void DatabaseCardsList()
    {
        LoadCards();

        void NewCardButtonPressed(object? sender, CommandEventArgs args)
        {
            var addCardWindow = new AddCardWindow();
            Application.Run(addCardWindow);
        }

        NewButton.Accepting += NewCardButtonPressed;
    }
}

using Pepper.Cards.Data.Models;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Pepper.Cli.Windows.Database;

public class AddDeckStyleDialog : Dialog
{
    private TextField nameField = new ();
    private CheckBox isFourColourCheckBox = new ();
    private CheckBox isDarkMode = new ();
    private CheckBox isLargePrint = new ();

    private Button SaveButton = new ();
    private Button CancelButton = new ();

    public AddDeckStyleDialog()
    {
        Title = "Add new deck style";
        Width = Dim.Auto();
        Height = Dim.Auto();
        
        nameField.Caption = "Name";
        nameField.Width = Dim.Fill();
        nameField.Y = 1;

        isFourColourCheckBox.Text = "Is Four Colour?";
        isFourColourCheckBox.Y = 2;
        
        isDarkMode.Text = "Dark Mode?";
        isDarkMode.Y = 3;

        isLargePrint.Text = "Large Print?";
        isLargePrint.Y = 4;

        Add(nameField);
        Add(isFourColourCheckBox);
        Add(isDarkMode);
        Add(isLargePrint);

        SaveButton.Text = "Save";
        SaveButton.Accepting += SaveButtonOnAccepting;

        CancelButton.Text = "Cancel";
        CancelButton.Accepting += (sender, args) => RequestStop();

        AddButton(SaveButton);
        AddButton(CancelButton);
    }

    private void SaveButtonOnAccepting(object? sender, CommandEventArgs e)
    {
        var newDeck = new DeckStyle
        {
            Name = nameField.Text,
            IsFourColour = isFourColourCheckBox.CheckedState == CheckState.Checked,
            DarkMode = isDarkMode.CheckedState == CheckState.Checked,
            LargePrint = isLargePrint.CheckedState == CheckState.Checked,
        };

        Program.CardsDbContext.DeckStyles.Add(newDeck);
        Program.CardsDbContext.SaveChanges();

        RequestStop();
    }
}

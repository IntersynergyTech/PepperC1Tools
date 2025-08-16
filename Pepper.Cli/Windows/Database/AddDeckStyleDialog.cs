using Pepper.Cards.Data.DbModels;
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
    private TextField backDesignKey = new ();

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
        isFourColourCheckBox.Y = 3;
        
        isDarkMode.Text = "Dark Mode?";
        isDarkMode.Y = 4;

        isLargePrint.Text = "Large Print?";
        isLargePrint.Y = 5;
        
        backDesignKey.Caption = "Back Design Key";
        backDesignKey.Width = Dim.Fill();
        backDesignKey.Y = 7;

        Add(nameField);
        Add(isFourColourCheckBox);
        Add(isDarkMode);
        Add(isLargePrint);
        Add(backDesignKey);

        SaveButton.Y = 9;
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
            BackDesignKey= backDesignKey.Text
        };

        Program.CardsDbContext.DeckStyles.Add(newDeck);
        Program.CardsDbContext.SaveChanges();

        RequestStop();
    }
}

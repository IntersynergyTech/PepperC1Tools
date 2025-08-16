using System.Collections.ObjectModel;
using Pepper.Cards.Data.DbModels;
using Pepper.Cards.Data.Enums;
using Pepper.Core.Data;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Pepper.Cli.Windows.Database;

public class AddCardWindow : Window
{
    private readonly Label _tagDisplayLabel;
    private readonly Label _resolvedCardLabel;
    private readonly ListView _deckList;
    private readonly ListView _suitList;
    private readonly ListView _valueList;
    private readonly Button _saveButton;

    public Tag? DetectedTag { get; set; }
    public Card? ResolvedCard { get; set; }
    public DeckStyle? SelectedDeckStyle { get; set; }

    public Card? NewCard { get; set; }

    private ObservableCollection<Suit> _suits = new ObservableCollection<Suit>();
    private ObservableCollection<CardValue> _values = new ObservableCollection<CardValue>();
    
    public AddCardWindow()
    {
        Program.TagDetected += TagDetected;

        Title = "Add Card";
        Width = Dim.Fill();
        Height = Dim.Fill();

        var tagLabel = new Label() { Text = "Detected Tag:", X = 1, Y = 1 };

        _tagDisplayLabel = new Label() { Text = "Scan a card.", X = Pos.AnchorEnd(), Y = Pos.Top(tagLabel) };

        var resolvedCardLabel = new Label() { Text = "Resolved Card:", X = 1, Y = Pos.Top(tagLabel) + 2 };

        _resolvedCardLabel = new Label()
        {
            Text = "Not Registered.", X = Pos.AnchorEnd(), Y = Pos.Top(resolvedCardLabel)
        };

        Add(tagLabel, _tagDisplayLabel, resolvedCardLabel, _resolvedCardLabel);

        var deckSelectLabel = new Label() { Text = "Deck Style:", X = 1, Y = Pos.Top(_resolvedCardLabel) + 2 };

        _deckList = new ListView()
        {
            X = 6,
            Y = Pos.Top(deckSelectLabel) + 1,
            Width = Dim.Fill(),
            Height = Dim.Absolute(5),
            BorderStyle = LineStyle.Single
        };

        _deckList.SetSource(new ObservableCollection<DeckStyle>(Program.CardsDbContext.DeckStyles));
        _deckList.SelectedItemChanged += DeckListOnSelectedItemChanged;

        var suitLabel = new Label() { Text = "Suit:", X = 1, Y = Pos.Bottom(_deckList) + 1 };

        _suitList = new ListView()
        {
            X = 6,
            Y = Pos.Top(suitLabel) + 1,
            Width = Dim.Fill(),
            Height = Dim.Absolute(6),
            BorderStyle = LineStyle.Single
        };

        _suits = new ObservableCollection<Suit>(Enum.GetValues<Suit>());
        _values = new ObservableCollection<CardValue>(Enum.GetValues<CardValue>());
        
        _suitList.SetSource(new ObservableCollection<Suit>(_suits));

        var valueLabel = new Label() { Text = "Value:", X = 1, Y = Pos.Bottom(_suitList) + 1 };

        _valueList = new ListView()
        {
            X = 6,
            Y = Pos.Top(valueLabel) + 1,
            Width = Dim.Fill(),
            Height = Dim.Absolute(15),
            BorderStyle = LineStyle.Single
        };

        _valueList.SetSource(new ObservableCollection<CardValue>(_values));

        _saveButton = new Button()
        {
            HotKey = Key.S,
            X = Pos.Center(),
            Y = Pos.Bottom(_valueList) + 2,
            Text = "Save Card",
            Width = Dim.Percent(80)
        };

        _saveButton.Accepting += SaveButtonOnAccepting;

        Add(
            deckSelectLabel,
            _deckList,
            suitLabel,
            _suitList,
            valueLabel,
            _valueList,
            _saveButton
        );
        
        Closing += (sender, args) =>
        {
            Program.TagDetected -= TagDetected;
        };
    }

    private void SaveButtonOnAccepting(object? sender, CommandEventArgs e)
    {
        if (DetectedTag == null)
        {
            MessageBox.ErrorQuery("Error", "No tag detected. Please scan a card first.", "OK");
            return;
        }

        if (SelectedDeckStyle == null)
        {
            MessageBox.ErrorQuery("Error", "No deck style selected. Please select a deck style.", "OK");
            return;
        }

        if (ResolvedCard != null)
        {
            MessageBox.ErrorQuery("Error", "This card is already registered.", "OK");
            return;
        }
        
        var suit = (Suit) _suits[_suitList.SelectedItem];
        var value = (CardValue) _values[_valueList.SelectedItem];

        var newCard = new Card
        {
            TagUid = DetectedTag.TagId,
            Value = value,
            Suit = suit,
            DeckStyle = SelectedDeckStyle
        };

        Program.CardsDbContext.Cards.Add(newCard);
        Program.CardsDbContext.SaveChanges();

        ResolveTag();
    }

    private void DeckListOnSelectedItemChanged(object? sender, ListViewItemEventArgs e)
    {
        SelectedDeckStyle = (DeckStyle?) e.Value;
    }

    private void TagDetected(object? sender, Tag e)
    {
        DetectedTag = e;
        _tagDisplayLabel.Text = e.ToString();
        _valueList.SetFocus();

        ResolveTag();
    }

    private void ResolveTag()
    {
        if (DetectedTag == null)
        {
            _resolvedCardLabel.Text = "No tag detected.";
            return;
        }

        var lookup = Program.CardsDbContext.Cards.SingleOrDefault(c => c.TagUid == DetectedTag.TagId);
        ResolvedCard = lookup;

        if (ResolvedCard != null)
        {
            _resolvedCardLabel.Text = ResolvedCard.ToString()!;
            return;
        }
        else
        {
            _resolvedCardLabel.Text = "Not Registered.";
        }
    }
}

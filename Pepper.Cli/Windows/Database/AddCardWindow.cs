using System.Collections.ObjectModel;
using Pepper.Cards.Data.Enums;
using Pepper.Cards.Data.Models;
using Pepper.Core.Data;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Pepper.Cli.Windows.Database;

public class AddCardWindow : Window
{
    private readonly Label _tagDisplayLabel;
    private readonly Label _resolvedCardLabel;
    private readonly ListView _deckList;
    private readonly RadioGroup _suitRadioGroup;
    private readonly RadioGroup _valueRadioGroup;

    public Tag? DetectedTag { get; set; }
    public Card? ResolvedCard { get; set; }
    public DeckStyle? SelectedDeckStyle { get; set; }

    public Card? NewCard { get; set; }

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
            Height = Dim.Absolute(3),
            BorderStyle = LineStyle.Single
        };

        _deckList.SetSource(new ObservableCollection<DeckStyle>(Program.CardsDbContext.DeckStyles));
        _deckList.SelectedItemChanged += DeckListOnSelectedItemChanged;

        var suitLabel = new Label() { Text = "Suit:", X = 1, Y = Pos.Bottom(_deckList) + 1 };

        _suitRadioGroup = new RadioGroup()
        {
            X = 6,
            Y = Pos.Top(suitLabel) + 1,
            Width = Dim.Fill(),
            RadioLabels = Enum.GetValues<Suit>().Select(s => s.ToString()).ToArray(),
            BorderStyle = LineStyle.Single
        };

        var valueLabel = new Label() { Text = "Value:", X = 1, Y = Pos.Bottom(_suitRadioGroup) + 1 };

        _valueRadioGroup = new RadioGroup()
        {
            X = 6,
            Y = Pos.Top(valueLabel) + 1,
            Width = Dim.Fill(),
            RadioLabels = Enum.GetValues<CardValue>().Select(v => v.ToString()).ToArray(),
            BorderStyle = LineStyle.Single
        };

        Add(
            deckSelectLabel,
            _deckList,
            suitLabel,
            _suitRadioGroup,
            valueLabel,
            _valueRadioGroup
        );
    }

    private void DeckListOnSelectedItemChanged(object? sender, ListViewItemEventArgs e)
    {
        SelectedDeckStyle = (DeckStyle?) e.Value;
    }

    private void TagDetected(object? sender, Tag e)
    {
        DetectedTag = e;
        _tagDisplayLabel.Text = e.ToString();
        _valueRadioGroup.SetFocus();
        _tagDisplayLabel.Draw();
        _tagDisplayLabel.DrawText();
        ResolveTag();
    }

    private void ResolveTag()
    {
        if (DetectedTag == null)
        {
            _resolvedCardLabel.Text = "No tag detected.";
            return;
        }

        // Here you would resolve the tag to a card in your database.
        // This is just a placeholder for demonstration purposes.
        _resolvedCardLabel.Text = $"Resolved Card: {DetectedTag.CardType} {DetectedTag.TagType}";
    }
}

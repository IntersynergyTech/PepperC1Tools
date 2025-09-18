using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Pepper.Cards.Data.DbModels;
using Pepper.Cards.Data.Enums;
using Pepper.Table.Core.Helpers;

namespace Pepper.Table.Gui.Windows;

public partial class GameManager : Window
{
    public GameManagerContext Context { get; }
    private HubConnection _hubConnection;

    public GameManager(GameManagerContext gameManagerContext)
    {
        InitializeComponent();
        Context = gameManagerContext;
        DataContext = Context;

        EndHandCommand.Command.CanExecute(false);
        DiscardHandCommand.Command.CanExecute(false);
        StartHandCommand.Command.CanExecute(true);
        EndGameCommand.Command.CanExecute(true);
        _hubConnection = new HubConnectionBuilder().WithUrl("https://localhost:7582/HandTrackingHub").WithAutomaticReconnect().Build();
        _hubConnection.StartAsync();
        _hubConnection.SendAsync("JoinTableGroup", Context.TableId);
        Context.TableState.CardMovementDetected += CardMovementDetected;
    }

    private void CardMovementDetected(object? sender, CardMovement e)
    {
        var handStep = new HandStep
        {
            CardId = e.Card.Id,
            Type = e.Type,
            ToPosition = e.NewPosition,
            FromPosition = e.PreviousPosition,
            Time = e.Timestamp
        };
        Context.CurrentHand?.Steps.Add(handStep);
        var handTrackingDto = new HandTrackingEventMessageDto
        {
            Rank = e.Card.Value,
            Suit = e.Card.Suit,
            TablePosition = e.NewPosition.Id,
            TableId = Context.TableId,
            TimestampUtc = DateTime.UtcNow,
            CardUid = e.Card.TagUid
        };
        try
        {
            _hubConnection.SendAsync("RecordEvent", handTrackingDto).Wait();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private void DiscardHand()
    {
        Context.TableState.EndHand();
        Context.CurrentHand = null;
    }
    
    private void StartHand()
    {
        Context.TableState.StartHand();

        var hand = new GameHand
        {
            StartedUtc = DateTime.UtcNow,
        };
        Context.CurrentHand = hand;

        StartHandCommand.Command.CanExecute(false);
        EndHandCommand.Command.CanExecute(true);
        DiscardHandCommand.Command.CanExecute(true);
    }

    private void EndHand()
    {
        Context.CurrentHand.EndedUtc = DateTime.UtcNow;
        
        Context.TableState.EndHand();
        Context.Game.Hands.Add(Context.CurrentHand!);
        Context.CurrentHand = null;
        EndHandCommand.Command.CanExecute(false);
        DiscardHandCommand.Command.CanExecute(false);
        StartHandCommand.Command.CanExecute(true);
    }

    private void EndGame()
    {
        Context.GameRunning = false;
        if (Context.CurrentHand != null)
        {
            Context.TableState.EndHand();
        }

        if (Context.Game.Hands.Count == 0)
        {
            MessageBox.Show("No hands were played. The game will be discarded.", "Game Discarded", MessageBoxButton.OK, MessageBoxImage.Information);
            App.CardsDbContext.Games.Remove(Context.Game);
        }

        App.CardsDbContext.SaveChanges();
    }
    
    private void WindowClosing(object? sender, CancelEventArgs e)
    {
        if (Context.GameRunning)
        {
            var result = MessageBox.Show(
                "A game is currently running. Are you sure you want to exit?",
                "Confirm Exit",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                EndGame();
            }
        }
    }
    
    
    
    private void StartHand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        StartHand();
    }
    
    private void EndHand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        EndHand();
    }
    private void DiscardHand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        DiscardHand();
    }
    private void EndGame_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        EndGame();
    }
    private void OpenLiveTable_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        var tableView = new TableWindow(Context.TableState);
        tableView.Show();
    }
}

public class HandTrackingEventMessageDto
{
    [Range(1, int.MaxValue)]
    public int TableId { get; set; }

    [Required]
    public Suit Suit { get; set; }

    [Required]
    public CardValue Rank { get; set; }

    [Range(1, int.MaxValue)]
    public int TablePosition { get; set; }

    public DateTime? TimestampUtc { get; set; }
    
    public byte[] CardUid { get; set; }
}
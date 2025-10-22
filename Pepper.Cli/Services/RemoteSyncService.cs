using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intersynergy.Poker.ApiClient;
using Microsoft.EntityFrameworkCore;
using Pepper.Cards.Data.Enums;
using Pepper.Cards.Database;

namespace Pepper.Cli.Services;

public class RemoteSyncService
{
    private readonly CardsDbContext _db;
    private readonly IntersynergyApiClient _client;

    public RemoteSyncService(CardsDbContext db, IntersynergyApiClient client)
    {
        _db = db;
        _client = client;
    }

    public async Task SyncAllAsync(CancellationToken ct = default)
    {
        try
        {
            await SyncDecksAsync(ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error syncing decks: {ex}");
        }

        try
        {
            await SyncCardsAsync(ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error syncing cards: {ex}");
        }
    }

    public async Task SyncDecksAsync(CancellationToken ct = default)
    {
        Console.WriteLine("Starting deck sync...");
        var decks = _db.DeckStyles.ToList();
        foreach (var deck in decks)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                if (deck.ApiDeckId != null)
                {
                    Console.WriteLine("Updating deck: " + deck.Name);
                    var dto = new UpdateDeckDto
                    {
                        DeckId = deck.ApiDeckId,
                        Name = deck.Name,
                        Description = deck.Name,
                        HasFourCornerLegend = deck.HasFourCornerLegend,
                        RemoteId = deck.Id.ToString()
                    };
                    var res = await _client.DeckPUTAsync(dto, ct);
                    Console.WriteLine("Deck synced: " + res.Name + " (remote id: " + res.Id + ")");
                }
                else
                {                
                    Console.WriteLine("Creating deck: " + deck.Name);
                    var dto = new CreateDeckDto
                    {
                        RemoteId = deck.Id.ToString(),
                        Name = deck.Name,
                        Description = deck.Name,
                        HasFourCornerLegend = deck.HasFourCornerLegend
                    };
                    var res = await _client.DeckPOSTAsync(dto, ct);
                    Console.WriteLine("Deck synced: " + res.Name + " (remote id: " + res.Id + ")");
                }


            }
            catch (ApiException e)
            {
                Console.WriteLine($"API error syncing deck '{deck.Name}': {e}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error syncing deck '{deck.Name}': {e}");
            }
        }
        Console.WriteLine("Deck sync complete.");
    }

    public async Task SyncCardsAsync(CancellationToken ct = default)
    {
        Console.WriteLine("Starting card sync...");
        var cards = _db.Cards.Include(x => x.DeckStyle).ToList();

        // Load all remote decks once
        var allDecks = (await _client.DeckAllAsync()).ToArray();
        
        foreach (var card in cards)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                if (card.Value == CardValue.JokerBlack || card.Value == CardValue.JokerRed)
                {
                    // Skip jokers for now
                    continue;
                }

                // Find the correct remote deck by the local DeckStyle Id
                var deck = allDecks.FirstOrDefault(x => x.RemoteId == card.DeckStyle.Id.ToString());
                if (deck == null)
                {
                    Console.WriteLine($"Remote deck not found for local deck '{card.DeckStyle.Name}' (DeckStyleId={card.DeckStyle.Id}), skipping card {card.Id}.");
                    continue;
                }

                var tagDto = new CardTagDto
                {
                    Rank = card.Value.ToRank(),
                    Suit = (Intersynergy.Poker.ApiClient.Suit)card.Suit,
                    TagUid = card.TagUid
                };

                var response = await _client.TagsAsync(deck.Id, new[] { tagDto });

                await using var trans = await _db.Database.BeginTransactionAsync(ct);
                card.RemoteDeckId = response.Id; // store the remote deck id for this card's deck association
                _db.Cards.Update(card);
                await _db.SaveChangesAsync(ct);
                await trans.CommitAsync(ct);

                Console.WriteLine("Synced card: " + card.Id + " to deck '" + deck.Name + "'");
            }
            catch (ApiException e)
            {
                Console.WriteLine($"API error syncing card {card.Id}: {e}");
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine($"Database error updating card {card.Id} after sync: {e}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error syncing card {card.Id}: {e}");
            }
        }
        Console.WriteLine("Card sync complete.");
    }
}

internal static class CardValueExtensions
{
    // Local helper to map CardValue to Rank via existing extension to avoid extra using in Program
    public static Rank ToRank(this CardValue value) => Pepper.Cli.Extensions.ToRank(value);
}

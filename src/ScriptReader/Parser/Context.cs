using System.Collections.Generic;

namespace MonoGwent;

public class Context
{
    public static BattleManager bm;

    public static string TriggerPlayer {get => bm.Current.name;}

    public static string OtherPlayer {get => bm.Rival.name;}

    public static List<Card> Hand {get => HandOfPlayer(TriggerPlayer);}
    public static List<Card> Deck {get => DeckOfPlayer(TriggerPlayer);}
    public static List<Card> Field {get => FieldOfPlayer(TriggerPlayer);}
    public static List<Card> Graveyard {get => GraveyardOfPlayer(TriggerPlayer);}

    public static List<Card> Board {
        get => [
            .. bm.Current.field,
            .. bm.Rival.field,
        ];
    }

    public static List<Card> HandOfPlayer(string player)
    {
        return bm.GetPlayerByName(player).hand;
    }

    public static List<Card> FieldOfPlayer(string player)
    {
        return bm.GetPlayerByName(player).field;
    }

    public static List<Card> GraveyardOfPlayer(string player)
    {
        return bm.GetPlayerByName(player).graveyard;
    }

    public static List<Card> DeckOfPlayer(string player)
    {
        return bm.GetPlayerByName(player).deck.Cards;
    }
}
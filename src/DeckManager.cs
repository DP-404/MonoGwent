using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGwent;

public class Deck : Stack<Card> {
    public CardLeader leader;
    public void Add(Card card) {
        Push(card);
    }
    public void Shuffle() {
        var shuffled = new Deck();
        foreach (var c in this.OrderBy(x=>Random.Shared.Next())) shuffled.Add(c);
        shuffled.leader = leader;
        Copy(shuffled);
    }

    public void Copy(Deck deck) {
        Clear();
        foreach (var i in deck) Add(i);
        leader = deck.leader;
    }
}

public interface IDeckGetter
{
    public static abstract Deck GetDeck();
}

public class CardBlueprint {

    public string image_name;

    public Texture2D image;

    public void LoadContent(GraphicTools gt) {
        image = gt.content.Load<Texture2D>(image_name);
    }

    public virtual Card GetCard() {
        throw new NotImplementedException();
    }
}

public class CardUnitBlueprint : CardBlueprint {
    public Type type;
    public RowType[] types;
    public int power;
    public bool is_hero = false;
    public int effect = 0;

    public override Card GetCard() {
        return new CardUnit {
            img=image,
            types=types,
            power=power,
            is_hero=is_hero,
            effect=effect
        };
    }

}

public class CardLeaderBlueprint : CardBlueprint {
    public LeaderEffect effect;

    public override Card GetCard() {
        return new CardLeader {
            img=image,
            effect=effect,
        };
    }

}

public struct CardsDump {

    public static CardLeaderBlueprint card_0L = new() {
        image_name="0L",
        effect=LeaderEffect.DRAW_EXTRA_CARD
    };
    public static CardUnitBlueprint card_01 = new() {
        type=typeof(CardUnit),
        image_name="01",
        types=[RowType.RANGE,RowType.SIEGE],
        power=3,
    };
    public static CardUnitBlueprint card_02 = new() {
        type=typeof(CardUnit),
        image_name="02",
        types=[RowType.MELEE,RowType.RANGE],
        power=3,
    };
    public static CardUnitBlueprint card_03 = new() {
        type=typeof(CardUnit),
        image_name="03",
        types=[RowType.MELEE],
        power=5,
    };

    public static void LoadContent(GraphicTools gt) {
        var blueprints = typeof(CardsDump)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f =>
                f.FieldType == typeof(CardUnitBlueprint) ||
                f.FieldType == typeof(CardLeaderBlueprint)
            )
            .ToDictionary(f => f.Name, f => (CardBlueprint) f.GetValue(null));
        foreach (var bp in blueprints.Values) {
            bp.LoadContent(gt);
        }
    }
}

public struct DecksDump {

    public class Deck1 : IDeckGetter {

        public static Deck GetDeck() {
            var deck = new Deck
            {
                CardsDump.card_01.GetCard(),
                CardsDump.card_01.GetCard(),
                CardsDump.card_01.GetCard(),
                CardsDump.card_01.GetCard(),
                CardsDump.card_02.GetCard(),
                CardsDump.card_02.GetCard(),
                CardsDump.card_02.GetCard(),
                CardsDump.card_02.GetCard(),
                CardsDump.card_03.GetCard(),
                CardsDump.card_03.GetCard(),
                CardsDump.card_03.GetCard(),
                CardsDump.card_03.GetCard(),
            };
            deck.leader = (CardLeader)CardsDump.card_0L.GetCard();
            return deck;
        }

    }

}

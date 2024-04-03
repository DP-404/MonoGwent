using System;
using System.Collections.Generic;

namespace MonoGwent;

public class Deck : List<Card>
{
    public void LoadContent(GraphicTools gt) {
        foreach (Card card in this) {
            card.LoadContent(gt);
        }
    }
}

public interface IDeckGetter
{
    public static abstract Deck GetDeck();
}

public class CardBlueprint {
    public Type type;
    public string img_name;
    public RowType[] types;
    public int power;
    public bool is_hero = false;
    public int effect = 0;

    public Card GetCard() {
        if (type == typeof(CardUnit)) {
            return new CardUnit {
                img_name=img_name,
                types=types,
                power=power,
                is_hero=is_hero,
                effect=effect
            };
        } else if (type == typeof(CardBait)) {
            return new CardBait {
                img_name=img_name,
                types=types,
            };
        } else {
            throw new Exception("Wrong card type in card generation.");
        }
    }
}

public struct CardsDump {

    public static CardBlueprint card_01 = new() {
        type=typeof(CardUnit),
        img_name="01",
        types=[RowType.RANGE,RowType.SIEGE],
        power=3,
    };
    public static CardBlueprint card_02 = new() {
        type=typeof(CardUnit),
        img_name="02",
        types=[RowType.MELEE,RowType.RANGE],
        power=3,
    };
    public static CardBlueprint card_03 = new() {
        type=typeof(CardUnit),
        img_name="03",
        types=[RowType.MELEE],
        power=5,
    };

}

struct DecksDump {

    public class Deck1 : IDeckGetter {

        public static Deck GetDeck() {
            return new Deck
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
        }

    }

}

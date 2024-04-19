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
    public RowType[] types = [];

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
    public int power;
    public bool is_hero = false;
    public int effect = 0;

    public override Card GetCard() {
        return new CardUnit {
            img_name=image_name,
            img=image,
            types=types,
            power=power,
            is_hero=is_hero,
            effect=effect
        };
    }

}

public class CardDecoyBlueprint : CardBlueprint {

    public override Card GetCard() {
        return new CardUnit {
            img_name=image_name,
            img=image,
            types=types,
            power=CardUnit.POWER_DECOY,
            is_hero=false,
            effect=0
        };
    }

}

public class CardLeaderBlueprint : CardBlueprint {
    public LeaderEffect effect;

    public override Card GetCard() {
        return new CardLeader {
            img_name=image_name,
            img=image,
            types=types,
            effect=effect,
        };
    }

}

public class CardWeatherBlueprint : CardBlueprint {
    public uint penalty = (uint)CardWeather.DEFAULT_PENALTY;

    public override Card GetCard() {
        return new CardWeather {
            img_name=image_name,
            img=image,
            types=types,
            penalty=(int)penalty
        };
    }

}

public class CardDispelBlueprint : CardBlueprint {

    public override Card GetCard() {
        return new CardWeather {
            img_name=image_name,
            img=image,
            types=types,
            penalty=CardWeather.DISPEL_PENALTY
        };
    }

}

public class CardBoostBlueprint : CardBlueprint {
    public int bonus;

    public override Card GetCard() {
        return new CardBoost {
            img_name=image_name,
            img=image,
            types=types,
            bonus=bonus
        };
    }

}

public struct CardsDump {

    public static CardLeaderBlueprint card_0L = new() {
        image_name="0L",
        effect=LeaderEffect.DRAW_EXTRA_CARD
    };
    public static CardUnitBlueprint card_0U1 = new() {
        type=typeof(CardUnit),
        image_name="0U1",
        types=[RowType.RANGE,RowType.SIEGE],
        power=3,
    };
    public static CardUnitBlueprint card_0U2 = new() {
        type=typeof(CardUnit),
        image_name="0U2",
        types=[RowType.MELEE,RowType.RANGE],
        power=3,
    };
    public static CardUnitBlueprint card_0U3 = new() {
        type=typeof(CardUnit),
        image_name="0U3",
        types=[RowType.MELEE],
        power=5,
    };
    public static CardUnitBlueprint card_0U4 = new() {
        type=typeof(CardUnit),
        image_name="0U4",
        types=[RowType.MELEE],
        power=8,
        is_hero=true
    };
    public static CardDecoyBlueprint card_0D1 = new() {
        image_name="0D1",
        types=[RowType.MELEE]
    };
    public static CardWeatherBlueprint card_0W1 = new() {
        image_name="0W1",
        types=[RowType.MELEE,RowType.RANGE]
    };
    public static CardDispelBlueprint card_0P1 = new() {
        image_name="0P1"
    };
    public static CardBoostBlueprint card_0B1 = new() {
        image_name="0B1",
        types=[RowType.MELEE],
        bonus=1
    };

    public static void LoadContent(GraphicTools gt) {
        var blueprints = typeof(CardsDump)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f =>
                f.FieldType == typeof(CardUnitBlueprint) ||
                f.FieldType == typeof(CardLeaderBlueprint) ||
                f.FieldType == typeof(CardWeatherBlueprint) ||
                f.FieldType == typeof(CardDispelBlueprint) ||
                f.FieldType == typeof(CardBoostBlueprint) ||
                f.FieldType == typeof(CardDecoyBlueprint)
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
                CardsDump.card_0U1.GetCard(),
                CardsDump.card_0U1.GetCard(),
                CardsDump.card_0U1.GetCard(),
                CardsDump.card_0U2.GetCard(),
                CardsDump.card_0U2.GetCard(),
                CardsDump.card_0U2.GetCard(),
                CardsDump.card_0U3.GetCard(),
                CardsDump.card_0U3.GetCard(),
                CardsDump.card_0U3.GetCard(),
                CardsDump.card_0U4.GetCard(),

                CardsDump.card_0W1.GetCard(),

                CardsDump.card_0P1.GetCard(),

                CardsDump.card_0B1.GetCard(),

                CardsDump.card_0D1.GetCard(),
            };
            deck.leader = (CardLeader)CardsDump.card_0L.GetCard();
            return deck;
        }

    }

}

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

    public string name = "";
    public string description = "";
    public string image_name;
    public RowType[] types = [];

    public Texture2D image;

    public void LoadContent(GameTools gt) {
        image = gt.content.Load<Texture2D>(image_name);
    }

    public virtual Card GetCard() {
        throw new NotImplementedException();
    }
}

public class CardUnitBlueprint : CardBlueprint {
    public int power;
    public bool is_hero = false;
    public int effect = 0;

    public override Card GetCard() {
        return new CardUnit {
            name=name,
            description=description,
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
            name=name,
            description=description,
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
            name=name,
            description=description,
            img_name=image_name,
            img=image,
            types=types,
            effect=effect,
        };
    }

}

public class CardWeatherBlueprint : CardBlueprint {
    public uint penalty = CardWeather.DEFAULT_PENALTY;

    public override Card GetCard() {
        return new CardWeather {
            name=name,
            description=description,
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
            name=name,
            description=description,
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
            name=name,
            description=description,
            img_name=image_name,
            img=image,
            types=types,
            bonus=bonus
        };
    }

}

public struct CardsDump {

    public static CardLeaderBlueprint card_0L = new() {
        name="The Pale King",
        description="No cost too great.",
        image_name="0L",
        effect=LeaderEffect.DRAW_EXTRA_CARD
    };
    public static CardUnitBlueprint card_0U1 = new() {
        name="The Knight",
        description="An enigmatic wanderer who descends into Hallownest carrying only a broken nail to fend off foes.",
        image_name="0U1",
        types=[RowType.MELEE,RowType.SIEGE],
        power=3,
    };
    public static CardUnitBlueprint card_0U2 = new() {
        name="Hornet",
        description="Skilled protector of Hallownest's ruins. Wields a needle and thread.",
        image_name="0U2",
        types=[RowType.MELEE,RowType.RANGE],
        power=3,
    };
    public static CardUnitBlueprint card_0U3 = new() {
        name="Mantis Lords",
        description="Leaders of the Mantis tribe and its finest warriors. They bear thin nail-lances and attack with blinding speed.",
        image_name="0U3",
        types=[RowType.MELEE,RowType.RANGE],
        power=4,
    };
    public static CardUnitBlueprint card_0U4 = new() {
        name="Hollow Knight",
        description="Fully grown Vessel, carrying the plague's heart within its body.",
        image_name="0U4",
        types=[RowType.MELEE],
        power=8,
        is_hero=true
    };
    public static CardDecoyBlueprint card_0D1 = new() {
        name="Zote",
        description="A self-proclaimed Knight, of no renown. Wields a nail he carved from shellwood, named 'Life Ender.'",
        image_name="0D1",
        types=[RowType.MELEE]
    };
    public static CardWeatherBlueprint card_0W1 = new() {
        name="City of Tears",
        description="The city looks to be built into an enormous cavern, and the rain pours down from cracks in the stone above. There must be a lot of water up there somewhere.",
        image_name="0W1",
        types=[RowType.MELEE,RowType.RANGE]
    };
    public static CardDispelBlueprint card_0P1 = new() {
        name="Lake of Unn",
        description="The greater mind once dreamed of leaf and cast these caverns so. In every bush and every vine the mind of Unn reveals itself to us.",
        image_name="0P1"
    };
    public static CardBoostBlueprint card_0B1 = new() {
        name="Grey Prince Zote",
        description="Figment of an obsessed mind. Lacks grace but becomes stronger with every defeat.",
        image_name="0B1",
        types=[RowType.MELEE],
        bonus=1
    };

    public static void LoadContent(GameTools gt) {
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

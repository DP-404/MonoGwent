using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;

using static MonoGwent.CardsDump;

namespace MonoGwent;

public class Deck {
    public string name;
    public string img_name;
    private Dictionary<CardBlueprint,int> card_blueprints;
    private CardLeaderBlueprint leader_blueprint;

    public CardLeader leader;
    private List<Card> cards = new();

    public List<Card> Cards {get => cards;}
    public int Count {get => cards.Count;}

    public Texture2D img;

    public Deck() {}
    public Deck(string name, string img_name, Dictionary<CardBlueprint,int> card_blueprints, CardLeaderBlueprint leader_blueprint) {
        this.name = name;
        this.img_name = img_name;
        this.card_blueprints = card_blueprints;
        this.leader_blueprint = leader_blueprint;
    }

    public void Populate() {
        cards = new();
        foreach (var c in card_blueprints) {
            for (int i = c.Value; i > 0; i--) {
                Add(c.Key.GetCard());
            }
        }
        leader = (CardLeader)leader_blueprint.GetCard();
    }
    public void Add(Card card) {
        cards.Add(card);
    }
    public Card Take() {
        var c = cards[^1];
        cards.Remove(c);
        return c;
    }
    public void Remove(Card card) {
        cards.Remove(card);
    }
    public void Shuffle() {
        var shuffled = new Deck();
        foreach (var c in cards.OrderBy(x=>Random.Shared.Next())) shuffled.Add(c);
        shuffled.leader = leader;
        Copy(shuffled);
    }

    public void Copy(Deck deck) {
        cards.Clear();
        foreach (var i in deck.cards) Add(i);
        leader = new() {
            name=deck.leader.name,
            description=deck.leader.description,
            img_name=deck.leader.img_name,
            img=deck.leader.img,
            types=deck.leader.types,
            effect=deck.leader.effect,
            used=deck.leader.used
        };
    }
    public Deck GetCopy() {
        var deck = new Deck();
        deck.Copy(this);
        return deck;
    }

    public void LoadContent(GameTools gt) {
        img = gt.content.Load<Texture2D>(img_name);
    }
}

public interface IDeckGetter
{
    public static abstract string GetName();
    public static abstract string GetImageName();
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
    public IEffect effect = new EffectNone();

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
            is_hero=false
        };
    }

}

public class CardLeaderBlueprint : CardBlueprint {
    public IEffect effect;

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

    public static CardLeaderBlueprint c0L = new() {
        name="The Pale King",
        description="No cost too great.",
        image_name="graphics/cards/0L",
        effect=new EffectDrawCard()
    };
    public static CardUnitBlueprint c0U1 = new() {
        name="The Knight",
        description="An enigmatic wanderer who descends into Hallownest carrying only a broken nail to fend off foes.",
        image_name="graphics/cards/0U1",
        types=[RowType.MELEE,RowType.SIEGE],
        power=3,
    };
    public static CardUnitBlueprint c0U2 = new() {
        name="Hornet",
        description="Skilled protector of Hallownest's ruins. Wields a needle and thread.",
        image_name="graphics/cards/0U2",
        types=[RowType.MELEE,RowType.RANGE],
        power=3,
    };
    public static CardUnitBlueprint c0U3 = new() {
        name="Mantis Lords",
        description="Leaders of the Mantis tribe and its finest warriors. They bear thin nail-lances and attack with blinding speed.",
        image_name="graphics/cards/0U3",
        types=[RowType.MELEE,RowType.RANGE],
        power=4,
    };
    public static CardUnitBlueprint c0U4 = new() {
        name="Hollow Knight",
        description="Fully grown Vessel, carrying the plague's heart within its body.",
        image_name="graphics/cards/0U4",
        types=[RowType.MELEE],
        power=7,
        is_hero=true,
        effect=new EffectSetWeather(RowType.MELEE)
    };
    public static CardUnitBlueprint c0U5 = new() {
        name="Radiance",
        description="The light, forgotten.",
        image_name="graphics/cards/0U5",
        types=[RowType.RANGE],
        power=8,
        is_hero=true
    };
    public static CardUnitBlueprint c0U6 = new() {
        name="Nightmare King Grimm",
        description="The expanse of dream in past was split,\nOne realm now must stay apart,\nDarkest reaches, beating red,\nTerror of sleep. The Nightmare's Heart.",
        image_name="graphics/cards/0U6",
        types=[RowType.SIEGE],
        power=9,
        is_hero=true,
        effect=new EffectSetBoost(RowType.SIEGE)
    };
    public static CardUnitBlueprint c0U7 = new() {
        name="Dung Defender",
        description="Skilled combatant living at the heart of the Waterways. Assails intruders with balls of compacted dung.",
        image_name="graphics/cards/0U7",
        types=[RowType.SIEGE],
        power=4
    };
    public static CardUnitBlueprint c0U8 = new() {
        name="White Defender",
        description="The Champion's Call, the Knotted Grove, the Battle of the Blackwyrm... I remember it all. I will carry those glories with me always... until we meet again.",
        image_name="graphics/cards/0U8",
        types=[RowType.SIEGE],
        power=5
    };
    public static CardDecoyBlueprint c0D1 = new() {
        name="Zote",
        description="A self-proclaimed Knight, of no renown. Wields a nail he carved from shellwood, named 'Life Ender.'",
        image_name="graphics/cards/0D1",
        types=[RowType.MELEE]
    };
    public static CardDecoyBlueprint c0D2 = new() {
        name="False Knight",
        description="Weak creatures love to steal the strength of others. Their lives are brief and fearful, and they yearn to have the power to dominate those who have dominated them.",
        image_name="graphics/cards/0D2",
        types=[RowType.SIEGE]
    };
    public static CardWeatherBlueprint c0W1 = new() {
        name="City of Tears",
        description="The city looks to be built into an enormous cavern, and the rain pours down from cracks in the stone above. There must be a lot of water up there somewhere.",
        image_name="graphics/cards/0W1",
        types=[RowType.RANGE,RowType.SIEGE]
    };
    public static CardWeatherBlueprint c0W2 = new() {
        name="Kingdom's Edge",
        description="This ashen place is grave of Wyrm. Once told, it came to die. But what is death for that ancient being? More transformation methinks. This failed kingdom is product of the being spawned from that event.",
        image_name="graphics/cards/0W2",
        types=[RowType.MELEE]
    };
    public static CardWeatherBlueprint c0W3 = new() {
        name="The Infection",
        description="The bugs of Hallownest were twisted out of shape by that ancient sickness. First they fell into deep slumber, then they awoke with broken minds, and then their bodies started to deform...",
        image_name="graphics/cards/0W3",
        types=[RowType.RANGE]
    };
    public static CardDispelBlueprint c0P1 = new() {
        name="Lake of Unn",
        description="The greater mind once dreamed of leaf and cast these caverns so. In every bush and every vine the mind of Unn reveals itself to us.",
        image_name="graphics/cards/0P1"
    };
    public static CardBoostBlueprint c0B1 = new() {
        name="Grey Prince Zote",
        description="Figment of an obsessed mind. Lacks grace but becomes stronger with every defeat.",
        image_name="graphics/cards/0B1",
        types=[RowType.MELEE],
        bonus=1
    };
    public static CardBoostBlueprint c0B2 = new() {
        name="The Grimm Troupe",
        description="Shadows dream of endless fire,\nFlames devour and embers swoop,\nOne will light the Nightmare Lantern,\nCall and serve in Grimm's dread Troupe.",
        image_name="graphics/cards/0B2",
        types=[RowType.SIEGE],
        bonus=1
    };
    public static CardBoostBlueprint c0B3 = new() {
        name="White Lady",
        description="These bindings about me, I've chosen to erect. There is some shame I feel from my own part in the deed, and this method guarantees it cease.",
        image_name="graphics/cards/0B3",
        types=[RowType.RANGE],
        bonus=1
    };


    public static CardLeaderBlueprint c1L = new() {
        name="Moon Lord",
        description="The mastermind behind all terrors which befall the world, freed from his lunar prison. Practically a god, his power knows no limits.",
        image_name="graphics/cards/1L",
        effect=new EffectLeaderWinMatch()
    };
    public static CardUnitBlueprint c1U1 = new() {
        name="Eye of Cthulhu",
        description="A piece of Cthulhu ripped from his body centuries ago in a bloody war. It wanders the night seeking its host body... and revenge!",
        image_name="graphics/cards/1U1",
        types=[RowType.RANGE],
        power=3,
    };
    public static CardUnitBlueprint c1U2 = new() {
        name="King Slime",
        description="Slimes normally aren't intelligent, but occasionally they merge together to become a powerful force to swallow all things.",
        image_name="graphics/cards/1U2",
        types=[RowType.MELEE,RowType.SIEGE],
        power=3,
    };
    public static CardUnitBlueprint c1U3 = new() {
        name="Eater of Worlds",
        description="Conceived from the bottomless malice of the Corruption, this mighty abyssal worm tunnels wildly to devour all in its path.",
        image_name="graphics/cards/1U3",
        types=[RowType.MELEE],
        power=4
    };
    public static CardUnitBlueprint c1U4 = new() {
        name="Wall of Flesh",
        description="Serving as the world's core and guardian, the towering demon lord exists to keep powerful ancient spirits sealed away. The Wall of Flesh's many mouths, attached by bloody veins. As a last resort, they can tear away and hungrily chase down threats.",
        image_name="graphics/cards/1U4",
        types=[RowType.SIEGE],
        power=7,
        is_hero=true,
        effect=new EffectRemoveRivalWeakest()
    };
    public static CardUnitBlueprint c1U5 = new() {
        name="Brain of Cthulhu",
        description="A piece of Cthulhu torn asunder, this vile mastermind pulses with agony and aids the Crimson in an attempt to avenge its master.",
        image_name="graphics/cards/1U5",
        types=[RowType.RANGE,RowType.SIEGE],
        power=5,
    };
    public static CardUnitBlueprint c1U6 = new() {
        name="Plantera",
        description="A dormant, yet powerful floral guardian awoken by the fallout of Cthulhu's destroyed machinations. Its reach spans the entire jungle.",
        image_name="graphics/cards/1U6",
        types=[RowType.MELEE,RowType.SIEGE],
        power=5,
    };
    public static CardUnitBlueprint c1U7 = new() {
        name="Golem",
        description="A remarkable display of ingenuity constructed by the Lihzahrd clan. Powered by solar energy cells, it is ready to guard the Temple.",
        image_name="graphics/cards/1U7",
        types=[RowType.MELEE],
        power=8,
        is_hero=true,
        effect=new EffectDrawCard()
    };
    public static CardUnitBlueprint c1U8 = new() {
        name="Lunatic Cultist",
        description="A fanatical leader hell-bent on bringing about the apocalypse by reviving the great Cthulhu through behind-the-scenes scheming.",
        image_name="graphics/cards/1U8",
        types=[RowType.RANGE],
        power=9,
        is_hero=true,
        effect=new EffectAveragePower()
    };
    public static CardDecoyBlueprint c1D1 = new() {
        name="Goldfish",
        description="A seemingly ordinary goldfish, until it decides to rain.",
        image_name="graphics/cards/1D1",
        types=[RowType.RANGE]
    };
    public static CardDecoyBlueprint c1D2 = new() {
        name="Bunny",
        description="Fuzzy wuzzy creatures that prefer safe, friendly locations.",
        image_name="graphics/cards/1D2",
        types=[RowType.MELEE]
    };
    public static CardWeatherBlueprint c1W1 = new() {
        name="Blood Moon",
        description="The Blood Moon is rising... You can tell a Blood Moon is out when the sky turns red. There is something about it that causes monsters to swarm.",
        image_name="graphics/cards/1W1",
        types=[RowType.MELEE,RowType.RANGE]
    };
    public static CardWeatherBlueprint c1W2 = new() {
        name="Solar Eclipse",
        description="A solar eclipse is happening! A day darker than night filled with creatures of horror.",
        image_name="graphics/cards/1W2",
        types=[RowType.SIEGE]
    };
    public static CardWeatherBlueprint c1W3 = new() {
        name="Slime Rain",
        description="Slime is falling from the sky! It's a slime rain, where gelatinous organisms fall from the sky in droves.",
        image_name="graphics/cards/1W3",
        types=[RowType.RANGE]
    };
    public static CardDispelBlueprint c1P1 = new() {
        name="Journey's End",
        description="After all the long roads, culmination is eventually reached. This is this Journey's End.",
        image_name="graphics/cards/1P1"
    };
    public static CardBoostBlueprint c1B1 = new() {
        name="The Destroyer",
        description="A mechanical simulacrum of Cthulhu's spine decorated in laser-armed probes, which detach from its body when damaged.",
        image_name="graphics/cards/1B1",
        types=[RowType.MELEE],
        bonus=1
    };
    public static CardBoostBlueprint c1B2 = new() {
        name="The Twins",
        description="Belonging to a pair of mechanically recreated Eyes of Cthulhu, one focuses its energy into firing powerful lasers, while the other chases at high speed, exhaling cursed flames..",
        image_name="graphics/cards/1B2",
        types=[RowType.RANGE],
        bonus=1
    };
    public static CardBoostBlueprint c1B3 = new() {
        name="Skeletron Prime",
        description="Mechanically reconstructed for reviving Cthulhu, this Skeletron has more arms than ever before, and a variety of fierce weapons.",
        image_name="graphics/cards/1B3",
        types=[RowType.SIEGE],
        bonus=1
    };


    public static CardLeaderBlueprint c2L = new() {
        name="Asgore",
        description="I so badly want to say, \"would you like a cup of tea?\" But...You know how it is.",
        image_name="graphics/cards/2L",
        effect=new EffectLeaderRecoverLastDiscardedCard()
    };
    public static CardUnitBlueprint c2U1 = new() {
        name="Toriel",
        description="I am TORIEL, caretaker of the RUINS.",
        image_name="graphics/cards/2U1",
        types=[RowType.RANGE,RowType.SIEGE],
        power=3,
    };
    public static CardUnitBlueprint c2U2 = new() {
        name="Sans",
        description="it's a beautiful day outside.birds are singing, flowers are blooming... on days like these, kids like you... Should be burning in hell.",
        image_name="graphics/cards/2U2",
        types=[RowType.RANGE],
        power=8,
        is_hero=true,
        effect=new EffectClearEmptiestRow()
    };
    public static CardUnitBlueprint c2U3 = new() {
        name="Papyrus",
        description="I WILL BE THE ONE! I MUST BE THE ONE! I WILL CAPTURE A HUMAN! THEN, I, THE GREAT PAPYRUS... WILL GET ALL THE THINGS I UTTERLY DESERVE!",
        image_name="graphics/cards/2U3",
        types=[RowType.MELEE],
        power=3,
        effect=new EffectSetNthMultPower()
    };
    public static CardUnitBlueprint c2U4 = new() {
        name="Undyne",
        description="Now, human! Let's end this, right here, right now. I'll show you how determined monsters can be! Step forward when you're ready! Fuhuhuhu!",
        image_name="graphics/cards/2U4",
        types=[RowType.MELEE,RowType.RANGE],
        power=5
    };
    public static CardUnitBlueprint c2U5 = new() {
        name="Undyne the Undying",
        description="SCREW IT! WHY SHOULD I TELL THAT STORY WHEN YOU'RE ABOUT TO DIE!?! NGAAAHHHH!",
        image_name="graphics/cards/2U5",
        types=[RowType.MELEE],
        power=7,
        is_hero=true,
        effect=new EffectRemoveStrongest()
    };
    public static CardUnitBlueprint c2U6 = new() {
        name="Mettaton",
        description="I'M NOT GOING TO DESTROY YOU WITHOUT A LIVE TELEVISION AUDIENCE!!",
        image_name="graphics/cards/2U6",
        types=[RowType.RANGE],
        power=4,
        effect=new EffectSetBoost(RowType.RANGE)
    };
    public static CardUnitBlueprint c2U7 = new() {
        name="Asriel",
        description="Don't kill, and don't be killed, alright? That's the best you can strive for.",
        image_name="graphics/cards/2U7",
        types=[RowType.SIEGE],
        power=6
    };
    public static CardUnitBlueprint c2U8 = new() {
        name="Photoshop Flowey",
        description="In this world it's kill or be killed. This is all just a bad dream...And you're NEVER waking up!",
        image_name="graphics/cards/2U8",
        types=[RowType.SIEGE],
        power=9,
        is_hero=true,
        effect=new EffectDrawCard()
    };
    public static CardDecoyBlueprint c2D1 = new() {
        name="Flowey",
        description="Howdy! I'm FLOWEY. FLOWEY the FLOWER!",
        image_name="graphics/cards/2D1",
        types=[RowType.MELEE]
    };
    public static CardDecoyBlueprint c2D2 = new() {
        name="Annoying Dog",
        description="I'm not snoring, I'm cheering you on in my sleep!! ...Oh, you're still here? Don't you have anything better to do?",
        image_name="graphics/cards/2D2",
        types=[RowType.MELEE,RowType.RANGE,RowType.SIEGE]
    };
    public static CardWeatherBlueprint c2W1 = new() {
        name="Muffet",
        description="Don't look so blue, my deary~... I think purple is a better look on you! Ahuhuhu~",
        image_name="graphics/cards/2W1",
        types=[RowType.MELEE]
    };
    public static CardWeatherBlueprint c2W2 = new() {
        name="It's Raining Somewhere",
        description="Someone who sincerely likes bad jokes... has an integrity you can't say \"no\" to.",
        image_name="graphics/cards/2W2",
        types=[RowType.RANGE]
    };
    public static CardWeatherBlueprint c2W3 = new() {
        name="Amalgam",
        description="Welcome to my special hell.",
        image_name="graphics/cards/2W3",
        types=[RowType.SIEGE]
    };
    public static CardDispelBlueprint c2P1 = new() {
        name="Barrier",
        description="This is the barrier. This is what keeps us all trapped underground.",
        image_name="graphics/cards/2P1"
    };
    public static CardBoostBlueprint c2B1 = new() {
        name="Soul",
        description="See that heart? That is your SOUL, the very culmination of your being!",
        image_name="graphics/cards/2B1",
        types=[RowType.MELEE],
        bonus=1
    };
    public static CardBoostBlueprint c2B2 = new() {
        name="Death by Glamour",
        description="Lights! Camera! Action!",
        image_name="graphics/cards/2B2",
        types=[RowType.RANGE],
        bonus=1
    };
    public static CardBoostBlueprint c2B3 = new() {
        name="Temmie",
        description="hOI!!! i'm TEMMIE!!",
        image_name="graphics/cards/2B3",
        types=[RowType.SIEGE],
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

    private static readonly Deck deck0 = new(
        "Hallownest",
        "graphics/cards/0",
        new() {
            // Total: 25
            // Unit: 16
                // Silver: 13
                {c0U1,3},
                {c0U2,3},
                {c0U3,3},
                {c0U7,2},
                {c0U8,2},
                // Golden: 3
                {c0U4,1},
                {c0U5,1},
                {c0U6,1},
            // Weather: 3
            {c0W1,1},
            {c0W2,1},
            {c0W3,1},
            // Dispel: 1
            {c0P1,1},
            // Boost: 3
            {c0B1,1},
            {c0B2,1},
            {c0B3,1},
            // Decoy: 2
            {c0D1,1},
            {c0D2,1},
        },
        c0L
    );

    private static readonly Deck deck1 = new(
        "Terraria",
        "graphics/cards/1",
        new () {
            // Total: 25
            // Unit: 16
                // Silver: 13
                {c1U1,3},
                {c1U2,3},
                {c1U3,3},
                {c1U5,2},
                {c1U6,2},
                // Golden: 3
                {c1U4,1},
                {c1U7,1},
                {c1U8,1},
            // Weather: 3
            {c1W1,1},
            {c1W2,1},
            {c1W3,1},
            // Dispel: 1
            {c1P1,1},
            // Boost: 3
            {c1B1,1},
            {c1B2,1},
            {c1B3,1},
            // Decoy: 2
            {c1D1,1},
            {c1D2,1},
        },
        c1L
    );

    private static readonly Deck deck2 = new(
        "Undertale",
        "graphics/cards/2",
        new () {
            // Total: 25
            // Unit: 16
                // Silver: 13
                {c2U1,3},
                {c2U3,4},
                {c2U4,3},
                {c2U6,1},
                {c2U7,2},
                // Golden: 3
                {c2U2,1},
                {c2U5,1},
                {c2U8,1},
            // Weather: 3
            {c2W1,1},
            {c2W2,1},
            {c2W3,1},
            // Dispel: 1
            {c2P1,1},
            // Boost: 3
            {c2B1,1},
            {c2B2,1},
            {c2B3,1},
            // Decoy: 2
            {c2D1,1},
            {c2D2,1},
        },
        c1L
    );

    private const int DEFAULT_DECK_INDEX = 0;
    private static Deck[] decks = typeof(DecksDump)
        .GetFields(BindingFlags.NonPublic | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(Deck))
        .Select(f => (Deck)f.GetValue(null))
        .ToArray();
    public static int Count {get => decks.Length;}

    public static Deck GetDeck(int index=DEFAULT_DECK_INDEX) {
        return decks[index];
    }

    public static void Initialize() {
        foreach (var deck in decks) {
            deck.Populate();
        }
    }

    public static void LoadContent(GameTools gt) {
        foreach (var deck in decks) {
            deck.LoadContent(gt);
        }
    }

}

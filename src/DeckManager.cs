using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGwent;

public class Deck : Stack<Card> {
    public string name;
    public string img_name;
    public CardLeader leader;

    public Texture2D img;

    public void LoadContent(GameTools gt) {
        img = gt.content.Load<Texture2D>(img_name);
    }

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
    public UnitEffect effect = UnitEffect.NONE;

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
        image_name="graphics/cards/0L",
        effect=LeaderEffect.DRAW_CARD
    };
    public static CardUnitBlueprint card_0U1 = new() {
        name="The Knight",
        description="An enigmatic wanderer who descends into Hallownest carrying only a broken nail to fend off foes.",
        image_name="graphics/cards/0U1",
        types=[RowType.MELEE,RowType.SIEGE],
        power=3,
    };
    public static CardUnitBlueprint card_0U2 = new() {
        name="Hornet",
        description="Skilled protector of Hallownest's ruins. Wields a needle and thread.",
        image_name="graphics/cards/0U2",
        types=[RowType.MELEE,RowType.RANGE],
        power=3,
    };
    public static CardUnitBlueprint card_0U3 = new() {
        name="Mantis Lords",
        description="Leaders of the Mantis tribe and its finest warriors. They bear thin nail-lances and attack with blinding speed.",
        image_name="graphics/cards/0U3",
        types=[RowType.MELEE,RowType.RANGE],
        power=4,
    };
    public static CardUnitBlueprint card_0U4 = new() {
        name="Hollow Knight",
        description="Fully grown Vessel, carrying the plague's heart within its body.",
        image_name="graphics/cards/0U4",
        types=[RowType.MELEE],
        power=7,
        is_hero=true
    };
    public static CardUnitBlueprint card_0U5 = new() {
        name="Radiance",
        description="The light, forgotten.",
        image_name="graphics/cards/0U5",
        types=[RowType.RANGE],
        power=8,
        is_hero=true
    };
    public static CardUnitBlueprint card_0U6 = new() {
        name="Nightmare King Grimm",
        description="The expanse of dream in past was split,\nOne realm now must stay apart,\nDarkest reaches, beating red,\nTerror of sleep. The Nightmare's Heart.",
        image_name="graphics/cards/0U6",
        types=[RowType.SIEGE],
        power=9,
        is_hero=true
    };
    public static CardUnitBlueprint card_0U7 = new() {
        name="Dung Defender",
        description="Skilled combatant living at the heart of the Waterways. Assails intruders with balls of compacted dung.",
        image_name="graphics/cards/0U7",
        types=[RowType.SIEGE],
        power=4,
        effect=UnitEffect.DRAW_CARD
    };
    public static CardUnitBlueprint card_0U8 = new() {
        name="White Defender",
        description="The Champion's Call, the Knotted Grove, the Battle of the Blackwyrm... I remember it all. I will carry those glories with me always... until we meet again.",
        image_name="graphics/cards/0U8",
        types=[RowType.SIEGE],
        power=5
    };
    public static CardDecoyBlueprint card_0D1 = new() {
        name="Zote",
        description="A self-proclaimed Knight, of no renown. Wields a nail he carved from shellwood, named 'Life Ender.'",
        image_name="graphics/cards/0D1",
        types=[RowType.MELEE]
    };
    public static CardDecoyBlueprint card_0D2 = new() {
        name="False Knight",
        description="Weak creatures love to steal the strength of others. Their lives are brief and fearful, and they yearn to have the power to dominate those who have dominated them.",
        image_name="graphics/cards/0D2",
        types=[RowType.SIEGE]
    };
    public static CardWeatherBlueprint card_0W1 = new() {
        name="City of Tears",
        description="The city looks to be built into an enormous cavern, and the rain pours down from cracks in the stone above. There must be a lot of water up there somewhere.",
        image_name="graphics/cards/0W1",
        types=[RowType.RANGE,RowType.SIEGE]
    };
    public static CardWeatherBlueprint card_0W2 = new() {
        name="Kingdom's Edge",
        description="This ashen place is grave of Wyrm. Once told, it came to die. But what is death for that ancient being? More transformation methinks. This failed kingdom is product of the being spawned from that event.",
        image_name="graphics/cards/0W2",
        types=[RowType.MELEE]
    };
    public static CardWeatherBlueprint card_0W3 = new() {
        name="The Infection",
        description="The bugs of Hallownest were twisted out of shape by that ancient sickness. First they fell into deep slumber, then they awoke with broken minds, and then their bodies started to deform...",
        image_name="graphics/cards/0W3",
        types=[RowType.RANGE]
    };
    public static CardDispelBlueprint card_0P1 = new() {
        name="Lake of Unn",
        description="The greater mind once dreamed of leaf and cast these caverns so. In every bush and every vine the mind of Unn reveals itself to us.",
        image_name="graphics/cards/0P1"
    };
    public static CardBoostBlueprint card_0B1 = new() {
        name="Grey Prince Zote",
        description="Figment of an obsessed mind. Lacks grace but becomes stronger with every defeat.",
        image_name="graphics/cards/0B1",
        types=[RowType.MELEE],
        bonus=1
    };
    public static CardBoostBlueprint card_0B2 = new() {
        name="The Grimm Troupe",
        description="Shadows dream of endless fire,\nFlames devour and embers swoop,\nOne will light the Nightmare Lantern,\nCall and serve in Grimm's dread Troupe.",
        image_name="graphics/cards/0B2",
        types=[RowType.RANGE],
        bonus=1
    };
    public static CardBoostBlueprint card_0B3 = new() {
        name="White Lady",
        description="These bindings about me, I've chosen to erect. There is some shame I feel from my own part in the deed, and this method guarantees it cease.",
        image_name="graphics/cards/0B3",
        types=[RowType.SIEGE],
        bonus=1
    };


    public static CardLeaderBlueprint card_1L = new() {
        name="Moon Lord",
        description="The mastermind behind all terrors which befall the world, freed from his lunar prison. Practically a god, his power knows no limits.",
        image_name="graphics/cards/1L",
        effect=LeaderEffect.WIN_ON_DRAW
    };
    public static CardUnitBlueprint card_1U1 = new() {
        name="Eye of Cthulhu",
        description="A piece of Cthulhu ripped from his body centuries ago in a bloody war. It wanders the night seeking its host body... and revenge!",
        image_name="graphics/cards/1U1",
        types=[RowType.RANGE],
        power=3,
    };
    public static CardUnitBlueprint card_1U2 = new() {
        name="King Slime",
        description="Slimes normally aren't intelligent, but occasionally they merge together to become a powerful force to swallow all things.",
        image_name="graphics/cards/1U2",
        types=[RowType.MELEE,RowType.SIEGE],
        power=3,
    };
    public static CardUnitBlueprint card_1U3 = new() {
        name="Eater of Worlds",
        description="Conceived from the bottomless malice of the Corruption, this mighty abyssal worm tunnels wildly to devour all in its path.",
        image_name="graphics/cards/1U3",
        types=[RowType.MELEE],
        power=4,
        effect=UnitEffect.DRAW_CARD
    };
    public static CardUnitBlueprint card_1U4 = new() {
        name="Wall of Flesh",
        description="Serving as the world's core and guardian, the towering demon lord exists to keep powerful ancient spirits sealed away. The Wall of Flesh's many mouths, attached by bloody veins. As a last resort, they can tear away and hungrily chase down threats.",
        image_name="graphics/cards/1U4",
        types=[RowType.SIEGE],
        power=7,
        is_hero=true
    };
    public static CardUnitBlueprint card_1U5 = new() {
        name="Brain of Cthulhu",
        description="A piece of Cthulhu torn asunder, this vile mastermind pulses with agony and aids the Crimson in an attempt to avenge its master.",
        image_name="graphics/cards/1U5",
        types=[RowType.RANGE,RowType.SIEGE],
        power=5,
    };
    public static CardUnitBlueprint card_1U6 = new() {
        name="Plantera",
        description="A dormant, yet powerful floral guardian awoken by the fallout of Cthulhu's destroyed machinations. Its reach spans the entire jungle.",
        image_name="graphics/cards/1U6",
        types=[RowType.MELEE,RowType.SIEGE],
        power=5,
    };
    public static CardUnitBlueprint card_1U7 = new() {
        name="Golem",
        description="A remarkable display of ingenuity constructed by the Lihzahrd clan. Powered by solar energy cells, it is ready to guard the Temple.",
        image_name="graphics/cards/1U7",
        types=[RowType.MELEE],
        power=8,
        is_hero=true
    };
    public static CardUnitBlueprint card_1U8 = new() {
        name="Lunatic Cultist",
        description="A fanatical leader hell-bent on bringing about the apocalypse by reviving the great Cthulhu through behind-the-scenes scheming.",
        image_name="graphics/cards/1U8",
        types=[RowType.RANGE],
        power=9,
        is_hero=true
    };
    public static CardDecoyBlueprint card_1D1 = new() {
        name="Goldfish",
        description="A seemingly ordinary goldfish, until it decides to rain.",
        image_name="graphics/cards/1D1",
        types=[RowType.RANGE]
    };
    public static CardDecoyBlueprint card_1D2 = new() {
        name="Bunny",
        description="Fuzzy wuzzy creatures that prefer safe, friendly locations.",
        image_name="graphics/cards/1D2",
        types=[RowType.MELEE]
    };
    public static CardWeatherBlueprint card_1W1 = new() {
        name="Blood Moon",
        description="The Blood Moon is rising... You can tell a Blood Moon is out when the sky turns red. There is something about it that causes monsters to swarm.",
        image_name="graphics/cards/1W1",
        types=[RowType.MELEE,RowType.RANGE]
    };
    public static CardWeatherBlueprint card_1W2 = new() {
        name="Solar Eclipse",
        description="A solar eclipse is happening! A day darker than night filled with creatures of horror.",
        image_name="graphics/cards/1W2",
        types=[RowType.SIEGE]
    };
    public static CardWeatherBlueprint card_1W3 = new() {
        name="Slime Rain",
        description="Slime is falling from the sky! It's a slime rain, where gelatinous organisms fall from the sky in droves.",
        image_name="graphics/cards/1W3",
        types=[RowType.RANGE]
    };
    public static CardDispelBlueprint card_1P1 = new() {
        name="Journey's End",
        description="After all the long roads, culmination is eventually reached. This is this Journey's End.",
        image_name="graphics/cards/1P1"
    };
    public static CardBoostBlueprint card_1B1 = new() {
        name="The Destroyer",
        description="A mechanical simulacrum of Cthulhu's spine decorated in laser-armed probes, which detach from its body when damaged.",
        image_name="graphics/cards/1B1",
        types=[RowType.MELEE],
        bonus=1
    };
    public static CardBoostBlueprint card_1B2 = new() {
        name="The Twins",
        description="Belonging to a pair of mechanically recreated Eyes of Cthulhu, one focuses its energy into firing powerful lasers, while the other chases at high speed, exhaling cursed flames..",
        image_name="graphics/cards/1B2",
        types=[RowType.RANGE],
        bonus=1
    };
    public static CardBoostBlueprint card_1B3 = new() {
        name="Skeletron Prime",
        description="Mechanically reconstructed for reviving Cthulhu, this Skeletron has more arms than ever before, and a variety of fierce weapons.",
        image_name="graphics/cards/1B3",
        types=[RowType.SIEGE],
        bonus=1
    };


    public static CardLeaderBlueprint card_2L = new() {
        name="Asgore",
        description="I so badly want to say, \"would you like a cup of tea?\" But...You know how it is.",
        image_name="graphics/cards/2L",
        effect=LeaderEffect.RECOVER_LAST_DISCARDED_CARD
    };
    public static CardUnitBlueprint card_2U1 = new() {
        name="Toriel",
        description="I am TORIEL, caretaker of the RUINS.",
        image_name="graphics/cards/2U1",
        types=[RowType.RANGE],
        power=3,
    };
    public static CardUnitBlueprint card_2U2 = new() {
        name="Sans",
        description="it's a beautiful day outside.birds are singing, flowers are blooming... on days like these, kids like you... Should be burning in hell.",
        image_name="graphics/cards/2U2",
        types=[RowType.RANGE],
        power=8,
        is_hero=true
    };
    public static CardUnitBlueprint card_2U3 = new() {
        name="Papyrus",
        description="I WILL BE THE ONE! I MUST BE THE ONE! I WILL CAPTURE A HUMAN! THEN, I, THE GREAT PAPYRUS... WILL GET ALL THE THINGS I UTTERLY DESERVE!",
        image_name="graphics/cards/2U3",
        types=[RowType.MELEE],
        power=3,
        effect=UnitEffect.DRAW_CARD
    };
    public static CardUnitBlueprint card_2U4 = new() {
        name="Undyne",
        description="Now, human! Let's end this, right here, right now. I'll show you how determined monsters can be! Step forward when you're ready! Fuhuhuhu!",
        image_name="graphics/cards/2U4",
        types=[RowType.MELEE,RowType.RANGE],
        power=5
    };
    public static CardUnitBlueprint card_2U5 = new() {
        name="Undyne the Undying",
        description="SCREW IT! WHY SHOULD I TELL THAT STORY WHEN YOU'RE ABOUT TO DIE!?! NGAAAHHHH!",
        image_name="graphics/cards/2U5",
        types=[RowType.MELEE],
        power=7,
        is_hero=true
    };
    public static CardUnitBlueprint card_2U6 = new() {
        name="Mettaton",
        description="I'M NOT GOING TO DESTROY YOU WITHOUT A LIVE TELEVISION AUDIENCE!!",
        image_name="graphics/cards/2U6",
        types=[RowType.RANGE,RowType.SIEGE],
        power=4,
    };
    public static CardUnitBlueprint card_2U7 = new() {
        name="Asriel",
        description="Don't kill, and don't be killed, alright? That's the best you can strive for.",
        image_name="graphics/cards/2U7",
        types=[RowType.SIEGE],
        power=6
    };
    public static CardUnitBlueprint card_2U8 = new() {
        name="Photoshop Flowey",
        description="In this world it's kill or be killed. This is all just a bad dream...And you're NEVER waking up!",
        image_name="graphics/cards/2U8",
        types=[RowType.SIEGE],
        power=9,
        is_hero=true
    };
    public static CardDecoyBlueprint card_2D1 = new() {
        name="Flowey",
        description="Howdy! I'm FLOWEY. FLOWEY the FLOWER!",
        image_name="graphics/cards/2D1",
        types=[RowType.MELEE]
    };
    public static CardDecoyBlueprint card_2D2 = new() {
        name="Annoying Dog",
        description="I'm not snoring, I'm cheering you on in my sleep!! ...Oh, you're still here? Don't you have anything better to do?",
        image_name="graphics/cards/2D2",
        types=[RowType.MELEE,RowType.RANGE,RowType.SIEGE]
    };
    public static CardWeatherBlueprint card_2W1 = new() {
        name="Muffet",
        description="Don't look so blue, my deary~... I think purple is a better look on you! Ahuhuhu~",
        image_name="graphics/cards/2W1",
        types=[RowType.MELEE]
    };
    public static CardWeatherBlueprint card_2W2 = new() {
        name="It's Raining Somewhere",
        description="Someone who sincerely likes bad jokes... has an integrity you can't say \"no\" to.",
        image_name="graphics/cards/2W2",
        types=[RowType.RANGE]
    };
    public static CardWeatherBlueprint card_2W3 = new() {
        name="Amalgam",
        description="Welcome to my special hell.",
        image_name="graphics/cards/2W3",
        types=[RowType.SIEGE]
    };
    public static CardDispelBlueprint card_2P1 = new() {
        name="Barrier",
        description="This is the barrier. This is what keeps us all trapped underground.",
        image_name="graphics/cards/2P1"
    };
    public static CardBoostBlueprint card_2B1 = new() {
        name="Soul",
        description="See that heart? That is your SOUL, the very culmination of your being!",
        image_name="graphics/cards/2B1",
        types=[RowType.MELEE],
        bonus=1
    };
    public static CardBoostBlueprint card_2B2 = new() {
        name="Death by Glamour",
        description="Lights! Camera! Action!",
        image_name="graphics/cards/2B2",
        types=[RowType.RANGE],
        bonus=1
    };
    public static CardBoostBlueprint card_2B3 = new() {
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

    public static Deck[] decks;

    public class Deck1 : IDeckGetter {
        public static string GetName() => "Hallownest";
        public static string GetImageName() => "graphics/cards/0";

        public static Deck GetDeck() {
            var deck = new Deck
            {
                // Total: 25
                // Unit: 16
                    // Silver: 13
                    CardsDump.card_0U1.GetCard(),
                    CardsDump.card_0U1.GetCard(),
                    CardsDump.card_0U1.GetCard(),
                    CardsDump.card_0U2.GetCard(),
                    CardsDump.card_0U2.GetCard(),
                    CardsDump.card_0U2.GetCard(),
                    CardsDump.card_0U3.GetCard(),
                    CardsDump.card_0U3.GetCard(),
                    CardsDump.card_0U3.GetCard(),
                    CardsDump.card_0U7.GetCard(),
                    CardsDump.card_0U7.GetCard(),
                    CardsDump.card_0U8.GetCard(),
                    CardsDump.card_0U8.GetCard(),
                    // Golden: 3
                    CardsDump.card_0U4.GetCard(),
                    CardsDump.card_0U5.GetCard(),
                    CardsDump.card_0U6.GetCard(),
                // Weather: 3
                CardsDump.card_0W1.GetCard(),
                CardsDump.card_0W2.GetCard(),
                CardsDump.card_0W3.GetCard(),
                // Dispel: 1
                CardsDump.card_0P1.GetCard(),
                // Boost: 3
                CardsDump.card_0B1.GetCard(),
                CardsDump.card_0B2.GetCard(),
                CardsDump.card_0B3.GetCard(),
                // Decoy: 2
                CardsDump.card_0D1.GetCard(),
                CardsDump.card_0D2.GetCard(),
            };
            deck.name = GetName();
            deck.img_name = GetImageName();
            deck.leader = (CardLeader)CardsDump.card_0L.GetCard();
            return deck;
        }

    }

    public class Deck2 : IDeckGetter {
        public static string GetName() => "Terraria";
        public static string GetImageName() => "graphics/cards/1";

        public static Deck GetDeck() {
            var deck = new Deck
            {
                // Total: 25
                // Unit: 16
                    // Silver: 13
                    CardsDump.card_1U1.GetCard(),
                    CardsDump.card_1U1.GetCard(),
                    CardsDump.card_1U1.GetCard(),
                    CardsDump.card_1U2.GetCard(),
                    CardsDump.card_1U2.GetCard(),
                    CardsDump.card_1U2.GetCard(),
                    CardsDump.card_1U3.GetCard(),
                    CardsDump.card_1U3.GetCard(),
                    CardsDump.card_1U3.GetCard(),
                    CardsDump.card_1U5.GetCard(),
                    CardsDump.card_1U5.GetCard(),
                    CardsDump.card_1U6.GetCard(),
                    CardsDump.card_1U6.GetCard(),
                    // Golden: 3
                    CardsDump.card_1U4.GetCard(),
                    CardsDump.card_1U7.GetCard(),
                    CardsDump.card_1U8.GetCard(),
                // Weather: 3
                CardsDump.card_1W1.GetCard(),
                CardsDump.card_1W2.GetCard(),
                CardsDump.card_1W3.GetCard(),
                // Dispel: 1
                CardsDump.card_1P1.GetCard(),
                // Boost: 3
                CardsDump.card_1B1.GetCard(),
                CardsDump.card_1B2.GetCard(),
                CardsDump.card_1B3.GetCard(),
                // Decoy: 2
                CardsDump.card_1D1.GetCard(),
                CardsDump.card_1D2.GetCard(),
            };
            deck.name = GetName();
            deck.img_name = GetImageName();
            deck.leader = (CardLeader)CardsDump.card_1L.GetCard();
            return deck;
        }

    }

    public class Deck3 : IDeckGetter {
        public static string GetName() => "Undertale";
        public static string GetImageName() => "graphics/cards/2";

        public static Deck GetDeck() {
            var deck = new Deck
            {
                // Total: 25
                // Unit: 16
                    // Silver: 13
                    CardsDump.card_2U1.GetCard(),
                    CardsDump.card_2U1.GetCard(),
                    CardsDump.card_2U1.GetCard(),
                    CardsDump.card_2U3.GetCard(),
                    CardsDump.card_2U3.GetCard(),
                    CardsDump.card_2U3.GetCard(),
                    CardsDump.card_2U4.GetCard(),
                    CardsDump.card_2U4.GetCard(),
                    CardsDump.card_2U4.GetCard(),
                    CardsDump.card_2U6.GetCard(),
                    CardsDump.card_2U6.GetCard(),
                    CardsDump.card_2U7.GetCard(),
                    CardsDump.card_2U7.GetCard(),
                    // Golden: 3
                    CardsDump.card_2U2.GetCard(),
                    CardsDump.card_2U5.GetCard(),
                    CardsDump.card_2U8.GetCard(),
                // Weather: 3
                CardsDump.card_2W1.GetCard(),
                CardsDump.card_2W2.GetCard(),
                CardsDump.card_2W3.GetCard(),
                // Dispel: 1
                CardsDump.card_2P1.GetCard(),
                // Boost: 3
                CardsDump.card_2B1.GetCard(),
                CardsDump.card_2B2.GetCard(),
                CardsDump.card_2B3.GetCard(),
                // Decoy: 2
                CardsDump.card_2D1.GetCard(),
                CardsDump.card_2D2.GetCard(),
            };
            deck.name = GetName();
            deck.img_name = GetImageName();
            deck.leader = (CardLeader)CardsDump.card_2L.GetCard();
            return deck;
        }

    }

    public static void Initialize() {
        decks = [
            Deck1.GetDeck(),
            Deck2.GetDeck(),
            Deck3.GetDeck()
        ];
    }

    public static void LoadContent(GameTools gt) {
        foreach (var deck in decks) {
            deck.LoadContent(gt);
        }
    }

}

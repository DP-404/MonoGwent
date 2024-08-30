using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGwent;

public struct CardsDump {

    public static List<CardBlueprint> blueprints = new() {

        new CardLeaderBlueprint() {
            name="The Pale King",
            description="No cost too great.",
            faction="Hallownest",
            effects=new List<IEffect>() {new EffectDrawCard()},
        },

        new CardUnitBlueprint() {
            name="The Knight",
            description="An enigmatic wanderer who descends into Hallownest carrying only a broken nail to fend off foes.",
            faction="Hallownest",
            types=[RowType.MELEE,RowType.SIEGE],
            power=3,
        },

        new CardUnitBlueprint() {
            name="Hornet",
            description="Skilled protector of Hallownest's ruins. Wields a needle and thread.",
            faction="Hallownest",
            types=[RowType.MELEE,RowType.RANGE],
            power=3,
        },

        new CardUnitBlueprint() {
            name="Mantis Lords",
            description="Leaders of the Mantis tribe and its finest warriors. They bear thin nail-lances and attack with blinding speed.",
            faction="Hallownest",
            types=[RowType.MELEE,RowType.RANGE],
            power=4,
        },

        new CardUnitBlueprint() {
            name="Hollow Knight",
            description="Fully grown Vessel, carrying the plague's heart within its body.",
            faction="Hallownest",
            types=[RowType.MELEE],
            power=7,
            is_hero=true,
            effects=new List<IEffect>() {new EffectSetWeather(RowType.MELEE)}
        },

        new CardUnitBlueprint() {
            name="Radiance",
            description="The light, forgotten.",
            faction="Hallownest",
            types=[RowType.RANGE],
            power=8,
            is_hero=true
        },

        new CardUnitBlueprint() {
            name="Nightmare King Grimm",
            description="The expanse of dream in past was split,\nOne realm now must stay apart,\nDarkest reaches, beating red,\nTerror of sleep. The Nightmare's Heart.",
            faction="Hallownest",
            types=[RowType.SIEGE],
            power=9,
            is_hero=true,
            effects=new List<IEffect>() {new EffectSetBoost(RowType.SIEGE)}
        },

        new CardUnitBlueprint() {
            name="Dung Defender",
            description="Skilled combatant living at the heart of the Waterways. Assails intruders with balls of compacted dung.",
            faction="Hallownest",
            types=[RowType.SIEGE],
            power=4
        },

        new CardUnitBlueprint() {
            name="White Defender",
            description="The Champion's Call, the Knotted Grove, the Battle of the Blackwyrm... I remember it all. I will carry those glories with me always... until we meet again.",
            faction="Hallownest",
            types=[RowType.SIEGE],
            power=5
        },

        new CardDecoyBlueprint() {
            name="Zote",
            description="A self-proclaimed Knight, of no renown. Wields a nail he carved from shellwood, named 'Life Ender.'",
            faction="Hallownest",
            types=[RowType.MELEE]
        },

        new CardDecoyBlueprint() {
            name="False Knight",
            description="Weak creatures love to steal the strength of others. Their lives are brief and fearful, and they yearn to have the power to dominate those who have dominated them.",
            faction="Hallownest",
            types=[RowType.SIEGE]
        },

        new CardWeatherBlueprint() {
            name="City of Tears",
            description="The city looks to be built into an enormous cavern, and the rain pours down from cracks in the stone above. There must be a lot of water up there somewhere.",
            faction="Hallownest",
            types=[RowType.RANGE,RowType.SIEGE]
        },

        new CardWeatherBlueprint() {
            name="Kingdom's Edge",
            description="This ashen place is grave of Wyrm. Once told, it came to die. But what is death for that ancient being? More transformation methinks. This failed kingdom is product of the being spawned from that event.",
            faction="Hallownest",
            types=[RowType.MELEE]
        },

        new CardWeatherBlueprint() {
            name="The Infection",
            description="The bugs of Hallownest were twisted out of shape by that ancient sickness. First they fell into deep slumber, then they awoke with broken minds, and then their bodies started to deform...",
            faction="Hallownest",
            types=[RowType.RANGE]
        },

        new CardDispelBlueprint() {
            name="Lake of Unn",
            description="The greater mind once dreamed of leaf and cast these caverns so. In every bush and every vine the mind of Unn reveals itself to us.",
            faction="Hallownest",
        },

        new CardBoostBlueprint() {
            name="Grey Prince Zote",
            description="Figment of an obsessed mind. Lacks grace but becomes stronger with every defeat.",
            faction="Hallownest",
            types=[RowType.MELEE],
            bonus=1
        },

        new CardBoostBlueprint() {
            name="The Grimm Troupe",
            description="Shadows dream of endless fire,\nFlames devour and embers swoop,\nOne will light the Nightmare Lantern,\nCall and serve in Grimm's dread Troupe.",
            faction="Hallownest",
            types=[RowType.SIEGE],
            bonus=1
        },

        new CardBoostBlueprint() {
            name="White Lady",
            description="These bindings about me, I've chosen to erect. There is some shame I feel from my own part in the deed, and this method guarantees it cease.",
            faction="Hallownest",
            types=[RowType.RANGE],
            bonus=1
        },

        new CardLeaderBlueprint() {
            name="Moon Lord",
            description="The mastermind behind all terrors which befall the world, freed from his lunar prison. Practically a god, his power knows no limits.",
            faction="Terraria",
            effects=new List<IEffect>() {new EffectLeaderWinMatch()}
        },

        new CardUnitBlueprint() {
            name="Eye of Cthulhu",
            description="A piece of Cthulhu ripped from his body centuries ago in a bloody war. It wanders the night seeking its host body... and revenge!",
            faction="Terraria",
            types=[RowType.MELEE,RowType.SIEGE],
            power=3,
        },

        new CardUnitBlueprint() {
            name="King Slime",
            description="Slimes normally aren't intelligent, but occasionally they merge together to become a powerful force to swallow all things.",
            faction="Terraria",
            types=[RowType.MELEE,RowType.SIEGE],
            power=3,
        },

        new CardUnitBlueprint() {
            name="Eater of Worlds",
            description="Conceived from the bottomless malice of the Corruption, this mighty abyssal worm tunnels wildly to devour all in its path.",
            faction="Terraria",
            types=[RowType.RANGE],
            power=3
        },

        new CardUnitBlueprint() {
            name="Wall of Flesh",
            description="Serving as the world's core and guardian, the towering demon lord exists to keep powerful ancient spirits sealed away. The Wall of Flesh's many mouths, attached by bloody veins. As a last resort, they can tear away and hungrily chase down threats.",
            faction="Terraria",
            types=[RowType.SIEGE],
            power=7,
            is_hero=true,
            effects=new List<IEffect>() {new EffectRemoveRivalWeakest()}
        },

        new CardUnitBlueprint() {
            name="Brain of Cthulhu",
            description="A piece of Cthulhu torn asunder, this vile mastermind pulses with agony and aids the Crimson in an attempt to avenge its master.",
            faction="Terraria",
            types=[RowType.RANGE,RowType.SIEGE],
            power=5,
        },

        new CardUnitBlueprint() {
            name="Plantera",
            description="A dormant, yet powerful floral guardian awoken by the fallout of Cthulhu's destroyed machinations. Its reach spans the entire jungle.",
            faction="Terraria",
            types=[RowType.MELEE,RowType.SIEGE],
            power=5,
        },

        new CardUnitBlueprint() {
            name="Golem",
            description="A remarkable display of ingenuity constructed by the Lihzahrd clan. Powered by solar energy cells, it is ready to guard the Temple.",
            faction="Terraria",
            types=[RowType.MELEE],
            power=8,
            is_hero=true,
            effects=new List<IEffect>() {new EffectDrawCard()}
        },

        new CardUnitBlueprint() {
            name="Lunatic Cultist",
            description="A fanatical leader hell-bent on bringing about the apocalypse by reviving the great Cthulhu through behind-the-scenes scheming.",
            faction="Terraria",
            types=[RowType.RANGE],
            power=9,
            is_hero=true,
            effects=new List<IEffect>() {new EffectAveragePower()}
        },

        new CardDecoyBlueprint() {
            name="Goldfish",
            description="A seemingly ordinary goldfish, until it decides to rain.",
            faction="Terraria",
            types=[RowType.RANGE]
        },

        new CardDecoyBlueprint() {
            name="Bunny",
            description="Fuzzy wuzzy creatures that prefer safe, friendly locations.",
            faction="Terraria",
            types=[RowType.MELEE]
        },

        new CardWeatherBlueprint() {
            name="Blood Moon",
            description="The Blood Moon is rising... You can tell a Blood Moon is out when the sky turns red. There is something about it that causes monsters to swarm.",
            faction="Terraria",
            types=[RowType.MELEE,RowType.RANGE]
        },

        new CardWeatherBlueprint() {
            name="Solar Eclipse",
            description="A solar eclipse is happening! A day darker than night filled with creatures of horror.",
            faction="Terraria",
            types=[RowType.SIEGE]
        },

        new CardWeatherBlueprint() {
            name="Slime Rain",
            description="Slime is falling from the sky! It's a slime rain, where gelatinous organisms fall from the sky in droves.",
            faction="Terraria",
            types=[RowType.RANGE]
        },

        new CardDispelBlueprint() {
            name="Journey's End",
            description="After all the long roads, culmination is eventually reached. This is this Journey's End.",
            faction="Terraria",
        },

        new CardBoostBlueprint() {
            name="The Destroyer",
            description="A mechanical simulacrum of Cthulhu's spine decorated in laser-armed probes, which detach from its body when damaged.",
            faction="Terraria",
            types=[RowType.MELEE],
            bonus=1
        },

        new CardBoostBlueprint() {
            name="The Twins",
            description="Belonging to a pair of mechanically recreated Eyes of Cthulhu, one focuses its energy into firing powerful lasers, while the other chases at high speed, exhaling cursed flames..",
            faction="Terraria",
            types=[RowType.RANGE],
            bonus=1
        },

        new CardBoostBlueprint() {
            name="Skeletron Prime",
            description="Mechanically reconstructed for reviving Cthulhu, this Skeletron has more arms than ever before, and a variety of fierce weapons.",
            faction="Terraria",
            types=[RowType.SIEGE],
            bonus=1
        },

        new CardLeaderBlueprint() {
            name="Asgore",
            description="I so badly want to say, \"would you like a cup of tea?\" But...You know how it is.",
            faction="Undertale",
            effects=new List<IEffect>() {new EffectLeaderRecoverLastDiscardedCard()}
        },

        new CardUnitBlueprint() {
            name="Toriel",
            description="I am TORIEL, caretaker of the RUINS.",
            faction="Undertale",
            types=[RowType.RANGE,RowType.SIEGE],
            power=3,
        },

        new CardUnitBlueprint() {
            name="Sans",
            description="it's a beautiful day outside.birds are singing, flowers are blooming... on days like these, kids like you... Should be burning in hell.",
            faction="Undertale",
            types=[RowType.RANGE],
            power=8,
            is_hero=true,
            effects=new List<IEffect>() {new EffectClearEmptiestRow()}
        },

        new CardUnitBlueprint() {
            name="Papyrus",
            description="I WILL BE THE ONE! I MUST BE THE ONE! I WILL CAPTURE A HUMAN! THEN, I, THE GREAT PAPYRUS... WILL GET ALL THE THINGS I UTTERLY DESERVE!",
            faction="Undertale",
            types=[RowType.MELEE],
            power=3,
            effects=new List<IEffect>() {new EffectSetNthMultPower()}
        },

        new CardUnitBlueprint() {
            name="Undyne",
            description="Now, human! Let's end this, right here, right now. I'll show you how determined monsters can be! Step forward when you're ready! Fuhuhuhu!",
            faction="Undertale",
            types=[RowType.MELEE,RowType.RANGE],
            power=5
        },

        new CardUnitBlueprint() {
            name="Undyne the Undying",
            description="SCREW IT! WHY SHOULD I TELL THAT STORY WHEN YOU'RE ABOUT TO DIE!?! NGAAAHHHH!",
            faction="Undertale",
            types=[RowType.MELEE],
            power=7,
            is_hero=true,
            effects=new List<IEffect>() {new EffectRemoveStrongest()}
        },

        new CardUnitBlueprint() {
            name="Mettaton",
            description="I'M NOT GOING TO DESTROY YOU WITHOUT A LIVE TELEVISION AUDIENCE!!",
            faction="Undertale",
            types=[RowType.RANGE],
            power=4,
            effects=new List<IEffect>() {new EffectSetBoost(RowType.RANGE)}
        },

        new CardUnitBlueprint() {
            name="Asriel",
            description="Don't kill, and don't be killed, alright? That's the best you can strive for.",
            faction="Undertale",
            types=[RowType.SIEGE],
            power=6
        },

        new CardUnitBlueprint() {
            name="Photoshop Flowey",
            description="In this world it's kill or be killed. This is all just a bad dream...And you're NEVER waking up!",
            faction="Undertale",
            types=[RowType.SIEGE],
            power=9,
            is_hero=true,
            effects=new List<IEffect>() {new EffectDrawCard()}
        },

        new CardDecoyBlueprint() {
            name="Flowey",
            description="Howdy! I'm FLOWEY. FLOWEY the FLOWER!",
            faction="Undertale",
            types=[RowType.MELEE]
        },

        new CardDecoyBlueprint() {
            name="Annoying Dog",
            description="I'm not snoring, I'm cheering you on in my sleep!! ...Oh, you're still here? Don't you have anything better to do?",
            faction="Undertale",
            types=[RowType.MELEE,RowType.RANGE,RowType.SIEGE]
        },

        new CardWeatherBlueprint() {
            name="Muffet",
            description="Don't look so blue, my deary~... I think purple is a better look on you! Ahuhuhu~",
            faction="Undertale",
            types=[RowType.MELEE]
        },

        new CardWeatherBlueprint() {
            name="It's Raining Somewhere",
            description="Someone who sincerely likes bad jokes... has an integrity you can't say \"no\" to.",
            faction="Undertale",
            types=[RowType.RANGE]
        },

        new CardWeatherBlueprint() {
            name="Amalgam",
            description="Welcome to my special hell.",
            faction="Undertale",
            types=[RowType.SIEGE]
        },

        new CardDispelBlueprint() {
            name="Barrier",
            description="This is the barrier. This is what keeps us all trapped underground.",
            faction="Undertale",
        },

        new CardBoostBlueprint() {
            name="Soul",
            description="See that heart? That is your SOUL, the very culmination of your being!",
            faction="Undertale",
            types=[RowType.MELEE],
            bonus=1
        },

        new CardBoostBlueprint() {
            name="Death by Glamour",
            description="Lights! Camera! Action!",
            faction="Undertale",
            types=[RowType.RANGE],
            bonus=1
        },

        new CardBoostBlueprint() {
            name="Temmie",
            description="hOI!!! i'm TEMMIE!!",
            faction="Undertale",
            types=[RowType.SIEGE],
            bonus=1
        }
    };

    public static void LoadContent(GameTools gt) {
        foreach (var bp in blueprints) {
            bp.LoadContent(gt);
        }
    }
}

public struct DecksDump {

    private const int DEFAULT_DECK_INDEX = 0;
    public static Dictionary<string,Deck> decks = new();
    public static int Count {get => decks.Count();}

    public static Deck Generate(string faction) {
        Dictionary<CardBlueprint,int> cards = new();
        CardLeaderBlueprint leader = null;

        foreach (var c in CardsDump.blueprints) {
            if (
                c.faction == faction
                || c.faction == Card.NEUTRAL_FACTION
            ) {
                int qtty = 0;
                switch (c) {
                    case CardLeaderBlueprint l:
                        leader = l;
                        break;
                    case CardUnitBlueprint u:
                        var has_effect = u.effects.Count() != 0;
                        if (u.is_hero) {
                            qtty = 1;
                            continue;
                        } else {
                            if (u.power < 4) qtty = 3;
                            else if (4 <= u.power && u.power < 5) {
                                if (!has_effect) qtty = 3;
                                else qtty = 2;
                            }
                            else {
                                if (!has_effect) qtty = 2;
                                else qtty = 1;
                            }
                        }
                        break;
                    case CardDecoyBlueprint d:
                        qtty = 1;
                        break;
                    case CardWeatherBlueprint w:
                        qtty = 1;
                        break;
                    case CardDispelBlueprint d:
                        qtty = 1;
                        break;
                    case CardBoostBlueprint b:
                        qtty = 1;
                        break;
                    default:
                        throw new Exception();
                };
                if (c is not CardLeaderBlueprint)
                    cards[c] = qtty;
            }
        }

        if (leader is null) throw new Exception($"Leader not found for faction: {faction}.");

        return new Deck(
            cards,
            leader
        );
    }

    public static Deck GetDeck(int index=DEFAULT_DECK_INDEX) {
        return decks.Values.ToArray()[index];
    }

    public static void Initialize() {
        foreach (var bp in CardsDump.blueprints) {
            if (
                bp.faction != Card.NEUTRAL_FACTION
                && !decks.Keys.Contains(bp.faction)
            ) {
                decks[bp.faction] = Generate(bp.faction);
            }
        }
        foreach (var deck in decks.Values) {
            deck.Populate();
        }
    }

    public static void LoadContent(GameTools gt) {
        foreach (var deck in decks.Values) {
            deck.LoadContent(gt);
        }
    }

}

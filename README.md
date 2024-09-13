# MonoGwent
MonoGwent is a card game similar to the Gwent Card Game from The Witcher 3. It is developed in C# language, with also using the Monogame framework.

# User Guide
MonoGwent is a two-players card game. Each player is given a deck which they will use to play cards in the board in order to accumulate power and win rounds.

## Key Bindings
| Key | Functionality |
| :---: | :--- |
| Esc | Exit game |
| F1 | Open/Close help |
| F2 | (Un)Mute music |
| F3 | Open script editor |
| F4 | Toggle Fullscreen/Window mode |
| F12 | Full restart |
| Arrow Keys | Move cursor |
| Enter | Accept/Use |
| Backs | Cancel/Return |
| Right Shift | Select Leader |
| Tab | Pass turn |

## Deck Composition
Decks are filled with lots of different cards. Each decks is its own faction, where all cards in it belong either to that faction, or the "Neutral" faction.

The existing card types are:
- **Unit**: these are the main type, as they add power to the player's overall. Unit cards can be played in any of the fields whose type is specified in the types of the card. The field types are Melee, Range and Siege. These can be:
    * **Silver**: decks can have up to 3 copies of these cards.
    * **Gold (Hero)**: decks can only have 1 copy of these cards. They are also unaffected by any effect.
- **Weather**: after being placed in any of the fields whose type is specified in the types of the card, all cards' power in this field for both players are decreased.
- **Dispel**: cancel one Weather in any of the fields whose type is specified in the types of the card, or all Weathers at once.
- **Boost**: increase the power of all cards in a single one of any of the fields whose type is specified in the types of the card.
- **Decoy**: takes the place of any card placed in any of the fields whose type is specified in the types of the card. These cards have no power.
- **Leader**: decks must have only 1 leader card. Has a special effect which can be used once per game.

From all the existing cards, decks for all factions (except "Neutral" faction) are automatically generated and filled with cards according to the following steps. For each card of either the deck faction or "Neutral" faction:

| Card Type | Power | Has Effect | Quantity |
|:---|:---:|:---:|:---:|
| Leader | - | - | 1 |
| Unit Silver | up to 3 | - | 3 |
| ... | 4 to 5 | No | 3 |
| ... | 4 to 5 | Yes | 2 |
| ... | 6 and on | No | 2 |
| ... | 6 and on | Yes | 1 |
| Unit Gold | - | - | 1 |
| Weather | - | - | 1 |
| Dispel | - | - | 1 |
| Boost | - | - | 1 |
| Decoy | - | - | 1 |

## Game Board
The board has:
- 2 Player Fields: Where each player will play their cards.
    * 1 Deck slot: deck is placed here.
    * 1 Graveyard slot: discarded cards are sent here.
    * 3 Field rows of each type: unit cards are placed here.
    * 3 Boost slots of each type. boost cards are placed here.
    * 1 Leader slot: the deck's leader card is placed here.
- 3 Weather slots of each type: weather cards are placed here.

## Game Flow

### Startup
Both players draw 10 cards each. This is the maximum amount they can have at any time. If any card is drawn while having this many already, they are discarded. The first player is randomly selected. Then, they can return up to 2 cards to the deck in order to draw again as many cards as returned.

### Play
Players play rounds to determine the winner. Each round, players take turns to play they cards. In a turn, a player can either use a card, use the leader's effect or pass. If a player passes, they can no longer play in the current round. When both players have passed, the one with the highest power is the round's winner. If both players have the same power at the end of a round, the round is a draw (both won). When starting a new round, be it the second or third round, both players' fields are cleared, including the weathers, and then they draw 2 cards, before starting to play the round.

### End
When any player has won twice, they win the game. If both players ended up with two round draws (both won twice), the game is a draw.

## Script
You may create your own effects and cards. Refer to the respective Script section in the Developer Guide section. Note that for a faction to be recognized and a deck to be created, it **MUST** have a leader card of such faction.

# Developer Guide

## Source Code
MonoGwent runs on [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0), written in [C# 12](https://dotnet.microsoft.com/languages/csharp) and uses the [Monogame Framework 3.8.1](https://docs.monogame.net/).
The source code is available here on GitHub:
```
git clone https://github.com/DP-404/MonoGwent.git
```
Then build and run:
```
dotnet restore
dotnet run
```

## Workflow
Within the `Gwent.cs`, the game is run through an instance of the `Gwent` class (child of `Microsoft.Xna.Framework.Game`, the heart of any Monogame project). This has 4 main methods:
- Game Constructor: tells the project how to start.
- `Initialize`: initialize the game upon its startup.
- `Load` and `Unload` Content: used to add and remove assets from the running game from the Content project. This is called before running `Update` and `Draw`, so game assets to be used should be loaded before the method termination.
- `Update`: called on a regular interval to update the game state, e.g. take player inputs or animate entities.
- `Draw`: called on a regular interval to take the current game state and draw the game entities to the screen.

Note: To know how Monogame projects work, please refer to the [Monogame Official Documentation](https://docs.monogame.net/).

## Project Setup
- `/`: settings and entry point file.
- `/src`: game code.
    - `Gwent.cs`: the `Gwent` class that represents the game is here. Inheriting from `Microsoft.Xna.Framework.Game`, it includes all sorts of utilities to develop and run a game. Application logic should be put here.
- `/src/BattleManager`: game logic code.
    - `/Card`: defines the base `Card` class, from which all the other card classes inherit. There is also the `CardUnit` (represents Silver/Gold unit cards and Decoy cards), `CardLeader`, `CardWeather` (represents Weather cards and Dispel cards) and `CardBoost`.
    - `/Effect`: defines the base `IEffect` interface, from which all the other card effects inherit. Effects are first called their `Eval()` method, which returns wether the effect should be activated, then the `Use()` method has the actual effect actions.
    - `/Scene`: defines the base `IScene` interface, from which all the other scenes inherit. Scenes logic is in the `Update()` method, which updated the game state according to different moments of the game. 
    - `BattleManager.cs` (partial `BattleManagerDraw.cs`): the core of the battle system and most likely the whole game, since all the `Update()` and `Draw()` logic is called through this root. The BM `Update()` method evaluates according to the current `IScene`, which calls a specific `Update()` method, this has different conditions with the `Cursor` indexes, the current/rival `Player` state and the `Keyboard` inputs to finally take the actions in the game, whereas the `Draw` while using the game status draws on screen.
    - `Player.cs`: defines the `Player` entity class, with all the required game fields such as `name`,`deck`, `hand`, `graveyard`, etc... Although a `Player` object does not have an `Update()` method, it does have a `Draw()` method. In-game, the fields flip according to the currently playing player in order to show their field in the lower part of the battlefield. Therefore, its drawing methods, which include the positions of all drawn assets, are relative to the `is_turn` bool (meaning, if `is_turn` is true, the field is drawn below).
- `/src/DeckManager`: game cards and decks code.
    - `/CardBlueprint`: defines the base `CardBlueprint` class, from which all the other card blueprint classes inherit. There is a blueprint class for every different card type, which can be used to make the actual `Card` objects that represent the actual cards.
    - `Deck.cs`: defines the `Deck` class, used by the `Player` class.
    - `Dumps.cs`: cards and decks are loaded, generated and initialized here. `CardsDump` stores all created `CardBlueprint`s, while `DecksDump` stores the created `Deck`.
- `/src/ScriptReader`: game script reader code.
    - `/Lexer`: defines the `Lexer` class, used for generating the `Token`s off the script text file.
    - `/Parser`:
        - `Parser.cs`: defines the `Parser` class, which receives the `Token`s generated and creates the `Effect`s and `Card`s according to the script.
        - `Effect.cs`: defines the `Effect` class, which represents the effects created through script. This also inherits from the `IEffect` class. It reads the `Statement`s created by the `Parser` and performs the actions.
        - `CardMaker.cs`: defines the `CardMaker` class, which is used to create `CardBlueprint`s of the cards created through script.
        - `Context.cs`: defines the `Context` class, an utility for the `Effect`s to access the game state of the `BattleManager`.
- `/Content`: game assets. Read the respective section.

## Content
Content is placed on the `Content` folder and managed through the [MonoGame Content Builder Tool (MGCB Editor)](https://docs.monogame.net/articles/tools/mgcb_editor.html).

- `font`: store game `spritefont` files.
- `graphics`: store game image files (`.png`,`.jpg`,`.jpeg`,etc...)
    * `cards`: store only decks cards.
    * `img`: store all other game related images.
- `music`: store game music files (`.mp3`,`.ogg`,`.wav`,etc...)
    * Note: `Song` files should be named `bgm_backgroundmusicname.ogg`
    * Note: `SoundEffect` files should be named `sfx_soundeffectname.ogg`
- `script.txt`: additional and external game script. Read the respective section. 

## Script
If you press the script editor key in-game, a text editor will be opened, allowing you to edit the script text file, where you will be able to create your own deck using a specific script language. The script file is located in `MonoGwent/Content/script.txt`. Script errors in this file when launching the game will be recorded in `MonoGwent/scripterror.txt`. Missing or empty `script.txt` will be skipped and game will launch normally.

### Example
```
    effect {
        Name: "Damage",
        Params: {
            Amount: Number
        },
        Action: (targets, context) => {
            for target in targets {
                i = 0;
                while (i++ < Amount)
                    target.Power -= 1;
            };
        }
    }

    effect {
        Name: "Draw",
        Action: (targets, context) => {
            topCard = context.Deck.Pop();
            context.Hand.Add(topCard);
            context.Hand.Shuffle();
        }
    }

    effect {
        Name: "ReturnToDeck",
        Action: (targets, context) => {
            for target in targets {
                owner = target.Owner;
                deck = context.DeckOfPlayer(owner);
                deck.Push(target);
                deck.Shuffle();
                context.Board.Remove(target);
            };
        }
    }

    card {
        Type: "Gold",
        Name: "Beluga",
        Faction: "Northern Realms",
        Power: 10,
        Range: ["Melee", "Ranged"],
        OnActivation: [
            {
                Effect: {
                    Name: "Damage", // must be previously defined
                    Amount: 5 // ... and have these parameters
                },
                Selector: {
                    Source: "board", // or "hand", "otherHand", etc...
                    Single: false, // by default is false tho
                    Predicate: (unit) => unit.Faction == "Northern" @@ "Realms"
                },
                PostAction: {
                    Type: "ReturnToDeck",
                    Selector: { // optional inside PostAction, where it won't select again, but use the parent's
                        Source: "parent",
                        Single: false,
                        Predicate: (unit) => unit.Power < 1
                    }
                }
            },
            {
                Effect: "Draw" // plain string equals { Name: "Draw: }
            }
        ]
    }
```

### Effects (`effect`)
- **Name**: Effect name, required.
- **Params**: Parameters recived by the effect alongside the associated type (Number, String or Bool), optional.
- **Action**: The effect function. The effect Params are accessible from within the function body. The following parameters are explicitly defined:
    - `targets` is the targets list. All effects have one or many targets stated by their selector when the effect is called (seen later when explaining cards).
    - `Context` is the game context, which contains information about the game state. Has the following properties:
        - `TriggerPlayer`: returns the identifier (id) of the player who triggered the effect.
        - `Board`: returns a list with all the cards in the board.
        - `HandOfPlayer(player)`, `DeckOfPlayer(player)`, `GraveyardOfPlayer(player)` and `DeckOfPlayer(player)`: Each of these properties returns the corresponding list of the player passed as parameter. As syntactic sugar, `context.Hand` is a diminutive of `context.HandOfPlayer(context.TriggerPlayer)`. Likewise there is the same for `Deck`, `Field` and `Graveyard`. Additionally, each card has the property `card.Owner` which returns the id of the card's owner player, that is, the player whose deck this card was part of initially.
    These are the methods possessed by each of the cards lists accessible from the context:
        - `Find(predicate)`: Returns all the cards that fulfill the predicate, which is a function that receives a card and returns a boolean, eg: "`context.hand.find((card) => card.Power == 5)`" returns all the cards in the player's hand with power 5.
        - `Push(card)`: Adds a card to the top of the list.
        - `SendBottom(card)`: Adds a card to the bottom of the list.
        - `Pop()`: Takes off the card on top of the list and returns it.
        - `Remove()`: Removes a card from the list.
        - `Shuffle()`: Shuffles the list.

### Cards (`card`)
- **Type**: The card type (can be "Gold", "Silver", "Weather", "Boost", "Leader", etc...).
- **Name**: The card name.
- **Faction**: The card faction, ideally all cards of a deck belong to the same faction.
- **Power**: The card power or points. Weather, Boost and Leader have `Power = 0`.
- **Range**: An array of possible card ranges. Accepts `"Melee"`, `"Ranged"` or `"Siege"`.
- **OnActivation**: A list of effects that execute in sequence when the card is placed in the field. In the effect call there are some properties:
    - **Effect**: An object that carries the name of the effect to use (must have been previously declared with `effect`) and the parameters that it receives, each as a different property. As syntactic sugar, a plain string equals to `Effect: { Name: "EffectName"}` (eg: `{ Effect : "Draw" }`).
    - **Selector**: A cards filter to which the effect will be applied (which in the definition will be the `targets`). The selector is built out of the following properties:
        - **Source**: The source from where the cards are taken, valid sources are: "`hand`", "`otherHand`", "`deck`", "`otherDeck`", "`field`", "`otherField`" or "`parent`". Particularly, the `"parent"` source can only be specified as a source in a PostAction (explained further on), and means that its source will be the `targets` list built by the effect of which it is its PostAction.
        - **Single**: A boolean which tells whether only the first value from the search will be taken (still a list but with a single element) or if all cards that fulfill the predicate will be selected.
        - **Predicate**: The filter itself, is a function that receives a card and returns a boolean stating wether it should be taken.
    - **PostAction**: Another effect declaration, optional, to be called after the first effect is done (wich in turn can have another PostAction). In a PostAction, the Selector property is optional and in case of be skipped the same list of its "parent" effect will be used as `targets`. In case of wanting to filter a subset of the parent's targets then a Selector with `Source: "parent"` and a Predicate will be defined as desired. A capable eye notices that it's virtually the same to put `n` effect in OnActivation than one after another as PostAction. Putting as PostAction allows however to perform nested behaviors that depend on the parent's targets.

### Language
- Not allowed comments (skip the `// comment` parts of the example).
- Allowed operators: aritmetic (`+`, `-`, `*`, `/`, `^`, `++`), logic (`&&`, `||`), comparison (`<`, `>`, `==`, `>=`, `<=`), string concatenation (`@`, `@@`) or assignment (`=`).
    > The `@@` operators includes a whitespace between the strings to concatenate.
- Allowed constants and variables declarations, (eg: `temp = 5`).
- Allowed access to properties either to the context (`context.Hand`) as well as a card (`card.Power`).
- Allowed lists indexing (eg: `context.Hand.Find((card) => true)[0]`).
- Allowed lists cycles (`for` and `while`) (eg: `for target in targets`, `while (i < count)`).
- The functions and cycles bodies can be either a statement as well as an expressions block. An expressions block is to state some expressions between curly brackets and finished with `;` (as shown in the `Action` examples of the effects). A single expression is seen in the `Predicate` example of the selector in the card.

## TODO
### Gameplay
- [x] Game can be won.
- [x] Unit cards.
    * [x] Silver cards.
    * [x] Gold cards.
    * [x] Special effects.
- [x] Leader cards.
- [x] Additional leader cards effects.
- [x] Additional unit cards effects.
- [x] Weather cards.
- [x] Dispel cards.
- [x] Boost cards.
- [x] Decoy cards.
- [ ] Rival AI.
- [x] Script interpreter.
### QoL
- [x] Enhance GUI.
- [x] Build full fledged decks.
- [x] Deck selection scene.
- [x] Add music.
- [x] Add sfx.

# Credits
Developed by: [DP-404](https://github.com/DP-404)
Made with: [MonoGame](https://docs.monogame.net/)

All assets used in this project belong to their respective owners.

Licenced under MIT Licence
Copyright (c) 2024 DP-404

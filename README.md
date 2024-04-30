# MonoGwent
MonoGwent is a card game similar to the Gwent Card Game from The Witcher 3. It is developed in C# language, with also using the Monogame framework.

# User Guide
MonoGwent is a two-players card game. Each player is given a deck which they will use to play cards in the board in order to accumulate power and win rounds.

## Key Bindings
- Esc: exit game
- F1: open/close help
- F2: un/mute music
- F4: toggle Fullscreen/Window mode
- Arrow Keys: move
- Enter: accept/use
- Backs: cancel/return
- Right Shift: select Leader
- Tab: pass

## Deck Composition
A deck is composed by +25 cards of different types:
- **Unit**: these are the main type, as they add power to the player's overall. Unit cards can be played in any of the fields whose type is specified in the types of the card. The field types are Melee, Range and Siege. These can be:
    * **Silver**: decks can have up to 3 copies of these cards.
    * **Golden (Hero)**: decks can only have 1 copy of these cards. They are also unaffected by any effect.
- **Weather**: after being placed in any of the fields whose type is specified in the types of the card, all cards' power in this field for both players are decreased.
- **Dispel**: cancel one Weather in any of the fields whose type is specified in the types of the card, or all Weathers at once.
- **Boost**: increase the power of all cards in a single one of any of the fields whose type is specified in the types of the card.
- **Decoy**: takes the place of any card placed in any of the fields whose type is specified in the types of the card. These cards have no power.
- **Leader**: decks must have only 1 leader card. Has a special effect which can be used once per game.

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
    * `BattleManager.cs` (partial `BattleManagerUpdate.cs` and `BattleManagerDraw,cs`): the core of the battle system and most likely the whole game, since all the `Update` and `Draw` logic is called through this root. The BM `Update` method evaluates according to the current `Scene`, which calls a specific `UpdateScene` method, this has different conditions with the `Cursor` indexes, the current/rival `Player` state and the `Keyboard` inputs to finally take the actions in the game, whereas the `Draw` while using the game status draws on screen.
    * `DeckManager.cs`: cards and decks are loaded, generated and initialized here. `CardsDump` stores all created `CardBlueprint` (something like a plan to create the actual `Card` objects), while `DecksDump` stores the created `Deck`.
    * `Player.cs`: defines the `Player` entity, with all the required game fields such as `name`,`deck`, `hand`, `graveyard`, etc.... Although a `Player` object does not have an `Update` method, it does have a `Draw` method. In-game, the fields flip according to the currently playing player in order to show their field in the lower part of the battlefield. Therefore, its drawing methods, which include the positions of all drawn assets, are relative to the `is_turn` bool (meaning, if `is_turn` is true, the field is drawn below).
    * `Card.cs`: defines the base `Card` class, from which all the other card classes inherit. There is also the `CardUnit` (represents Silver/Golden unit cards and Decoy cards), `CardLeader`, `CardWeather` (represents Weather cards and Dispel cards) and `CardBoost`.
    * `Gwent.cs`: the `Gwent` class that represents the game is here. Inheriting from `Microsoft.Xna.Framework.Game`, it includes all sorts of utilities to develop and run a game. Application logic should be put here.
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


## TODO
### Gameplay
- [x] Game can be won.
- [x] Unit cards.
    * [x] Silver cards.
    * [x] Golden cards.
    * [x] Special effects.
- [x] Leader cards.
- [x] Additional leader cards effects.
- [x] Weather cards.
- [x] Dispel cards.
- [x] Boost cards.
- [x] Decoy cards.
- [ ] Add AI.
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

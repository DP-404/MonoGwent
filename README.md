# MonoGwent
MonoGwent is a card game similar to the Gwent Card Game from The Witcher 3. It is developed in C# language, with also using the Monogame framework.

# User Guide
MonoGwent is a two-players card game. Each player is given a deck which they will use to play cards in the board in order to accumulate power and win rounds.

## Deck Composition
A deck is composed by +25 cards of different types:
- **Unit**: these are the main type, as they add power to the player's overall. Unit cards can be played in any of the fields whose type is specified in the types of the card. The field types are Melee, Range and Siege. These can be:
    * **Silver**: decks can have up to 3 copies of these cards.
    * **Gold (Hero)**: decks can only have 1 copy of these cards. They are also unaffected by any effect.
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

## Project Setup
- `/`: settings and entry point file.
- `/src`: code.
- `/Content`: assets

## Workflow
Within the `Gwent.cs`, the game is run through an instance of the `Gwent` class (child of `Microsoft.Xna.Framework.Game`, the heart of any Monogame project). This has 4 main methods:
- Game Constructor: tells the project how to start.
- `Initialize`: initialize the game upon its startup.
- `Load` and `Unload` Content: used to add and remove assets from the running game from the Content project.
- `Update`: called on a regular interval to update your game state, e.g. take player inputs or animate entities.
- `Draw`: called on a regular interval to take the current game state and draw the game entities to the screen.

## Content
Content is managed through the [MonoGame Content Builder Tool (MGCB Editor)](https://docs.monogame.net/articles/tools/mgcb_editor.html).
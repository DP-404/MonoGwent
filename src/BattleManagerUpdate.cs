using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MonoGwent;

public partial class BattleManager
{

    private void NewGame() {
        MediaPlayer.Play(bgm_playing1);
        scene = Scene.START_GAME;
    }
    private void StartGame() {
        ClearAllWeathers();
        InitializePlayers();
        phase = REDRAW_PHASE;
        scene = Scene.START_TURN;
        cursor.Move(Section.HAND, false);
        victor = null;
    }
    private void Redraw() {
        scene = Scene.REDRAW;
    }
    private void StartPhase() {
        scene = Scene.START_PHASE;
        if (phase != REDRAW_PHASE) {
            victor = null;
            ClearAllWeathers();
            player_1.Clear();
            player_2.Clear();
            player_1.ReceiveCard(Player.PHASE_DRAW_CARDS);
            player_2.ReceiveCard(Player.PHASE_DRAW_CARDS);
        }
        phase += 1;
        switch (phase) {
            case 2:
                MediaPlayer.Play(bgm_playing2);
                break;
            case 3:
                MediaPlayer.Play(bgm_playing3);
                break;
        }
    }
    private void StartTurn() {
        if (!rival_player.has_passed) current_player = rival_player;
        cursor.Move(Section.HAND);
        scene = Scene.START_TURN;
    }
    private void PlayTurn() {
        // If not Pass > Start next turn
        if (phase == REDRAW_PHASE) 
            {scene = Scene.REDRAW;}
        else
            {scene = Scene.PLAY_TURN;}
    }
    private void EndTurn() {
        scene = Scene.END_TURN;
    }
    private void EndPhase() {
        if (phase != REDRAW_PHASE) {
            // Determine victor
            if (player_1.GetPower(weathers) > player_2.GetPower(weathers)) {
                victor = player_1;
            }
            else if (player_1.GetPower(weathers) < player_2.GetPower(weathers)) {
                victor = player_2;
            }

            // If Draw, check leader effect
            if (victor is null) {
                if (player_1.leader.effect == LeaderEffect.WIN_ON_DRAW && !player_1.leader.used) {
                    player_1.leader.used = true;
                    victor = player_1;
                }
                if (player_2.leader.effect == LeaderEffect.WIN_ON_DRAW && !player_2.leader.used) {
                    player_2.leader.used = true;
                    if (victor is null) {
                        victor = player_2;
                    }
                    else {victor = null;}
                }
            }

            // Reduce health
            if (player_1 != victor) player_1.health -= 1;
            if (player_2 != victor) player_2.health -= 1;
            if (victor is not null) current_player = GetOtherPlayer(victor);
        }
        scene = Scene.END_PHASE;
    }
    private void EndGame() {
        if (player_1.IsDefeated() && !player_2.IsDefeated()) {victor = player_2;}
        else if (player_2.IsDefeated() && !player_1.IsDefeated()) {victor = player_1;}
        else {victor = null;}
        scene = Scene.END_GAME;
    }

    private void PlayCard() {
        var card = current_player.GetHandCard(cursor.hand);
        RowType field;
        if (cursor.field == Cursor.NONE) {
            field = (RowType)cursor.index;
        } else {
            field = (RowType)cursor.field;
        }

        // If Field is not valid > Return
        if (!card.types.Contains(field) && card.types.Length != 0) return;

        switch (card) {

            case CardUnit:
                if (((CardUnit)card).is_decoy) {
                    // Take field card to hand
                    var takeback_card = (CardUnit)current_player.GetFieldCard((RowType)cursor.field, cursor.index);
                    current_player.rows[(RowType)cursor.field].Remove(takeback_card);
                    current_player.hand.Add(takeback_card);

                    // Place card on field card's place
                    current_player.rows[(RowType)cursor.field].Insert(cursor.index, (CardUnit)card);
                    current_player.hand.Remove(card);
                    break;
                } else {
                    // Place card on field
                    current_player.hand.Remove(card);
                    current_player.rows[(RowType)cursor.index].Add((CardUnit)card);
                    break;
                }

            case CardWeather:
                // Card is Weather
                if (!((CardWeather)card).is_dispel) {
                    current_player.hand.Remove(card);
                    var old_weather = weathers[field];
                    if (old_weather.Item1 is not null) {
                        old_weather.Item2.graveyard.Add(old_weather.Item1);
                    }
                    weathers[field] = new ((CardWeather)card, current_player);
                }
                // Card is Dispel
                else {
                    var exists_weather = false;
                    var is_single = card.types.Length != 0;
                    if (is_single) {
                        exists_weather = weathers[field].Item1 is not null;
                    } else {
                        foreach (var row in Enum.GetValues(typeof(RowType)).Cast<RowType>()) {
                            if (weathers[row].Item1 is not null) {
                                exists_weather = true;
                                break;
                            }
                        }
                    }

                    // Exists Weather > Dispel
                    if (!exists_weather) {return;}

                    current_player.hand.Remove(card);
                    current_player.graveyard.Add(card);

                    // Dispel All
                    if (card.types.Length == 0) {
                        ClearAllWeathers();
                    }
                    // Dispel Single
                    else {
                        ClearWeather(field);
                    }

                }
                break;

            case CardBoost:
                var current_boost = current_player.boosts[(RowType)cursor.index];
                if (current_boost is not null) {
                    current_player.graveyard.Add(current_boost);
                }
                current_player.hand.Remove(card);
                current_player.boosts[(RowType)cursor.index] = (CardBoost)card;
                break;

            default:
                throw new Exception();
        }

        if (card is CardUnit) {
            var card_unit = (CardUnit)card;
            if (card_unit.effect is UnitEffect.DRAW_CARD) current_player.ReceiveCard();
        }

        sfx_playcard.Play();

        cursor.Move(Section.HAND);

        EndTurn();
    }

    private void UseLeaderEffect() {
        if (current_player.leader.used) return;

        switch (current_player.leader.effect) {
            case LeaderEffect.DRAW_CARD:
                current_player.ReceiveCard(1);
                break;
            default:
                return;
        }
        current_player.leader.used = true;

        cursor.Move(Section.HAND);

        EndTurn();
    }

    private void ClearWeather(RowType row) {
        if (weathers[row].Item1 is not null) {
            weathers[row].Item2.graveyard.Add(weathers[row].Item1);
            weathers[row] = new(null,null);
        }
    }

    private void ClearAllWeathers() {
        foreach (var row in Enum.GetValues(typeof(RowType)).Cast<RowType>())
            ClearWeather(row);
    }

    private void UpdateHelp() {
        // Await for input > Toggle Help
        if (
            !cursor.holding &&
            Keyboard.GetState().IsKeyDown(Keys.F1)
        ) {
            cursor.Hold();
            help = !help;
        }
    }
    private void UpdateMediaPlayer() {
        // Await for input > Un/Mute music
        if (
            !cursor.holding &&
            Keyboard.GetState().IsKeyDown(Keys.M)
        ) {
            cursor.Hold();
            MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
        }
    }
    private void UpdateStartGame() {
        // Await for input > Start new game
        if (
            !cursor.holding &&
            Keyboard.GetState().IsKeyDown(Keys.Enter)
        ) {
            cursor.Hold();
            StartGame();
        }
    }
    private void UpdateRedraw() {
        // Await for input > Draw card
        if (
            !cursor.holding &&
            Keyboard.GetState().IsKeyDown(Keys.Enter)
        ) {
            cursor.Hold();
            var selected_card = current_player.hand[cursor.index];
            if (!current_player.selected.Contains(selected_card)) {
                if (current_player.selected.Count != 2) {
                    current_player.selected.Add(current_player.hand[cursor.index]);
                }
            } else {
                current_player.selected.Remove(selected_card);
            }
        }

        // Await for input > Finish redrawing
        else if (
            !cursor.holding &&
            Keyboard.GetState().IsKeyDown(Keys.Tab)
        ) {
            if (rival_player.has_passed) {
                foreach (var player in players) {
                    player.ReceiveCard(player.selected.Count, true);
                    foreach (var card in player.selected) {
                        player.deck.Add(card);
                        player.deck.Shuffle();
                        player.hand.Remove(card);
                    }
                    player.selected.Clear();
                    player.has_passed = false;
                }
                EndPhase();
            } else {
                current_player.has_passed = true;
                EndTurn();
            }
        }

        // Move Right
        else if (
            !cursor.holding &&
            current_player.hand.Count != 0 &&
            Keyboard.GetState().IsKeyDown(Keys.Right)
        ) {
            if (cursor.index == current_player.hand.Count-1)
            {cursor.Move(0);} else {cursor.Move(cursor.index+1);}
        }
        // Move Left
        else if (
            !cursor.holding &&
            current_player.hand.Count != 0 &&
            Keyboard.GetState().IsKeyDown(Keys.Left)
        ) {
            if (cursor.index == 0)
            {cursor.Move(current_player.hand.Count-1);} else {cursor.Move(cursor.index-1);}
        }
    }

    private void UpdateStartPhase() {
        // Await for input > Start turn
        if (
            !cursor.holding &&
            Keyboard.GetState().IsKeyDown(Keys.Enter)
        ) {
            cursor.Hold();
            StartTurn();
        }
    }

    private void UpdateStartTurn() {
        // Await for input > Play turn
        if ( 
            !cursor.holding &&
            Keyboard.GetState().IsKeyDown(Keys.Enter)
        ) {
            cursor.Hold();
            PlayTurn();
        }
    }

    private void UpdatePlayTurn() {
        Card hand_card;

        // Cursor in hand
        switch (cursor.section) {

            case Section.HAND:
                // Pass turn
                if (
                    scene != Scene.END_TURN &&
                    Keyboard.GetState().IsKeyDown(Keys.Tab)
                ) {
                    current_player.has_passed = true;
                    EndTurn();
                }
                // Move Right
                else if (
                    !cursor.holding &&
                    cursor.section == Section.HAND &&
                    current_player.hand.Count != 0 &&
                    Keyboard.GetState().IsKeyDown(Keys.Right)
                ) {
                    if (cursor.index == current_player.hand.Count-1)
                    {cursor.Move(0);} else {cursor.Move(cursor.index+1);}
                }
                // Move Left
                else if (
                    !cursor.holding &&
                    cursor.section == Section.HAND &&
                    current_player.hand.Count != 0 &&
                    Keyboard.GetState().IsKeyDown(Keys.Left)
                ) {
                    if (cursor.index == 0)
                    {cursor.Move(current_player.hand.Count-1);} else {cursor.Move(cursor.index-1);}
                }
                // Select Card > Move to Field
                else if (
                    !cursor.holding &&
                    cursor.section == Section.HAND &&
                    current_player.hand.Count != 0 &&
                    Keyboard.GetState().IsKeyDown(Keys.Enter)
                ) {
                    cursor.hand = cursor.index;
                    cursor.Move(Section.FIELD);
                }
                // Select Leader
                else if (
                    !cursor.holding &&
                    cursor.section == Section.HAND &&
                    Keyboard.GetState().IsKeyDown(Keys.RightShift)
                ) {
                    cursor.Move(Section.LEADER);
                }

                break;

            // Cursor in Field
            case Section.FIELD:
                hand_card = current_player.GetHandCard(cursor.hand);

                // Move Down/Right
                if (
                    !cursor.holding &&
                    (
                        (
                            hand_card is not CardWeather &&
                            Keyboard.GetState().IsKeyDown(Keys.Down)
                        )
                            ||
                        (
                            hand_card is CardWeather &&
                            Keyboard.GetState().IsKeyDown(Keys.Right)
                        )
                    )
                ) {
                    if (cursor.index == Enum.GetNames(typeof(RowType)).Length-1)
                    {cursor.Move(0);} else {cursor.Move(cursor.index+1);}
                }
                // Move Up/Left
                else if (
                    !cursor.holding &&
                    (
                        (
                            hand_card is not CardWeather &&
                            Keyboard.GetState().IsKeyDown(Keys.Up)
                        )
                            ||
                        (
                            hand_card is CardWeather &&
                            Keyboard.GetState().IsKeyDown(Keys.Left)
                        )
                    )
                ) {
                    if (cursor.index == 0)
                    {cursor.Move(Enum.GetNames(typeof(RowType)).Length-1);} else {cursor.Move(cursor.index-1);}
                }

                // Selected card is Unit and no Field selected
                else if (
                    !cursor.holding &&
                    hand_card is CardUnit &&
                    cursor.field == Cursor.NONE &&
                    Keyboard.GetState().IsKeyDown(Keys.Enter)
                ) {
                    // Selected card is Decoy > Move to Row
                    if (
                        ((CardUnit)hand_card).is_decoy
                    ) {
                        if (hand_card.types.Contains((RowType)cursor.index)) {
                            cursor.field = cursor.index;
                            cursor.Move(Section.ROW);
                        }
                    }
                    // Selected card is Unit > Play Card
                    else {
                        PlayCard();
                    }
                }

                // Selected card is Weather
                else if (
                    !cursor.holding &&
                    hand_card is CardWeather &&
                    Keyboard.GetState().IsKeyDown(Keys.Enter)
                ) {
                    PlayCard();
                }

                // Selected card is Boost
                else if (
                    !cursor.holding &&
                    hand_card is CardBoost &&
                    Keyboard.GetState().IsKeyDown(Keys.Enter)
                ) {
                    PlayCard();
                }

                // Cancelling
                else if (
                    !cursor.holding &&
                    Keyboard.GetState().IsKeyDown(Keys.Back)
                ) {
                    sfx_cancel.Play();
                    cursor.Move(Section.HAND, cursor.hand);
                }
                break;

            // Cursor in Row
            case Section.ROW:
                hand_card = current_player.GetHandCard(cursor.hand);

                // Selected card is Decoy > Play Card
                if (
                    !cursor.holding &&
                    hand_card is CardUnit &&
                    ((CardUnit)hand_card).is_decoy &&
                    current_player.rows[(RowType)cursor.field].Count != 0 &&
                    !((CardUnit)current_player.GetFieldCard((RowType)cursor.field, cursor.index)).is_decoy &&
                    Keyboard.GetState().IsKeyDown(Keys.Enter)
                ) {
                    PlayCard();
                }

                // Move Right
                if (
                    !cursor.holding &&
                    Keyboard.GetState().IsKeyDown(Keys.Right)
                ) {
                    if (cursor.index == current_player.rows[(RowType)cursor.field].Count-1)
                    {cursor.Move(0);} else {cursor.Move(cursor.index+1);}
                }
                // Move Left
                else if (
                    !cursor.holding &&
                    Keyboard.GetState().IsKeyDown(Keys.Left)
                ) {
                    if (cursor.index == 0)
                    {cursor.Move(current_player.rows[(RowType)cursor.field].Count-1);} else {cursor.Move(cursor.index-1);}
                }

                // Cancelling
                else if (
                    !cursor.holding &&
                    Keyboard.GetState().IsKeyDown(Keys.Back)
                ) {
                    sfx_cancel.Play();
                    cursor.Move(Section.FIELD, cursor.field);
                }
                break;

            // Cursor in Leader
            case Section.LEADER:

                // Use Leader
                if (
                    !cursor.holding &&
                    !current_player.leader.used &&
                    Keyboard.GetState().IsKeyDown(Keys.Enter)
                ) {
                    UseLeaderEffect();
                }

                // Cancelling
                else if (
                    !cursor.holding &&
                    Keyboard.GetState().IsKeyDown(Keys.Back)
                ) {
                    sfx_cancel.Play();
                    cursor.Move(Section.HAND);
                }
                break;
        }
    }

    private void UpdateEndTurn() {
        // If not Pass or Redraw > Start next turn
        if (
            !current_player.has_passed ||
            phase == REDRAW_PHASE
        ) {
            StartTurn();
        }

        // If Pass, await for input
        else if (
            current_player.has_passed &&
            Keyboard.GetState().IsKeyDown(Keys.Enter)
        ) {
            cursor.Move(Section.HAND);
            // If both passed > End phase / Start next turn
            if (rival_player.has_passed) {
                EndPhase();
            } else {
                StartTurn();
            }
        }
    }

    private void UpdateEndPhase() {
        // Either play is defeated > Game Over
        if (
            player_1.IsDefeated() ||
            player_2.IsDefeated()
        ) {
            EndGame();
        } else {
            if (phase != REDRAW_PHASE) {
                // Await for input > Start next phase
                if (
                    !cursor.holding &&
                    Keyboard.GetState().IsKeyDown(Keys.Enter)
                ) {
                    cursor.Hold();
                    StartPhase();
                }
            } else {
                cursor.Hold();
                StartPhase();
            }
        }
    }

    private void UpdateEndGame() {
        // Await for input > Start new game
        if (
            !cursor.holding &&
            Keyboard.GetState().IsKeyDown(Keys.Enter)
        ) {
            cursor.Hold();
            NewGame();
        }
    }

    public void Update() {
        UpdateHelp();
        UpdateMediaPlayer();

        if (!help) {
            switch (scene)
            {
                case Scene.START_GAME:
                    UpdateStartGame();
                    break;
                case Scene.REDRAW:
                    UpdateRedraw();
                    break;
                case Scene.START_PHASE:
                    UpdateStartPhase();
                    break;
                case Scene.START_TURN:
                    UpdateStartTurn();
                    break;
                case Scene.PLAY_TURN:
                    UpdatePlayTurn();
                    break;
                case Scene.END_TURN:
                    UpdateEndTurn();
                    break;
                case Scene.END_PHASE:
                    UpdateEndPhase();
                    break;
                case Scene.END_GAME:
                    UpdateEndGame();
                    break;
            }
        }

        // Release Cursor
        if (
            cursor.holding &&
            Keyboard.GetState().GetPressedKeyCount() == 0
        ) cursor.Release();
    }

}

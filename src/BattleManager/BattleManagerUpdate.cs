using System;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MonoGwent;

public partial class BattleManager
{

    public void NewGame() {
        MediaPlayer.Play(bgm_playing1);
        Scene = new SceneStartGame();
        ClearAllWeathers();
        InitializePlayers();
    }
    public void StartGame() {
        phase = REDRAW_PHASE;
        scene = new SceneStartTurn();
        cursor.Move(Section.HAND, false);
        victor = null;
    }
    public void StartPhase() {
        scene = new SceneStartPhase();
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
    public void StartTurn() {
        // If not Pass > Start next turn
        if (!rival_player.has_passed) current_player = rival_player;
        cursor.Move(Section.HAND);
        scene = new SceneStartTurn();
    }
    public void PlayTurn() {
        // Redraw or Play according to phase
        if (phase == REDRAW_PHASE) 
            scene = new SceneRedraw();
        else
            scene = new ScenePlayTurn();
    }
    public void EndTurn() {
        scene = new SceneEndTurn();
    }
    public void EndPhase() {
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
        scene = new SceneEndPhase();
    }
    public void EndGame() {
        if (player_1.IsDefeated() && !player_2.IsDefeated()) {victor = player_2;}
        else if (player_2.IsDefeated() && !player_1.IsDefeated()) {victor = player_1;}
        else {victor = null;}
        MediaPlayer.Pause();
        if (victor is not null) sfx_win.Play();
        scene = new SceneEndGame();
    }

    public void PlayCard() {
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
                    var takeback_card = (CardUnit)current_player.GetFieldCard((RowType)cursor.field, cursor.index);

                    // If takeback card is Hero > Return
                    if (takeback_card.is_hero) return;

                    // Take field card to hand
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
                        exists_weather = Enum.GetValues(typeof(RowType))
                            .Cast<RowType>()
                            .Select(row => weathers[row].Item1)
                            .Any(w => w is not null);
                    }

                    if (!exists_weather) {return;}

                    current_player.hand.Remove(card);
                    current_player.graveyard.Add(card);

                    // Dispel All
                    if (card.types.Length == 0)
                    {ClearAllWeathers();}
                    // Dispel Single
                    else {ClearWeather(field);}

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

    public void UseLeaderEffect() {
        if (current_player.leader.used) return;

        switch (current_player.leader.effect) {
            case LeaderEffect.DRAW_CARD:
                current_player.ReceiveCard(1);
                break;
            case LeaderEffect.RECOVER_LAST_DISCARDED_CARD:
                if (current_player.graveyard.Count == 0) return;
                if (current_player.hand.Count >= Player.HAND_CARDS_LIMIT) return;
                if (
                    current_player.graveyard[^1] is CardUnit &&
                    ((CardUnit)current_player.graveyard[^1]).is_hero
                ) return;
                current_player.hand.Add(current_player.graveyard[^1]);
                current_player.graveyard.RemoveAt(current_player.graveyard.Count-1);
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
            Keyboard.GetState().IsKeyDown(Keys.F2)
        ) {
            cursor.Hold();
            MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
        }
    }

    public void Update() {
        UpdateHelp();
        UpdateMediaPlayer();

        if (!help) scene.Update(this);

        // Release Cursor
        if (
            cursor.holding &&
            Keyboard.GetState().GetPressedKeyCount() == 0
        ) cursor.Release();
    }

}

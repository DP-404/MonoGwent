using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGwent;

public partial class BattleManager
{
    private const int ENTER_NAME_XPOS = 360;
    private const int ENTER_NAME_YPOS = 50;
    private const string TEXT_ENTER_NAME = "Enter player name:";
    private const int NAME_XPOS = 360;
    private const int NAME_YPOS = 70;
    private const int CHOOSE_DECK_XPOS = 360;
    private const int CHOOSE_DECK_YPOS = 150;
    private const string TEXT_CHOOSE_DECK = "Use Left and Right Arrows to choose your deck:";
    private const int CHOSEN_DECK_XPOS = 360;
    private const int CHOSEN_DECK_YPOS = 170;
    private const int CHOSEN_DECK_WIDTH = 224;
    private const float CHOSEN_DECK_SCALE = (float)CHOSEN_DECK_WIDTH/Card.ACTUAL_WIDTH;
    private const int CHOSEN_DECK_NAME_XPOS = 360;
    private const int CHOSEN_DECK_NAME_YPOS = 530;

    private const int WEATHER_CARD_XPOS = 6;
    private const int WEATHER_CARD_YPOS = 320;
    private const int PREVIEW_CARD_XPOS = 800;
    private const int PREVIEW_CARD_YPOS = 0;
    private const int PREVIEW_CARD_WIDTH = 224;
    private const int PREVIEW_CARD_HEIGHT = 331;
    private static readonly Vector2 PREVIEW_CARD_SCALE
    = new Vector2((float)PREVIEW_CARD_WIDTH/Card.ACTUAL_WIDTH, (float)PREVIEW_CARD_HEIGHT/Card.ACTUAL_HEIGHT);
    private static readonly int PREVIEW_CARD_POWER_XPOS = PREVIEW_CARD_XPOS + (int)(58 * PREVIEW_CARD_SCALE.X);
    private static readonly int PREVIEW_CARD_POWER_YPOS = PREVIEW_CARD_YPOS + (int)(58 * PREVIEW_CARD_SCALE.Y);
    private const int IMAGE_TEXT_XPOS = 10;
    private const int IMAGE_TEXT_YPOS = 255;
    private const string TEXT_START_GAME = "Starting new game. You can choose up to 2 cards to redraw.";
    private const string TEXT_START_PHASE = "Starting new phase. Both fields were cleared.";
    private const string TEXT_START_TURN = "{0} turn to play.";
    private const string TEXT_END_TURN = "{0} has passed.";
    private const string TEXT_END_PHASE = "Phase ended. {0}";
    private const string TEXT_END_GAME = "Game over. {0}";
    private const string TEXT_VICTORY = "The victor is {0}!";
    private const string TEXT_DRAW = "It's a draw. Players failed to decide a victor.";
    private const string TEXT_PRESS_ENTER = "Press Enter to continue.";
    private const string TEXT_PRESS_F1 = "Press F1 for help.";
    private const string TEXT_CARD_TYPE = "Type: {0}";
    private const string TEXT_CARD_EFFECT = "Effect: {0}";
    private const string TEXT_CARD_DESCRIPTION = "{0}";
    private const int HELP_XPOS = 20;
    private const int HELP_YPOS = 20;
    private const string TEXT_HELP = @"MonoGwent Help

Both players draw 10 cards each. This is the maximum amount they can have at any time. If any card is drawn while having this many already, they are discarded. The first player is randomly selected. Then, they can return up to 2 cards to the deck in order to draw again as many cards as returned.
Players play rounds to determine the winner. Each round, players take turns to play they cards. In a turn, a player can either use a card, use the leader's effect or pass. If a player passes, they can no longer play in the current round. When both players have passed, the one with the highest power is the round's winner. If both players have the same power at the end of a round, the round is a draw (both won). When starting a new round, be it the second or third round, both players' fields are cleared, including the weathers, and then they draw 2 cards, before starting to play the round.
When any player has won twice, they win the game. If both players ended up with two round draws (both won twice), the game is a draw.

Controls:
Esc - Exit game
F1 - Open/Close help
F2 - Mute music
F4 - Toggle Fullscreen/Window mode
Arrow Keys - Move
Enter - Accept/Use
Back - Cancel/Return
Right Shift - Select Leader
Tab - Pass

Credits:
Developed by DP-404 (https://github.com/DP-404).
Made with MonoGame.
All assets used in this game belong to their respective owners.

Licenced under MIT Licence
Copyright (c) 2024 DP-404
";

    private void DrawDeckSelection(GameTools gt) {
        // Draw Background
        gt.spriteBatch.Draw(
            img_dim_background,
            new Vector2(0,0),
            null,
            Color.Black
        );

        // Draw Enter your name
        var enter_name_size = fnt_message.MeasureString(TEXT_ENTER_NAME);
        gt.spriteBatch.DrawString(
            fnt_message,
            TEXT_ENTER_NAME,
            new Vector2(
                (gt.graphics.PreferredBackBufferWidth - enter_name_size.X) / 2,
                ENTER_NAME_YPOS - enter_name_size.Y/2
            ),
            Color.White
        );

        // Draw Name
        var name_size = fnt_message.MeasureString(current_player.name);
        gt.spriteBatch.DrawString(
            fnt_message,
            current_player.name,
            new Vector2(
                (gt.graphics.PreferredBackBufferWidth - name_size.X) / 2,
                NAME_YPOS - name_size.Y/2
            ),
            Color.White
        );

        // Draw Choose deck
        var choose_deck_size = fnt_message.MeasureString(TEXT_CHOOSE_DECK);
        gt.spriteBatch.DrawString(
            fnt_message,
            TEXT_CHOOSE_DECK,
            new Vector2(
                (gt.graphics.PreferredBackBufferWidth - choose_deck_size.X) / 2,
                CHOOSE_DECK_YPOS - choose_deck_size.Y/2
            ),
            Color.White
        );

        // Draw Deck Preview
        gt.spriteBatch.Draw(
            current_player.original_deck.img,
            new Vector2(
                (gt.graphics.PreferredBackBufferWidth - CHOSEN_DECK_WIDTH) / 2,
                CHOSEN_DECK_YPOS
            ),
            null,
            Color.White,
            CHOSEN_DECK_SCALE
        );

        // Draw chosen deck name
        var chosen_deck_size = fnt_message.MeasureString(current_player.original_deck.name);
        gt.spriteBatch.DrawString(
            fnt_message,
            current_player.original_deck.name,
            new Vector2(
                (gt.graphics.PreferredBackBufferWidth - chosen_deck_size.X) / 2,
                CHOSEN_DECK_YPOS + PREVIEW_CARD_HEIGHT + chosen_deck_size.Y
            ),
            Color.White
        );

        // Draw Press Enter
        var input_size = fnt_message.MeasureString(TEXT_PRESS_ENTER);
        gt.spriteBatch.DrawString(
            fnt_message,
            TEXT_PRESS_ENTER,
            new Vector2(
                (gt.graphics.PreferredBackBufferWidth - input_size.X) / 2,
                gt.graphics.PreferredBackBufferHeight * 5 / 6
            ),
            Color.White
        );

        var help_size = fnt_message.MeasureString(TEXT_PRESS_F1);
        gt.spriteBatch.DrawString(
            fnt_message,
            TEXT_PRESS_F1,
            new Vector2(
                (gt.graphics.PreferredBackBufferWidth - help_size.X) / 2,
                gt.graphics.PreferredBackBufferHeight * 6 / 7
            ),
            Color.White
        );
    }

    private void DrawBoard(GameTools gt) {
        gt.spriteBatch.Draw(img_background, new Vector2(0,0), Color.White);
    }
    private void DrawFields(GameTools gt) {
        var show = scene is SceneRedraw || scene is ScenePlayTurn;
        player_1.Draw(gt, weathers, current_player == player_1, highscore_player == player_1, show);
        player_2.Draw(gt, weathers, current_player == player_2, highscore_player == player_2, show);

        var weather_xpos = WEATHER_CARD_XPOS;
        foreach (var row in weathers) {
            var weather = weathers[row.Key];
            if (weather.Item1 is not null) {
                gt.spriteBatch.Draw(
                    weather.Item1.img,
                    new Vector2(weather_xpos, WEATHER_CARD_YPOS),
                    null,
                    Color.White,
                    Card.THUMB_SCALE
                );
            }
            weather_xpos += Card.WIDTH;
        }
    }
    private void DrawCursor(GameTools gt) {
        Vector2 position;

        if (cursor.section == Section.HAND) {
            position = Card._GetRowPosition(
                cursor.index,
                current_player.hand.Count,
                Player.HAND_XPOS,
                Player.GetRelativePosition(Player.HAND_YPOS, Player.HAND_YPOS_OFFSET, Card.HEIGHT, true),
                Player.HAND_WIDTH
            );
            gt.spriteBatch.Draw(
                cursor.mark_card_hovered,
                position,
                Color.White
            );
        }
        else if (cursor.section == Section.FIELD) {
            var selected_card = current_player.GetHandCard(cursor.hand);

            if (selected_card is CardUnit) {
                // Highlight rows
                foreach (RowType row in Enum.GetValues(typeof(RowType))) {
                    if ((int)row == cursor.index) continue;
                    gt.spriteBatch.Draw(
                        selected_card.types.Contains(row)? cursor.mark_row_enabled : cursor.mark_row_disabled,
                        new Rectangle(
                            Player.ROW_XPOS,
                            Player.ROW_YPOS + Player.ROW_YPOS_OFFSET[row],
                            Player.ROW_WIDTH,
                            Card.HEIGHT
                        ),
                        Color.White
                    );
                }
                // Hovered row
                gt.spriteBatch.Draw(
                    selected_card.types.Contains((RowType)cursor.index)? cursor.mark_row_hovered : cursor.mark_row_hovered_disabled,
                    new Vector2(
                        Player.ROW_XPOS,
                        Player.ROW_YPOS + Player.ROW_YPOS_OFFSET[(RowType)cursor.index]
                    ),
                    Color.White
                );
            }
            else if (selected_card is CardBoost) {
                // Highlight rows
                foreach (RowType row in Enum.GetValues(typeof(RowType))) {
                    if ((int)row == cursor.index) continue;
                    gt.spriteBatch.Draw(
                        selected_card.types.Contains(row)? cursor.mark_card_enabled : cursor.mark_card_disabled,
                        new Vector2(
                            Player.BOOST_XPOS,
                            Player.BOOST_YPOS + Player.BOOST_YPOS_OFFSET[row]
                        ),
                        Color.White
                    );
                }
                // Hovered row
                gt.spriteBatch.Draw(
                    selected_card.types.Contains((RowType)cursor.index)? cursor.mark_card_hovered : cursor.mark_card_hovered_disabled,
                    new Vector2(
                        Player.BOOST_XPOS,
                        Player.GetRelativePosition(Player.BOOST_YPOS, Player.BOOST_YPOS_OFFSET[(RowType)cursor.index], 0, true)
                    ),
                    Color.White
                );
            }
            else if (selected_card is CardWeather) {
                // Highlight Weathers
                foreach (RowType row in Enum.GetValues(typeof(RowType))) {
                    if ((int)row == cursor.index) continue;
                    gt.spriteBatch.Draw(
                        (selected_card.types.Contains(row) || selected_card.types.Length == 0)?
                        cursor.mark_card_enabled : cursor.mark_card_disabled,
                        new Vector2(
                            WEATHER_CARD_XPOS + (int)row * Card.WIDTH,
                            WEATHER_CARD_YPOS
                        ),
                        Color.White
                    );
                }
                // Hovered Weather
                gt.spriteBatch.Draw(
                    (selected_card.types.Contains((RowType)cursor.index) || selected_card.types.Length == 0)?
                    cursor.mark_card_hovered : cursor.mark_card_hovered_disabled,
                    new Vector2(
                        WEATHER_CARD_XPOS + cursor.index * Card.WIDTH,
                        WEATHER_CARD_YPOS
                    ),
                    Color.White
                );
            }
            else {throw new Exception("Unsupported hovered card type.");}
        }
        else if (cursor.section == Section.ROW) {
            // Highlight Cards
            var row_type = (RowType)cursor.field;
            var row = current_player.rows[row_type];
            foreach (CardUnit card in row) {
                var index = row.IndexOf(card);
                position = card.GetRowPosition(
                    index,
                    row.Count,
                    Player.ROW_XPOS,
                    Player.ROW_YPOS + Player.ROW_YPOS_OFFSET[row_type],
                    Player.ROW_WIDTH
                );
                gt.spriteBatch.Draw(
                    card.types.Contains(row_type)? cursor.mark_card_enabled : cursor.mark_card_disabled,
                    position,
                    Color.White
                );
            }
            // Hovered Card
            position = Card._GetRowPosition(
                cursor.index,
                row.Count,
                Player.ROW_XPOS,
                Player.ROW_YPOS + Player.ROW_YPOS_OFFSET[row_type],
                Player.ROW_WIDTH
            );
            gt.spriteBatch.Draw(
                cursor.mark_card_hovered,
                position,
                Color.White
            );
        }
        else if (cursor.section == Section.LEADER) {
            gt.spriteBatch.Draw(
                !current_player.leader.used? cursor.mark_card_hovered : cursor.mark_card_hovered_disabled,
                new Vector2(
                    Player.LEADER_XPOS,
                    Player.GetRelativePosition(Player.LEADER_YPOS, Player.LEADER_YPOS_OFFSET, Card.HEIGHT, true)
                ),
                Color.White
            );
        }
    }
    private void DrawSelectedCards(GameTools gt) {
        foreach (Card card in current_player.selected) {
            var position = Card._GetRowPosition(
                current_player.hand.IndexOf(card),
                current_player.hand.Count,
                Player.HAND_XPOS,
                Player.GetRelativePosition(Player.HAND_YPOS, Player.HAND_YPOS_OFFSET, Card.HEIGHT, true),
                Player.HAND_WIDTH
            );
            gt.spriteBatch.Draw(
                cursor.mark_card_selected,
                position,
                Color.White
            );
        }
    }
    private void DrawHoveredCardInfo(GameTools gt) {
        if (
            (
                scene is SceneRedraw ||
                scene is ScenePlayTurn
            ) &&
            cursor.index != Cursor.NONE
        ) {
            void DrawHoveredCard(Card card) {
                // Draw Card
                gt.spriteBatch.Draw(
                    card.img,
                    new Vector2(PREVIEW_CARD_XPOS, PREVIEW_CARD_YPOS),
                    null,
                    Color.White,
                    PREVIEW_CARD_SCALE
                );

                // Draw Power Icon
                Texture2D img_card_type = null;
                if (card is CardUnit) {
                    Color power_color;
                    if (((CardUnit)card).is_decoy) {
                        img_card_type = Card.img_decoy;
                        power_color = Color.Black;
                    }
                    else if (((CardUnit)card).is_hero) {
                        img_card_type = Card.img_power_hero;
                        power_color = Color.White;
                    } else {
                        img_card_type = Card.img_power_normal;
                        power_color = Color.Black;
                    }
                    gt.spriteBatch.Draw(
                        img_card_type,
                        new Vector2(PREVIEW_CARD_XPOS, PREVIEW_CARD_YPOS),
                        null,
                        Color.White,
                        PREVIEW_CARD_SCALE
                    );

                    var power = ((CardUnit)card).power.ToString();
                    var power_size = fnt_message.MeasureString(power);
                    gt.spriteBatch.DrawString(
                        fnt_message,
                        power.ToString(),
                        new Vector2(PREVIEW_CARD_POWER_XPOS-power_size.X/2, PREVIEW_CARD_POWER_YPOS-power_size.Y/2),
                        power_color
                    );
                }
                else if (card is CardWeather) {
                    if (!((CardWeather)card).is_dispel) {img_card_type = Card.img_weather;}
                    else {img_card_type = Card.img_dispel;}

                    gt.spriteBatch.Draw(
                        img_card_type,
                        new Vector2(PREVIEW_CARD_XPOS, PREVIEW_CARD_YPOS),
                        null,
                        Color.White,
                        PREVIEW_CARD_SCALE
                    );
                }
                else if (card is CardBoost) {
                    img_card_type = Card.img_boost;

                    gt.spriteBatch.Draw(
                        img_card_type,
                        new Vector2(PREVIEW_CARD_XPOS, PREVIEW_CARD_YPOS),
                        null,
                        Color.White,
                        PREVIEW_CARD_SCALE
                    );
                }

                // Draw Row Types Icons
                var last_ypos = PREVIEW_CARD_YPOS + (int)((img_card_type is null)? 0 : img_card_type.Height*PREVIEW_CARD_SCALE.Y);
                foreach (RowType row in Enum.GetValues(typeof(RowType))) {
                    if (card.types.Contains(row)) {
                        gt.spriteBatch.Draw(
                            Card.img_rows[row],
                            new Vector2(PREVIEW_CARD_XPOS, last_ypos),
                            null,
                            Color.White,
                            PREVIEW_CARD_SCALE
                        );
                        last_ypos += (int)(Card.img_rows[row].Height * PREVIEW_CARD_SCALE.Y);
                    }
                }

                // Draw Card Info

                // Draw Card Info Name
                var card_name = gt.WrapText(fnt_message, card.name, PREVIEW_CARD_WIDTH);
                var card_name_size = fnt_message.MeasureString(card_name);
                last_ypos = PREVIEW_CARD_YPOS + PREVIEW_CARD_HEIGHT;
                gt.spriteBatch.DrawString(
                    fnt_message,
                    card_name,
                    new Vector2(
                        PREVIEW_CARD_XPOS + PREVIEW_CARD_WIDTH/2 - card_name_size.X/2,
                        last_ypos
                    ),
                    Color.White
                );
                last_ypos += (int)card_name_size.Y;

                // Draw Card Info Type
                var card_type = gt.WrapText(fnt_message, string.Format(TEXT_CARD_TYPE, card.type_name), PREVIEW_CARD_WIDTH);
                var card_type_size = fnt_message.MeasureString(card_type);
                gt.spriteBatch.DrawString(
                    fnt_message,
                    card_type,
                    new Vector2(
                        PREVIEW_CARD_XPOS,
                        last_ypos
                    ),
                    Color.White
                );
                last_ypos += (int)card_type_size.Y;

                // Draw Card Info Effect
                if (card is CardUnit || card is CardLeader) {
                    if (card.effect is not EffectNone) {
                        var card_effect = gt.WrapText(fnt_message, string.Format(TEXT_CARD_EFFECT, card.effect.Description), PREVIEW_CARD_WIDTH);
                        var card_effect_size = fnt_message.MeasureString(card_effect);
                        gt.spriteBatch.DrawString(
                            fnt_message,
                            card_effect,
                            new Vector2(
                                PREVIEW_CARD_XPOS,
                                last_ypos
                            ),
                            Color.White
                        );
                        last_ypos += (int)card_effect_size.Y;
                    }
                }

                // Draw Card Info Description
                var card_description = gt.WrapText(fnt_message, card.description, PREVIEW_CARD_WIDTH);
                var card_description_size = fnt_message.MeasureString(card_description);
                gt.spriteBatch.DrawString(
                    fnt_message,
                    card_description,
                    new Vector2(
                        PREVIEW_CARD_XPOS,
                        last_ypos
                    ),
                    Color.White
                );
                last_ypos += (int)card_description_size.Y;
            }

            switch (cursor.section) {
                case Section.HAND:
                    if (current_player.hand.Count != 0) DrawHoveredCard(current_player.hand[cursor.index]);
                    break;
                case Section.FIELD:
                    DrawHoveredCard(current_player.hand[cursor.hand]);
                    break;
                case Section.ROW:
                    var row = current_player.rows[(RowType)cursor.field];
                    if (row.Count != 0) DrawHoveredCard(row[cursor.index]);
                    break;
                case Section.LEADER:
                    DrawHoveredCard(current_player.leader);
                    break;
            }
        }
    }
    private void DrawMessage(GameTools gt) {
        if (
            scene is SceneStartGame ||
            scene is SceneStartPhase ||
            scene is SceneStartTurn ||
            scene is SceneEndTurn ||
            scene is SceneEndPhase ||
            scene is SceneEndGame ||
            help
        ) {
            gt.spriteBatch.Draw(
                img_dim_background,
                new Vector2(0,0),
                null,
                Color.Black
            );

            if (help) return;
            if (scene is SceneDeckSelection) return;

            var text_xpos = gt.graphics.PreferredBackBufferWidth / 4;
            var text_ypos = gt.graphics.PreferredBackBufferHeight / 2;
            var color = Color.White;
            string text = "";
            Vector2 size = new();
            Texture2D image = null;
            bool input = true;

            switch (scene) {
                case SceneStartGame:
                    text = TEXT_START_GAME;
                    size = fnt_message.MeasureString(text);
                    image = img_round_start;
                    break;
                case SceneStartPhase:
                    text = TEXT_START_PHASE;
                    size = fnt_message.MeasureString(text);
                    image = img_round_start;
                    break;
                case SceneStartTurn:
                    text = string.Format(TEXT_START_TURN, current_player.name);
                    size = fnt_message.MeasureString(text);
                    image = img_turn_start;
                    break;
                case SceneEndTurn:
                    if (!current_player.has_passed) {
                        input = false;
                        break;
                    }

                    text = string.Format(TEXT_END_TURN, current_player.name);
                    size = fnt_message.MeasureString(text);
                    image = img_turn_passed;
                    break;
                case SceneEndPhase:
                    text = string.Format(
                        TEXT_END_PHASE, 
                        (victor is not null)?
                            string.Format(TEXT_VICTORY, victor.name)
                            : TEXT_DRAW
                    );
                    size = fnt_message.MeasureString(text);
                    image = (victor is not null)? img_victory : img_draw;
                    break;
                case SceneEndGame:
                    text = string.Format(
                        TEXT_END_GAME, 
                        (victor is not null)?
                            string.Format(TEXT_VICTORY, victor.name)
                            : TEXT_DRAW
                    );
                    size = fnt_message.MeasureString(text);
                    image = (victor is not null)? img_victory : img_draw;
                    break;
                default:
                    input = false;
                    break;
            }

            if (input) {
                gt.spriteBatch.DrawString(
                    fnt_message,
                    text,
                    new Vector2(text_xpos, text_ypos - size.Y / 2),
                    color
                );
                gt.spriteBatch.Draw(
                    image,
                    new Vector2(IMAGE_TEXT_XPOS, IMAGE_TEXT_YPOS),
                    Color.White
                );

                var input_size = fnt_message.MeasureString(TEXT_PRESS_ENTER);
                gt.spriteBatch.DrawString(
                    fnt_message,
                    TEXT_PRESS_ENTER,
                    new Vector2(
                        (gt.graphics.PreferredBackBufferWidth - input_size.X) / 2,
                        gt.graphics.PreferredBackBufferHeight * 3 / 4
                    ),
                    color
                );
            }

            var help_size = fnt_message.MeasureString(TEXT_PRESS_F1);
            gt.spriteBatch.DrawString(
                fnt_message,
                TEXT_PRESS_F1,
                new Vector2(
                    (gt.graphics.PreferredBackBufferWidth - help_size.X) / 2,
                    gt.graphics.PreferredBackBufferHeight * 4 / 5
                ),
                color
            );
        }
    }
    private void DrawHelp(GameTools gt) {
        if (help) {
            var wrapped_text = gt.WrapText(fnt_message, TEXT_HELP, 1000);
            gt.spriteBatch.DrawString(
                fnt_message,
                wrapped_text,
                new Vector2(HELP_XPOS, HELP_YPOS),
                Color.White
            );
        }
    }

    public void Draw(GameTools gt) {
        if (scene is SceneDeckSelection) {
            DrawDeckSelection(gt);
        } else {
            DrawBoard(gt);
            DrawFields(gt);
            DrawCursor(gt);
            DrawSelectedCards(gt);
            DrawHoveredCardInfo(gt);
        }
        DrawMessage(gt);
        DrawHelp(gt);
    }

}

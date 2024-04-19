using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGwent;

public partial class BattleManager
{
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
    private const string TEXT_CARD_TYPE = "Card Type: {0}";

    private void DrawBoard(GraphicTools gt) {
        gt.spriteBatch.Draw(img_background, new Vector2(0,0), Color.White);
    }
    private void DrawFields(GraphicTools gt) {
        var show = scene == Scene.PLAY_TURN || scene == Scene.REDRAW;
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
    private void DrawCursor(GraphicTools gt) {
        if (cursor.section == Section.HAND) {
            var position = Card._GetRowPosition(
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
                        selected_card.types.Contains(row)? cursor.mark_card_enabled : cursor.mark_card_disabled,
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
        else if (cursor.section == Section.LEADER) {
            gt.spriteBatch.Draw(
                cursor.mark_card_hovered,
                new Vector2(
                    Player.LEADER_XPOS,
                    Player.GetRelativePosition(Player.LEADER_YPOS, Player.LEADER_YPOS_OFFSET, Card.HEIGHT, true)
                ),
                Color.White
            );
        }
    }
    private void DrawSelectedCards(GraphicTools gt) {
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
    private void DrawHoveredCardInfo(GraphicTools gt) {
        if (
            (
                scene == Scene.REDRAW ||
                scene == Scene.PLAY_TURN
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
                    if (((CardUnit)card).is_hero) {
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
                var card_type_size = fnt_message.MeasureString(card.type_name);
                gt.spriteBatch.DrawString(
                    fnt_message,
                    string.Format(TEXT_CARD_TYPE, card.type_name),
                    new Vector2(
                        PREVIEW_CARD_POWER_XPOS,
                        PREVIEW_CARD_YPOS + PREVIEW_CARD_HEIGHT + card_type_size.Y/2
                    ),
                    Color.White
                );
            }

            switch (cursor.section) {
                case Section.HAND:
                    if (player_1.hand.Count != 0) DrawHoveredCard(current_player.hand[cursor.index]);
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
    private void DrawMessage(GraphicTools gt) {
        if (
            scene == Scene.START_GAME ||
            scene == Scene.START_PHASE ||
            scene == Scene.START_TURN ||
            scene == Scene.END_TURN ||
            scene == Scene.END_PHASE ||
            scene == Scene.END_GAME
        ) {
            gt.spriteBatch.Draw(
                img_dim_background,
                new Vector2(0,0),
                null,
                Color.Black
            );

            var text_xpos = gt.graphics.PreferredBackBufferWidth / 4;
            var text_ypos = gt.graphics.PreferredBackBufferHeight / 2;
            var color = Color.White;
            string text;
            Vector2 size;
            bool input = true;

            switch (scene) {
                case Scene.START_GAME:
                    text = TEXT_START_GAME;
                    size = fnt_message.MeasureString(text);
                    gt.spriteBatch.DrawString(
                        fnt_message,
                        text,
                        new Vector2(text_xpos, text_ypos - size.Y / 2),
                        color
                    );
                    gt.spriteBatch.Draw(
                        img_round_start,
                        new Vector2(IMAGE_TEXT_XPOS, IMAGE_TEXT_YPOS),
                        Color.White
                    );
                    break;
                case Scene.START_PHASE:
                    text = TEXT_START_PHASE;
                    size = fnt_message.MeasureString(text);
                    gt.spriteBatch.DrawString(
                        fnt_message,
                        text,
                        new Vector2(text_xpos, text_ypos - size.Y / 2),
                        color
                    );
                    gt.spriteBatch.Draw(
                        img_round_start,
                        new Vector2(IMAGE_TEXT_XPOS, IMAGE_TEXT_YPOS),
                        Color.White
                    );
                    break;
                case Scene.START_TURN:
                    text = string.Format(TEXT_START_TURN, current_player.name);
                    size = fnt_message.MeasureString(text);
                    gt.spriteBatch.DrawString(
                        fnt_message,
                        text,
                        new Vector2(text_xpos, text_ypos - size.Y / 2),
                        color
                    );
                    gt.spriteBatch.Draw(
                        img_turn_start,
                        new Vector2(IMAGE_TEXT_XPOS, IMAGE_TEXT_YPOS),
                        Color.White
                    );
                    break;
                case Scene.END_TURN:
                    if (!current_player.has_passed) {
                        input = false;
                        break;
                    }

                    text = string.Format(TEXT_END_TURN, current_player.name);
                    size = fnt_message.MeasureString(text);
                    gt.spriteBatch.DrawString(
                        fnt_message,
                        text,
                        new Vector2(text_xpos, text_ypos - size.Y / 2),
                        color
                    );
                    gt.spriteBatch.Draw(
                        img_turn_passed,
                        new Vector2(IMAGE_TEXT_XPOS, IMAGE_TEXT_YPOS),
                        Color.White
                    );
                    break;
                case Scene.END_PHASE:
                    text = string.Format(TEXT_END_PHASE, 
                        (victor is not null)? string.Format(TEXT_VICTORY, victor.name) : TEXT_DRAW
                    );
                    size = fnt_message.MeasureString(text);
                    gt.spriteBatch.DrawString(
                        fnt_message,
                        text,
                        new Vector2(text_xpos, text_ypos - size.Y / 2),
                        color
                    );
                    gt.spriteBatch.Draw(
                        (victor is not null)? img_victory : img_draw,
                        new Vector2(IMAGE_TEXT_XPOS, IMAGE_TEXT_YPOS),
                        Color.White
                    );
                    break;
                case Scene.END_GAME:
                    text = string.Format(TEXT_END_GAME, 
                        (victor is not null)? string.Format(TEXT_VICTORY, victor.name) : TEXT_DRAW
                    );
                    size = fnt_message.MeasureString(text);
                    gt.spriteBatch.DrawString(
                        fnt_message,
                        text,
                        new Vector2(text_xpos, text_ypos - size.Y / 2),
                        color
                    );
                    gt.spriteBatch.Draw(
                        (victor is not null)? img_victory : img_draw,
                        new Vector2(IMAGE_TEXT_XPOS, IMAGE_TEXT_YPOS),
                        Color.White
                    );
                    break;
                default:
                    input = false;
                    break;
            }

            if (input) {
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
        }
    }

    public void Draw(GraphicTools gt) {
        DrawBoard(gt);
        DrawFields(gt);
        DrawCursor(gt);
        DrawSelectedCards(gt);
        DrawHoveredCardInfo(gt);
        DrawMessage(gt);
    }

}

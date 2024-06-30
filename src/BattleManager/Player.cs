using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGwent;

public class Player
{
    public const int MAX_NAME_LENGTH = 12;
    public const int DEFAULT_HEALTH = 2;
    public const int DEFEATED_HEALTH = 0;
    public const int STARTING_CARDS = 10;
    public const int PHASE_DRAW_CARDS = 2;
    public const int HAND_CARDS_LIMIT = 10;

    public const int PLAYER_LABEL_XPOS = 10;
    public const int PLAYER_LABEL_PLAYER_YPOS = 420;
    public const int PLAYER_LABEL_RIVAL_YPOS = 242;
    public const int PLAYER_DECK_COUNT_XPOS = 55;
    public const int PLAYER_DECK_COUNT_YPOS = 360;
    public const int PLAYER_DECK_COUNT_YPOS_OFFSET = 110;
    public const int PLAYER_DECK_COUNT_YPOS_RELATIVE = -40;
    public const int PLAYER_HAND_COUNT_XPOS = 110;
    public const int PLAYER_HAND_COUNT_YPOS = 360;
    public const int PLAYER_HAND_COUNT_YPOS_OFFSET = 110;
    public const int PLAYER_HAND_COUNT_YPOS_RELATIVE = -40;
    public const int PLAYER_POWER_XPOS = 162;
    public const int PLAYER_POWER_YPOS = 360;
    public const int PLAYER_POWER_YPOS_OFFSET = 87;
    public const int PLAYER_HIGHSCORE_XPOS = 137;
    public const int PLAYER_HIGHSCORE_YPOS = 360;
    public const int PLAYER_HIGHSCORE_YPOS_OFFSET = 68;
    public const int PLAYER_PASSED_XPOS = 162;
    public const int PLAYER_PASSED_PLAYER_YPOS = 486;
    public const int PLAYER_PASSED_RIVAL_YPOS = 309;
    public const string TEXT_PLAYER_PASS = "Pass";

    public const int HEALTH_XPOS = 50;
    public const int HEALTH_XPOS_RIM = 50;
    public const int HEALTH_YPOS = 360;
    public const int HEALTH_YPOS_OFFSET = 135;
    public const int HEALTH_HEIGHT = 42;

    public const int HAND_XPOS = 6;
    public const int HAND_YPOS = 360;
    public const int HAND_YPOS_OFFSET = 280;
    public const int HAND_WIDTH = 790;
    public const int HAND_HEIGHT = 80;

    public const int DECK_XPOS = 737;
    public const int DECK_YPOS = 360;
    public const int DECK_YPOS_OFFSET = 184;

    public const int LEADER_XPOS = 25;
    public const int LEADER_YPOS = 360;
    public const int LEADER_YPOS_OFFSET = 184;
    public const int LEADER_ACTIVE_XPOS = 84;
    public const int LEADER_ACTIVE_YPOS = 360;
    public const int LEADER_ACTIVE_YPOS_OFFSET = 204;
    public const int LEADER_ACTIVE_HEIGHT = 40;

    public const int GRAVEYARD_XPOS = 737;
    public const int GRAVEYARD_YPOS = 360;
    public const int GRAVEYARD_YPOS_OFFSET = 94;

    public const int ROW_XPOS = 293;
    public const int ROW_WIDTH = 430;
    public const int ROW_YPOS = 360;
    public static readonly Dictionary<RowType,int> ROW_YPOS_OFFSET = new Dictionary<RowType,int> {
        {RowType.MELEE, 4},
        {RowType.RANGE, 94},
        {RowType.SIEGE, 184}
    };


    public const int ROW_POWER_XPOS = 210;
    public const int ROW_POWER_YPOS = 360;
    public static Dictionary<RowType,int> ROW_POWER_YPOS_OFFSET
    = ROW_YPOS_OFFSET.ToDictionary(x => x.Key, x => x.Value + Card.HEIGHT / 2);

    public const int BOOST_XPOS = 228;
    public const int BOOST_YPOS = 360;
    public static readonly Dictionary<RowType,int> BOOST_YPOS_OFFSET = ROW_YPOS_OFFSET;

    public string name;
    public int health = DEFAULT_HEALTH;
    public readonly Dictionary<RowType, List<CardUnit>> rows
    = Enum.GetValues(typeof(RowType)).Cast<RowType>().ToDictionary(x => x, x => new List<CardUnit>());
    public readonly Dictionary<RowType, CardBoost> boosts
    = Enum.GetValues(typeof(RowType)).Cast<RowType>().ToDictionary(x => x, x => (CardBoost)null);
    public List<Card> hand = [];
    public Deck original_deck;
    public Deck deck = new();
    public readonly List<Card> graveyard = [];
    public readonly List<Card> selected = [];
    public CardLeader leader {get => deck.leader;}
    public bool has_passed = false;

    private Texture2D img_health_on;
    private Texture2D img_health_off;
    private Texture2D img_leader;
    private Texture2D img_highscore;
    private Texture2D img_row_weather;
    private SpriteFont fnt_status;

    public Player(string name) {
        this.name = name;
    }

    public bool IsDefeated() => health == DEFEATED_HEALTH;

    public void Initialize(Deck d=null) {
        health = DEFAULT_HEALTH;
        has_passed = false;
        if (d is not null) {
            original_deck = d;
        }
        deck.Copy(original_deck);
        deck.Shuffle();
        graveyard.Clear();
        hand.Clear();
        selected.Clear();
        foreach (var row in rows) {
            rows[row.Key].Clear();
            boosts[row.Key] = null;
        }
        ReceiveCard(STARTING_CARDS);
    }
    public void ReceiveCard(int count=1, bool skip_limit=false) {
        while (count > 0 && deck.Count != 0) {
            var card = deck.Take();
            if (hand.Count >= HAND_CARDS_LIMIT && !skip_limit)
            {graveyard.Add(card);}
            else {hand.Add(card);}            
            count -= 1;
        }
    }
    public Card GetHandCard(int index) {
        return hand[index];
    }
    public Card GetFieldCard(RowType row, int index) {
        return rows[row][index];
    }
    public int GetRowPower(RowType row_type, Tuple<CardWeather,Player> weather) {
        int row_power = 0;
        var row = rows[row_type];
        foreach (var card in row) {
            row_power += card.GetPower(weather.Item1, boosts[row_type]);
        }
        return row_power;
    }
    public int GetPower(Dictionary<RowType,Tuple<CardWeather,Player>> weathers) {
        int power = 0;
        foreach (var row in rows.Keys) {
            power += GetRowPower(row, weathers[row]);
        }
        return power;
    }
    public bool HasBoost(RowType row) {
        return boosts[row] is not null;
    }
    public void ClearField() {
        foreach (var row in rows) {
            foreach (var card in row.Value) {
                graveyard.Add(card);
            }
            row.Value.Clear();
            if (boosts[row.Key] is not null) graveyard.Add(boosts[row.Key]);
            boosts[row.Key] = null;
        }
    }
    public void Clear() {
        ClearField();
        has_passed = false;
    }

    public void LoadContent(GameTools gt) {
        img_health_on = gt.content.Load<Texture2D>("graphics/img/health_on");
        img_health_off = gt.content.Load<Texture2D>("graphics/img/health_off");
        img_leader = gt.content.Load<Texture2D>("graphics/img/leader");
        img_highscore = gt.content.Load<Texture2D>("graphics/img/highscore");
        img_row_weather = gt.content.Load<Texture2D>("graphics/img/row_weather");
        fnt_status = gt.content.Load<SpriteFont>("font/Arial");
    }

    public static int GetRelativePosition(int pos, int offset, int relative, bool positive) {
        return pos + (positive? 1 : -1)*offset - (!positive? relative : 0);
    }

    public void DrawPlayerStatus(GameTools gt, Dictionary<RowType, Tuple<CardWeather,Player>> weathers, bool is_turn, bool highscore) {
        int xpos = PLAYER_LABEL_XPOS;
        int ypos = (is_turn == true)? PLAYER_LABEL_PLAYER_YPOS : PLAYER_LABEL_RIVAL_YPOS;
        gt.spriteBatch.DrawString(fnt_status, name, new Vector2(xpos, ypos), Color.White);

        var deck_count = deck.Count.ToString();
        var deck_count_size = fnt_status.MeasureString(deck_count);
        gt.spriteBatch.DrawString(
            fnt_status,
            deck_count,
            new Vector2(
                PLAYER_DECK_COUNT_XPOS - deck_count_size.X / 2,
                GetRelativePosition(PLAYER_DECK_COUNT_YPOS, PLAYER_DECK_COUNT_YPOS_OFFSET, PLAYER_DECK_COUNT_YPOS_RELATIVE, is_turn) - deck_count_size.Y/2
            ),
            Color.White
        );

        var hand_count = hand.Count.ToString();
        var hand_count_size = fnt_status.MeasureString(hand_count);
        gt.spriteBatch.DrawString(
            fnt_status,
            hand_count,
            new Vector2(
                PLAYER_HAND_COUNT_XPOS - hand_count_size.X / 2,
                GetRelativePosition(PLAYER_HAND_COUNT_YPOS, PLAYER_HAND_COUNT_YPOS_OFFSET, PLAYER_HAND_COUNT_YPOS_RELATIVE, is_turn) - hand_count_size.Y/2
            ),
            Color.White
        );

        var total_power = GetPower(weathers).ToString();
        var total_power_size = fnt_status.MeasureString(total_power);
        gt.spriteBatch.DrawString(
            fnt_status, 
            total_power,
            new Vector2(
                PLAYER_POWER_XPOS - total_power_size.X / 2,
                GetRelativePosition(PLAYER_POWER_YPOS, PLAYER_POWER_YPOS_OFFSET, 0, is_turn) - total_power_size.Y/2
            ),
            Color.White
        );

        if (highscore) {
            gt.spriteBatch.Draw(
                img_highscore,
                new Vector2(
                    PLAYER_HIGHSCORE_XPOS,
                    GetRelativePosition(PLAYER_HIGHSCORE_YPOS, PLAYER_HIGHSCORE_YPOS_OFFSET, img_highscore.Height, is_turn)
                ),
                Color.White
            );
        }

        if (has_passed) {
            var passed_size = fnt_status.MeasureString(TEXT_PLAYER_PASS);
            gt.spriteBatch.DrawString(
                fnt_status,
                TEXT_PLAYER_PASS,
                new Vector2(
                    PLAYER_PASSED_XPOS - passed_size.X/2,
                    (is_turn? PLAYER_PASSED_PLAYER_YPOS : PLAYER_PASSED_RIVAL_YPOS) - passed_size.Y/2
                ),
                Color.Orange
            );
        }

        // Draw Health
        gt.spriteBatch.Draw(
            (health > 0)? img_health_on : img_health_off,
            new Vector2(
                HEALTH_XPOS,
                GetRelativePosition(HEALTH_YPOS, HEALTH_YPOS_OFFSET, HEALTH_HEIGHT, is_turn)
            ),
            null,
            Color.White
        );
        gt.spriteBatch.Draw(
            (health > 1)? img_health_on : img_health_off,
            new Vector2(
                HEALTH_XPOS + HEALTH_XPOS_RIM,
                GetRelativePosition(HEALTH_YPOS, HEALTH_YPOS_OFFSET, HEALTH_HEIGHT, is_turn)
            ),
            null,
            Color.White
        );
    }
    public void DrawHand(GameTools gt, bool is_turn, bool show) {
        for (int i = 0; i<hand.Count; i++) {
            var card = hand[i];
            var position = card.GetRowPosition(
                i,
                hand.Count,
                HAND_XPOS,
                GetRelativePosition(HAND_YPOS, HAND_YPOS_OFFSET, Card.HEIGHT, is_turn),
                HAND_WIDTH 
            );
            gt.spriteBatch.Draw(
                (is_turn && show)? card.img : Card.img_back,
                position,
                null,
                Color.White,
                Card.THUMB_SCALE
            );
        }
    }
    public void DrawRows(GameTools gt, Dictionary<RowType, Tuple<CardWeather,Player>> weathers, bool is_turn) {
        foreach (var row in rows) {

            // Draw Row Cards
            var cards = row.Value;
            for (int i = 0; i<cards.Count; i++) {
                var card = cards[i];
                var position = card.GetRowPosition(
                    i,
                    cards.Count,
                    ROW_XPOS,
                    GetRelativePosition(ROW_YPOS, ROW_YPOS_OFFSET[row.Key], Card.HEIGHT, is_turn),
                    ROW_WIDTH 
                );
                gt.spriteBatch.Draw(
                    card.img,
                    position,
                    null,
                    Color.White,
                    Card.THUMB_SCALE
                );
            }

            // Draw Row Boost
            if (boosts[row.Key] is not null) {
                gt.spriteBatch.Draw(
                    boosts[row.Key].img,
                    new Vector2(
                        BOOST_XPOS,
                        GetRelativePosition(BOOST_YPOS, BOOST_YPOS_OFFSET[row.Key], Card.HEIGHT, is_turn)
                    ),
                    null,
                    Color.White,
                    Card.THUMB_SCALE
                );
            }

            // Draw Row Weather
            if (weathers[row.Key].Item1 is not null) {
                gt.spriteBatch.Draw(
                    img_row_weather,
                    new Vector2(
                        ROW_XPOS,
                        GetRelativePosition(ROW_YPOS, ROW_YPOS_OFFSET[row.Key], Card.HEIGHT, is_turn)
                    ),
                    null,
                    Color.White
                );
            }

            // Draw Row Power
            var row_power = GetRowPower(row.Key, weathers[row.Key]);
            var size = fnt_status.MeasureString(row_power.ToString());
            gt.spriteBatch.DrawString(
                fnt_status,
                row_power.ToString(),
                new Vector2(
                    ROW_POWER_XPOS - size.X / 2,
                    GetRelativePosition(ROW_POWER_YPOS, ROW_POWER_YPOS_OFFSET[row.Key], 0, is_turn) - size.Y/2
                ),
                Color.White
            );
        }
    }
    public void DrawLeader(GameTools gt, bool is_turn) {
        gt.spriteBatch.Draw(
            leader.img,
            new Vector2(
                LEADER_XPOS,
                GetRelativePosition(LEADER_YPOS, LEADER_YPOS_OFFSET, Card.HEIGHT, is_turn)
            ),
            null,
            Color.White,
            Card.THUMB_SCALE
        );
        if (!leader.used) {
            gt.spriteBatch.Draw(
                img_leader,
                new Vector2(
                    LEADER_ACTIVE_XPOS,
                    GetRelativePosition(LEADER_ACTIVE_YPOS, LEADER_ACTIVE_YPOS_OFFSET, LEADER_ACTIVE_HEIGHT, is_turn)
                ),
                null,
                Color.White
            );
        }
    }
    public void DrawDeck(GameTools gt, bool is_turn) {
        if (deck.Count != 0) {
            gt.spriteBatch.Draw(
                Card.img_back,
                new Vector2(
                    DECK_XPOS,
                    GetRelativePosition(DECK_YPOS, DECK_YPOS_OFFSET, Card.HEIGHT, is_turn)
                ),
                null,
                Color.White,
                Card.THUMB_SCALE
            );
        }
    }
    public void DrawGraveyard(GameTools gt, bool is_turn) {
        if (graveyard.Count != 0) {
            gt.spriteBatch.Draw(
                graveyard[^1].img,
                new Vector2(
                    GRAVEYARD_XPOS,
                    GetRelativePosition(GRAVEYARD_YPOS, GRAVEYARD_YPOS_OFFSET, Card.HEIGHT, is_turn)
                ),
                null,
                Color.White,
                Card.THUMB_SCALE
            );
        }
    }
    public void Draw(GameTools gt, Dictionary<RowType, Tuple<CardWeather,Player>> weathers, bool is_turn, bool highscore, bool show) {
        DrawPlayerStatus(gt, weathers, is_turn, highscore);
        DrawHand(gt, is_turn, show);
        DrawRows(gt, weathers, is_turn);
        DrawLeader(gt, is_turn);
        DrawGraveyard(gt, is_turn);
        DrawDeck(gt, is_turn);
    }

}

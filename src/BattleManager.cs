using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace MonoGwent;

public partial class BattleManager
{
    private class Cursor {
        private const int DEFAULT_INDEX = 0;
        public const int NONE = -1;
        public Section section = Section.HAND;
        public int index = DEFAULT_INDEX;
        public int hand = NONE;
        public int field = NONE;
        public bool holding = false;

        public Texture2D mark_card_hovered;
        public Texture2D mark_card_selected;
        public Texture2D mark_row_hovered;
        public Texture2D mark_row_enabled;
        public Texture2D mark_row_disabled;
        public Texture2D mark_row_hovered_disabled;

        public void Move(Section s, int i, int h, int f, bool hold=true) {
            section = s;
            index = i;
            hand = h;
            field = f;
            if (hold) Hold();
        }
        public void Move(Section s, int i, bool hold=true) {
            var f = NONE;
            var h = NONE;
            if (s == Section.FIELD) {
                h = hand;
            }
            else if (s == Section.ROW) {
                f = field;
                h = hand;
            }
            Move(s, i, h, f, hold);
        }
        public void Move(Section s, bool hold=true) {
            Move(s, DEFAULT_INDEX, hold);
        }
        public void Move(int i, bool hold=true) {
            Move(section, i, hand, field, hold);
        }
        public void Hold() {holding = true;}
        public void Release() {holding = false;}
    }

    private enum Scene {
        START_GAME,
        REDRAW,
        START_PHASE,
        START_TURN,
        PLAY_TURN,
        END_TURN,
        END_PHASE,
        END_GAME
    }
    private enum Section {
        HAND,
        FIELD,
        ROW,
        LEADER,
    }

    private const string PLAYER_1_NAME = "Radiant";
    private const string PLAYER_2_NAME = "Dire";
    private const int REDRAW_PHASE = 0;
    private const int LOSE_HEALTH = 0;

    private int phase = REDRAW_PHASE;
    private Scene scene = Scene.START_GAME;
    private Cursor cursor = new ();
    private Player player_1 = new Player(PLAYER_1_NAME);
    private Player player_2 = new Player(PLAYER_2_NAME);
    private Player[] players {get => [player_1, player_2];}
    private Player current_player;
    private Player rival_player {get => current_player==player_1? player_2 : player_1;}
    private Player highscore_player {
        get {
            var p1 = player_1.GetPower(weathers);
            var p2 = player_2.GetPower(weathers);
            if
            (p1 > p2) {return player_1;}
            else if
            (p2 > p1) {return player_2;}
            else
            {return null;}
        }
    }
    private Player victor = null;
    private Dictionary<RowType,List<CardWeather>> weathers
    = Enum.GetValues(typeof(RowType)).Cast<RowType>().ToDictionary(x => x, x => new List<CardWeather>());

    private Texture2D img_background;
    private Texture2D img_dim_background;
    private Texture2D img_round_start;
    private Texture2D img_turn_start;
    private Texture2D img_turn_passed;
    private Texture2D img_victory;
    private Texture2D img_draw;
    private SpriteFont fnt_message;

    public void InitializePlayers() {
        player_1.Initialize();
        player_2.Initialize();
        current_player = (Random.Shared.Next(2)==0)? player_1 : player_2;
    }
    public void Initialize(Deck deck1, Deck deck2) {
        current_player = player_1;
        player_1.Initialize(deck1);
        player_2.Initialize(deck2);
    }

    public void LoadContent(GraphicTools gt) {
        Card.img_back = gt.content.Load<Texture2D>("card_back");
        Card.img_power_normal = gt.content.Load<Texture2D>("power_normal");
        Card.img_power_hero = gt.content.Load<Texture2D>("power_hero");
        Card.img_melee = gt.content.Load<Texture2D>("melee");
        Card.img_range = gt.content.Load<Texture2D>("range");
        Card.img_siege = gt.content.Load<Texture2D>("siege");
        Card.img_rows = new() {
            {RowType.MELEE,Card.img_melee},
            {RowType.RANGE,Card.img_range},
            {RowType.SIEGE,Card.img_siege}
        };

        cursor.mark_card_hovered = new Texture2D(gt.graphics.GraphicsDevice, Card.WIDTH, Card.HEIGHT);
        cursor.mark_card_hovered.CreateBorder(6, Color.Lime);
        cursor.mark_card_selected = new Texture2D(gt.graphics.GraphicsDevice, Card.WIDTH, Card.HEIGHT);
        cursor.mark_card_selected.CreateBorder(3, Color.DeepSkyBlue);

        cursor.mark_row_hovered = new Texture2D(gt.graphics.GraphicsDevice, Player.ROW_WIDTH, Card.HEIGHT);
        cursor.mark_row_hovered.CreateBorder(5, Color.Lime);
        cursor.mark_row_enabled = new Texture2D(gt.graphics.GraphicsDevice, Player.ROW_WIDTH, Card.HEIGHT);
        cursor.mark_row_enabled.CreateBorder(5, Color.Silver);
        cursor.mark_row_disabled = new Texture2D(gt.graphics.GraphicsDevice, Player.ROW_WIDTH, Card.HEIGHT);
        cursor.mark_row_disabled.CreateBorder(5, Color.Black);
        cursor.mark_row_hovered_disabled = new Texture2D(gt.graphics.GraphicsDevice, Player.ROW_WIDTH, Card.HEIGHT);
        cursor.mark_row_hovered_disabled.CreateBorder(5, Color.Red);

        img_background = gt.content.Load<Texture2D>("desk");
        img_dim_background = gt.content.Load<Texture2D>("dim_background");
        img_round_start = gt.content.Load<Texture2D>("round_start");
        img_turn_start = gt.content.Load<Texture2D>("turn_start");
        img_turn_passed = gt.content.Load<Texture2D>("turn_passed");
        img_victory = gt.content.Load<Texture2D>("victory");
        img_draw = gt.content.Load<Texture2D>("draw");
        fnt_message = gt.content.Load<SpriteFont>("Arial");
        foreach (var player in players) player.LoadContent(gt);
        CardsDump.LoadContent(gt);
    }

}

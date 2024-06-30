using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace MonoGwent;

public partial class BattleManager
{

    public const string PLAYER_1_NAME = "Radiant";
    public const string PLAYER_2_NAME = "Dire";
    public const int REDRAW_PHASE = 0;
    public const int LOSE_HEALTH = 0;

    private int phase = REDRAW_PHASE;
    private IScene scene = new SceneDeckSelection();
    private bool help = false;
    private Cursor cursor = new ();
    private Player player_1 = new Player(PLAYER_1_NAME);
    private Player player_2 = new Player(PLAYER_2_NAME);
    private Player[] players {get => [player_1, player_2];}
    private Player current_player;
    private Player rival_player {get => GetOtherPlayer(current_player);}
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
    private Dictionary<RowType,Tuple<CardWeather,Player>> weathers
    = Enum.GetValues(typeof(RowType)).Cast<RowType>().ToDictionary(
    x => x, x => new Tuple<CardWeather, Player>(null, null));

    // BattleManager public accessors
    public int Phase {get => phase;}
    public IScene Scene {get => scene; set => scene = value;}
    public Cursor Cursor {get => cursor;}
    public Player Player1 {get => player_1;}
    public Player Player2 {get => player_2;}
    public Player[] Players {get => [player_1,player_2];}
    public Player Current {get => current_player; set => current_player = value;}
    public Player Rival {get => rival_player;}
    public Player Highscore {get => highscore_player;}
    public Dictionary<RowType,Tuple<CardWeather,Player>> Weathers {get => weathers;}
    public Card HandCard {
        get {
            if (
                cursor.section == Section.FIELD ||
                cursor.section == Section.ROW
            ) {
                return current_player.GetHandCard(cursor.hand);
            }
            return null;
        }
    }

    private Texture2D img_background;
    private Texture2D img_dim_background;
    private Texture2D img_round_start;
    private Texture2D img_turn_start;
    private Texture2D img_turn_passed;
    private Texture2D img_victory;
    private Texture2D img_draw;
    private SpriteFont fnt_message;
    private Song bgm_startup;
    private Song bgm_playing1;
    private Song bgm_playing2;
    private Song bgm_playing3;
    private SoundEffect sfx_playcard;
    private SoundEffect sfx_select;
    private SoundEffect sfx_cancel;
    private SoundEffect sfx_win;

    public Texture2D ImgBackground {get => img_background;}
    public Texture2D ImgDimBackground {get => img_dim_background;}
    public Texture2D ImgRoundStart {get => img_round_start;}
    public Texture2D ImgTurnStart {get => img_turn_start;}
    public Texture2D ImgTurnPassed {get => img_turn_passed;}
    public Texture2D ImgVictory {get => img_victory;}
    public Texture2D ImgDraw {get => img_draw;}
    public SpriteFont FntMessage {get => fnt_message;}
    public Song BgmStartup {get => bgm_startup;}
    public Song BgmPlaying1 {get => bgm_playing1;}
    public Song BgmPlaying2 {get => bgm_playing2;}
    public Song BgmPlaying3 {get => bgm_playing3;}
    public SoundEffect SfxPlaycard {get => sfx_playcard;}
    public SoundEffect SfxSelect {get => sfx_select;}
    public SoundEffect SfxCancel {get => sfx_cancel;}
    public SoundEffect SfxWin {get => sfx_win;}

    public void InitializePlayers() {
        player_1.Initialize();
        player_2.Initialize();
        current_player = (Random.Shared.Next(2)==0)? player_1 : player_2;
    }
    public void Initialize() {
        MediaPlayer.Play(bgm_startup);
        MediaPlayer.IsRepeating = true;
        current_player = player_1;
        player_1.original_deck = DecksDump.GetDeck();
        player_2.original_deck = DecksDump.GetDeck();
    }

    public void LoadContent(GameTools gt) {
        Card.img_back = gt.content.Load<Texture2D>("graphics/img/card_back");
        Card.img_power_normal = gt.content.Load<Texture2D>("graphics/img/card_power_normal");
        Card.img_power_hero = gt.content.Load<Texture2D>("graphics/img/card_power_hero");
        Card.img_weather = gt.content.Load<Texture2D>("graphics/img/card_weather");
        Card.img_dispel = gt.content.Load<Texture2D>("graphics/img/card_dispel");
        Card.img_boost = gt.content.Load<Texture2D>("graphics/img/card_boost");
        Card.img_decoy = gt.content.Load<Texture2D>("graphics/img/card_decoy");
        Card.img_melee = gt.content.Load<Texture2D>("graphics/img/melee");
        Card.img_range = gt.content.Load<Texture2D>("graphics/img/range");
        Card.img_siege = gt.content.Load<Texture2D>("graphics/img/siege");
        Card.img_rows = new() {
            {RowType.MELEE,Card.img_melee},
            {RowType.RANGE,Card.img_range},
            {RowType.SIEGE,Card.img_siege}
        };

        cursor.mark_card_hovered = new Texture2D(gt.graphics.GraphicsDevice, Card.WIDTH, Card.HEIGHT);
        cursor.mark_card_hovered.CreateBorder(6, Color.Lime);
        cursor.mark_card_selected = new Texture2D(gt.graphics.GraphicsDevice, Card.WIDTH, Card.HEIGHT);
        cursor.mark_card_selected.CreateBorder(3, Color.DeepSkyBlue);
        cursor.mark_card_enabled = new Texture2D(gt.graphics.GraphicsDevice, Card.WIDTH, Card.HEIGHT);
        cursor.mark_card_enabled.CreateBorder(6, Color.Silver);
        cursor.mark_card_disabled = new Texture2D(gt.graphics.GraphicsDevice, Card.WIDTH, Card.HEIGHT);
        cursor.mark_card_disabled.CreateBorder(6, Color.Black);
        cursor.mark_card_hovered_disabled = new Texture2D(gt.graphics.GraphicsDevice, Card.WIDTH, Card.HEIGHT);
        cursor.mark_card_hovered_disabled.CreateBorder(5, Color.Red);

        cursor.mark_row_hovered = new Texture2D(gt.graphics.GraphicsDevice, Player.ROW_WIDTH, Card.HEIGHT);
        cursor.mark_row_hovered.CreateBorder(5, Color.Lime);
        cursor.mark_row_enabled = new Texture2D(gt.graphics.GraphicsDevice, Player.ROW_WIDTH, Card.HEIGHT);
        cursor.mark_row_enabled.CreateBorder(5, Color.Silver);
        cursor.mark_row_disabled = new Texture2D(gt.graphics.GraphicsDevice, Player.ROW_WIDTH, Card.HEIGHT);
        cursor.mark_row_disabled.CreateBorder(5, Color.Black);
        cursor.mark_row_hovered_disabled = new Texture2D(gt.graphics.GraphicsDevice, Player.ROW_WIDTH, Card.HEIGHT);
        cursor.mark_row_hovered_disabled.CreateBorder(5, Color.Red);

        img_background = gt.content.Load<Texture2D>("graphics/img/desk");
        img_dim_background = gt.content.Load<Texture2D>("graphics/img/dim_background");
        img_round_start = gt.content.Load<Texture2D>("graphics/img/round_start");
        img_turn_start = gt.content.Load<Texture2D>("graphics/img/turn_start");
        img_turn_passed = gt.content.Load<Texture2D>("graphics/img/turn_passed");
        img_victory = gt.content.Load<Texture2D>("graphics/img/victory");
        img_draw = gt.content.Load<Texture2D>("graphics/img/draw");
        fnt_message = gt.content.Load<SpriteFont>("font/Arial");
        bgm_startup = gt.content.Load<Song>("music/bgm_startup");
        bgm_playing1 = gt.content.Load<Song>("music/bgm_playing1");
        bgm_playing2 = gt.content.Load<Song>("music/bgm_playing2");
        bgm_playing3 = gt.content.Load<Song>("music/bgm_playing3");
        sfx_playcard = gt.content.Load<SoundEffect>("music/sfx_playcard");
        sfx_select = gt.content.Load<SoundEffect>("music/sfx_select");
        sfx_cancel = gt.content.Load<SoundEffect>("music/sfx_cancel");
        sfx_win = gt.content.Load<SoundEffect>("music/sfx_win");

        foreach (var player in players) player.LoadContent(gt);
    }

    public Player GetOtherPlayer(Player player) {
        return player==player_1? player_2 : player_1;
    }

}

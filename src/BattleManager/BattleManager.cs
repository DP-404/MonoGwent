using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace MonoGwent;

public partial class BattleManager
{

    public const string PLAYER_1_NAME = "Radiant";
    public const string PLAYER_2_NAME = "Dire";
    public const int REDRAW_PHASE = -1;
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
    private Player effect_player = null;
    private List<Player>[] phase_victors = new List<Player>[Player.DEFAULT_HEALTH+1];
    private Player victor = null;
    private Dictionary<RowType,Tuple<CardWeather,Player>> weathers
    = Enum.GetValues(typeof(RowType)).Cast<RowType>().ToDictionary(
    x => x, x => new Tuple<CardWeather, Player>(null, null));
    private Card hand_card = null;

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
    public Player EffectPlayer {get => effect_player;}
    public Player Victor {get => victor;}
    public Dictionary<RowType,Tuple<CardWeather,Player>> Weathers {get => weathers;}
    public Card HandCard {get => hand_card;}
    public RowType? FieldType {
        get {
            if (cursor.hand == Cursor.NONE) {
                return null;
            }
            else if (cursor.field == Cursor.NONE) {
                return (RowType)cursor.index;
            } else {
                return (RowType)cursor.field;
            }
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
    private SoundEffect sfx_pass;
    private SoundEffect sfx_select;
    private SoundEffect sfx_cancel;
    private SoundEffect sfx_win;

    public SoundEffect SfxSelect {get => sfx_select;}
    public SoundEffect SfxCancel {get => sfx_cancel;}

    public BattleManager() {
        for (int i = 0; i < phase_victors.Length; i++) {
            phase_victors[i] = new();
        }
    }

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
        sfx_pass = gt.content.Load<SoundEffect>("music/sfx_pass");
        sfx_select = gt.content.Load<SoundEffect>("music/sfx_select");
        sfx_cancel = gt.content.Load<SoundEffect>("music/sfx_cancel");
        sfx_win = gt.content.Load<SoundEffect>("music/sfx_win");

        foreach (var player in players) player.LoadContent(gt);
    }

    public Player GetOtherPlayer(Player player) {
        return player==player_1? player_2 : player_1;
    }

    public void UpdateHandCard() {
        if (
            (
                cursor.section == Section.FIELD
                || cursor.section == Section.ROW
            )
            && cursor.hand < current_player.hand.Count()
        ) {
            hand_card = current_player.GetHandCard(cursor.hand);
        } else {
            hand_card = null;
        }
    }

    public void AddVictor(Player player) {
        phase_victors[phase].Add(player);
    }

    public bool IsVictor(Player player, int phase) {
        return phase_victors[phase].Contains(player);
    }

    public void ClearVictor(int? phase=null) {
        if (phase is null) {
            foreach (var l in phase_victors) l.Clear();
            victor = null;
        } else {
            phase_victors[(int)phase].Clear();
            if (phase == this.phase) victor = null;
        }
    }

    public void PlayCard() {
        if (current_player.has_played) return;

        // If Field is not valid > Return
        if (!HandCard.types.Contains((RowType)FieldType) && HandCard.types.Length != 0) return;

        var played = HandCard.PlayCard(this);
        if (!played) return;

        sfx_playcard.Play();
        cursor.Move(Section.HAND);

        current_player.has_played = true;
    }

    public void UseCardEffect(Player player, Card card) {
        effect_player = player;
        if (card.effect.Eval(this)) {
            card.effect.Use(this);
            if (card is CardLeader leader) leader.used = true;
        }
        effect_player = null;
    }

    public void UseLeaderEffect() {
        if (current_player.has_played) return;
        if (current_player.leader.used) return;
        UseCardEffect(current_player, current_player.leader);
        if (!current_player.leader.used) return;

        current_player.leader.PlayCard(this);
        cursor.Move(Section.HAND);
        current_player.has_played = true;
    }

    public bool ExistsWeather(RowType row) {
        return weathers[row].Item1 is not null;
    }

    public void ClearWeather(RowType row) {
        if (ExistsWeather(row)) {
            weathers[row].Item2.graveyard.Add(weathers[row].Item1);
            weathers[row] = new(null,null);
        }
    }

    public void ClearAllWeathers() {
        foreach (var row in Enum.GetValues(typeof(RowType)).Cast<RowType>())
            ClearWeather(row);
    }

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
        ClearVictor();
    }
    public void StartPhase() {
        scene = new SceneStartPhase();
        if (phase != REDRAW_PHASE) {
            ClearVictor(phase);
            ClearAllWeathers();
            player_1.Clear();
            player_2.Clear();
            player_1.ReceiveCard(Player.PHASE_DRAW_CARDS);
            player_2.ReceiveCard(Player.PHASE_DRAW_CARDS);
        }
        phase += 1;
        switch (phase) {
            case REDRAW_PHASE+2:
                MediaPlayer.Play(bgm_playing2);
                break;
            case REDRAW_PHASE+3:
                MediaPlayer.Play(bgm_playing3);
                break;
        }
    }
    public void StartTurn() {
        // If not Pass > Start next turn
        if (!rival_player.has_passed) current_player = rival_player;
        current_player.has_played = false;
        rival_player.has_played = false;
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
        sfx_pass.Play();
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

            // Check leader effect
            foreach (var p in players) {
                if (
                    p.leader.effect.Type == EffectType.ON_PHASE_END &&
                    !p.leader.used
                ) {
                    UseCardEffect(p, p.leader);
                }
            }

            // Double check victor existance
            if (victor is not null) {
                AddVictor(victor);
            }
            else if (phase_victors[phase].Count == 1) {
                victor = phase_victors[phase][0];
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
        UpdateHandCard();

        if (!help) scene.Update(this);

        // Release Cursor
        if (
            cursor.holding &&
            Keyboard.GetState().GetPressedKeyCount() == 0
        ) cursor.Release();
    }

}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGwent;

public class BattleManager
{
    private class Cursor {
        private const int DEFAULT_INDEX = 0;
        public const int NONE = -1;
        public Section section = Section.HAND;
        public int index = DEFAULT_INDEX;
        public int sub_index = DEFAULT_INDEX;
        public bool holding = false;

        public Texture2D cardSquare;

        public void Move(Section s, int i) {
            section = s;
            index = i;
        }
        public void Move(Section s) {
            Move(s, DEFAULT_INDEX);
        }
        public void Move(int i) {
            Move(section, i);
        }
        public void Hold() {holding = true;}
        public void Release() {holding = false;}

    }
    private enum Scene {
        START_TURN,
        PLAY_TURN,
        END_TURN,
    }
    private enum Section {
        HAND,
        FIELD,
    }

    private const string PLAYER_1_NAME = "Radiant";
    private const string PLAYER_2_NAME = "Dire";
    private const int STARTING_CARDS = 10;
    private const int PREVIEW_CARD_XPOS = 800;
    private const int PREVIEW_CARD_YPOS = 0;
    private const int PREVIEW_CARD_WIDTH = 224;
    private const int PREVIEW_CARD_HEIGHT = 325;
    private const int PREVIEW_CARD_XOFFSET = 55;
    private const int PREVIEW_CARD_YOFFSET = 50;

    private int phase = 0;
    private Scene scene = Scene.START_TURN;
    private Cursor cursor = new ();
    private Player player_1;
    private Player player_2;
    private Player turn;

    private Texture2D background;
    private Texture2D turnEndBackground;

    private Player getRivalPlayer(Player player) {
        return player==player_1? player_2 : player_1;
    }

    private void GameStart() {
        phase += 1;
        player_1.GetCards(STARTING_CARDS);
        player_2.GetCards(STARTING_CARDS);
    }

    private void StartTurn() {
        if (!turn.has_passed) turn = getRivalPlayer(turn);
        cursor.Move(Section.HAND);
        scene = Scene.START_TURN;
    }
    private void PlayTurn() {
        scene = Scene.START_TURN;
    }
    private void EndTurn() {
        scene = Scene.END_TURN;
    }


    public void Initialize(Deck[] decks) {
        player_1 = new Player(PLAYER_1_NAME, decks[0]);
        player_2 = new Player(PLAYER_2_NAME, decks[1]);
        turn = (Random.Shared.Next(1)==0)? player_1 : player_2;
    }

    public void LoadContent(GraphicTools gt) {
        turnEndBackground = new Texture2D(
            gt.graphics.GraphicsDevice,
            gt.graphics.PreferredBackBufferWidth,
            gt.graphics.PreferredBackBufferHeight
        );
        Card.hover_image = gt.content.Load<Texture2D>(Card.HOVER_NAME);
        Card.back_image = gt.content.Load<Texture2D>(Card.BACK_NAME);
        cursor.cardSquare = new Texture2D(gt.graphics.GraphicsDevice, Card.WIDTH, Card.HEIGHT);
        cursor.cardSquare.CreateBorder(5, Color.Red);

        background = gt.content.Load<Texture2D>("desk");
        player_1.LoadContent(gt);
        player_2.LoadContent(gt);
    }

    public void Update() {
        if (phase == 0) GameStart();

        if (
            scene == Scene.END_TURN &&
            Keyboard.GetState().IsKeyDown(Keys.Enter)
        ) StartTurn();

        if (
            scene != Scene.END_TURN &&
            Keyboard.GetState().IsKeyDown(Keys.Tab)
        ) EndTurn();

        if (
            scene == Scene.START_TURN &&
            cursor.section == Section.HAND &&
            cursor.index != 0 &&
            !cursor.holding &&
            Keyboard.GetState().IsKeyDown(Keys.Left)
        ) {
            cursor.Move(cursor.index-1);
            cursor.Hold();
        }

        if (
            scene == Scene.START_TURN &&
            cursor.section == Section.HAND &&
            cursor.index != player_1.hand.Count-1 &&
            !cursor.holding &&
            Keyboard.GetState().IsKeyDown(Keys.Right)
        ) {
            cursor.Move(cursor.index+1);
            cursor.Hold();
        }

        if (
            cursor.holding &&
            Keyboard.GetState().IsKeyUp(Keys.Up) &&
            Keyboard.GetState().IsKeyUp(Keys.Down) &&
            Keyboard.GetState().IsKeyUp(Keys.Left) &&
            Keyboard.GetState().IsKeyUp(Keys.Right)
        ) cursor.Release();
    }

    public void Draw(GraphicTools gt) {
        // Draw Board
        gt.spriteBatch.Begin();
        gt.spriteBatch.Draw(background, new Vector2(0,0), Color.White);
        gt.spriteBatch.End();

        // Draw Fields
        player_1.Draw(gt, turn == player_1);
        player_2.Draw(gt, turn == player_2);

        // Draw Cursor
        gt.spriteBatch.Begin();
        if (cursor.section == Section.HAND) {
            var position = Card._GetRowPosition(
                cursor.index,
                turn.hand.Count,
                Player.HAND_XPOS,
                Player.HAND_PLAYER_YPOS,
                Player.HAND_WIDTH
            );
            gt.spriteBatch.Draw(
                cursor.cardSquare,
                position,
                Color.Red
            );
        }
        gt.spriteBatch.End();

        // Draw Selected Card
        if (
            scene == Scene.START_TURN &&
            cursor.index != Cursor.NONE
        ) {
            void DrawSelectedCard(Card card) {
                gt.spriteBatch.Begin();
                gt.spriteBatch.Draw(
                    card.image,
                    new Vector2(PREVIEW_CARD_XPOS+PREVIEW_CARD_XOFFSET, PREVIEW_CARD_YPOS+PREVIEW_CARD_YOFFSET),
                    null,
                    Color.White,
                    new Vector2(0.3125f, 0.3640f)
                );
                gt.spriteBatch.Draw(
                    Card.hover_image,
                    new Vector2(PREVIEW_CARD_XPOS, PREVIEW_CARD_YPOS),
                    null,
                    Color.White,
                    0.4375f
                );
                gt.spriteBatch.End();
            }
            if (cursor.section == Section.HAND) {
                if (player_1.hand.Count != 0) {
                    DrawSelectedCard(turn.hand[cursor.index]);
                }
            } else if (cursor.section == Section.FIELD) {
                if (cursor.sub_index != Cursor.NONE) {
                    DrawSelectedCard(turn.card_rows[cursor.index][cursor.sub_index]);
                }
            }
        }

        // Draw End Turn Background
        if (scene == Scene.END_TURN) {
            gt.spriteBatch.Begin();
            gt.spriteBatch.Draw(turnEndBackground, new Vector2(0,0), Color.Black * 0.8f);
            gt.spriteBatch.End();
        }
    }

}

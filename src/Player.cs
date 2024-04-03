using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGwent;

public class Player
{

    public const int PLAYER_LABEL_XPOS = 30;
    public const int PLAYER_LABEL_PLAYER_YPOS = 445;
    public const int PLAYER_LABEL_RIVAL_YPOS = 240;
    public const int PLAYER_LABEL_WIDTH = 100;
    public const int PLAYER_LABEL_HEIGHT = 30;

    public const int HAND_XPOS = 6;
    public const int HAND_PLAYER_YPOS = 640;
    public const int HAND_RIVAL_YPOS = 0;
    public const int HAND_WIDTH = 790;
    public const int HAND_HEIGHT = 80;

    public const int DECK_XPOS = 710;
    public const int DECK_PLAYER_YPOS = 400;
    public const int DECK_RIVAL_YPOS = 240;

    public const float CARD_THUMBNAIL_SCALE = 0.1040f;

    public const int ROW_POWER_XPOS = 165;
    public const int ROW_POWER_YPOS = 360;
    public static Dictionary<RowType,int> ROW_POWER_OFFSET = new Dictionary<RowType,int> {
        {RowType.MELEE, 30},
        {RowType.RANGE, 120},
        {RowType.SIEGE, 210}
    };

    public string name;
    public Dictionary<int, List<Card>> card_rows;
    public List<Card> hand;
    public Stack<Card> deck;
    public List<Card> graveyard;
    public Card leader;
    public bool has_passed = false;

    private SpriteFont arial;

    public Player(string name, Deck deck) {
        this.name = name;
        card_rows = new Dictionary<int, List<Card>>();
        foreach (int rt in Enum.GetValues(typeof(RowType))) {
            card_rows[rt] = [];
        }

        hand = [];
        this.deck = new Stack<Card>(deck.OrderBy(x=>Random.Shared.Next()).ToList());
        graveyard = [];
        leader = null;
    }

    public void GetCards(int count=1) {
        while (count > 0 && deck.Count != 0) {
            hand.Add(deck.Pop());
            count -= 1;
        }
    }

    public void LoadContent(GraphicTools gt) {
        arial = gt.content.Load<SpriteFont>("Arial");
        foreach (var card in deck) {
            card.LoadContent(gt);
        }
    }

    public void Draw(GraphicTools gt, bool is_turn) {
        // Draw Player Label
        Vector2 size = arial.MeasureString(name);
        int xpos = PLAYER_LABEL_XPOS + (PLAYER_LABEL_WIDTH/2 - (int)size.X/2);
        int ypos = (is_turn == true ? PLAYER_LABEL_PLAYER_YPOS : PLAYER_LABEL_RIVAL_YPOS)
            + (PLAYER_LABEL_HEIGHT/2 - (int)size.Y/2);
        gt.spriteBatch.Begin();
        gt.spriteBatch.DrawString(arial, name, new Vector2(xpos, ypos), Color.Black);
        gt.spriteBatch.End();

        // Draw Player Hand
        gt.spriteBatch.Begin();
        for (int i = 0; i<hand.Count; i++) {
            var card = hand[i];
            var position = card.GetRowPosition(
                i,
                hand.Count,
                HAND_XPOS,
                is_turn? HAND_PLAYER_YPOS : HAND_RIVAL_YPOS,
                HAND_WIDTH 
            );
            gt.spriteBatch.Draw(
                is_turn? card.image : Card.back_image,
                position,
                null,
                Color.White,
                CARD_THUMBNAIL_SCALE
            );
        }
        gt.spriteBatch.End();

        // Draw Player Deck
        if (deck.Count != 0) {
            gt.spriteBatch.Begin();
            gt.spriteBatch.Draw(
                Card.back_image,
                new Vector2(DECK_XPOS, is_turn? DECK_PLAYER_YPOS : DECK_RIVAL_YPOS),
                null,
                Color.White,
                CARD_THUMBNAIL_SCALE
            );
            gt.spriteBatch.End();
        }
    }

}

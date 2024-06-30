using Microsoft.Xna.Framework.Input;

namespace MonoGwent;


public class SceneRedraw : IScene {

    public void Update(BattleManager bm) {
        // Check for previous input release
        if (!bm.Cursor.holding) {

            // Input > Draw card
            if (
                Keyboard.GetState().IsKeyDown(Keys.Enter)
            ) {
                bm.Cursor.Hold();
                var selected_card = bm.Current.hand[bm.Cursor.index];
                if (!bm.Current.selected.Contains(selected_card)) {
                    if (bm.Current.selected.Count != 2) {
                        bm.Current.selected.Add(bm.Current.hand[bm.Cursor.index]);
                    }
                } else {
                    bm.Current.selected.Remove(selected_card);
                }
            }

            // Await for input > Finish redrawing
            else if (
                !bm.Cursor.holding &&
                Keyboard.GetState().IsKeyDown(Keys.Tab)
            ) {
                if (bm.Rival.has_passed) {
                    foreach (var player in bm.Players) {
                        player.ReceiveCard(player.selected.Count, true);
                        foreach (var card in player.selected) {
                            player.deck.Add(card);
                            player.deck.Shuffle();
                            player.hand.Remove(card);
                        }
                        player.selected.Clear();
                        player.has_passed = false;
                    }
                    bm.EndPhase();
                } else {
                    bm.Current.has_passed = true;
                    bm.EndTurn();
                }
            }

            // Move Right
            else if (
                !bm.Cursor.holding &&
                bm.Current.hand.Count != 0 &&
                Keyboard.GetState().IsKeyDown(Keys.Right)
            ) {
                if (bm.Cursor.index == bm.Current.hand.Count-1)
                {bm.Cursor.Move(0);} else {bm.Cursor.Move(bm.Cursor.index+1);}
            }
            // Move Left
            else if (
                !bm.Cursor.holding &&
                bm.Current.hand.Count != 0 &&
                Keyboard.GetState().IsKeyDown(Keys.Left)
            ) {
                if (bm.Cursor.index == 0)
                {bm.Cursor.Move(bm.Current.hand.Count-1);} else {bm.Cursor.Move(bm.Cursor.index-1);}
            }

        }
    }
}

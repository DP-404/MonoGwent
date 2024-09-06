using Microsoft.Xna.Framework.Input;

namespace MonoGwent;


public class SceneEndPhase : IScene {

    public void Update(BattleManager bm) {
        // Check for previous input release
        if (!bm.Cursor.holding) {

            // Either play is defeated > Game Over
            if (
                bm.Player1.IsDefeated()
                || bm.Player2.IsDefeated()
            ) {
                // Input > End Game
                if (
                    Keyboard.GetState().IsKeyDown(Keys.Enter)
                ) {
                    bm.Cursor.Hold();
                    bm.EndGame();
                }
            } else {
                // Redraw
                if (bm.Phase != BattleManager.REDRAW_PHASE) {
                    // Await for input > Start Next Phase
                    if (
                        Keyboard.GetState().IsKeyDown(Keys.Enter)
                    ) {
                        bm.Cursor.Hold();
                        bm.StartPhase();
                    }
                // Playing > Start Next Phase
                } else {
                    bm.Cursor.Hold();
                    bm.StartPhase();
                }
            }

        }
    }
}

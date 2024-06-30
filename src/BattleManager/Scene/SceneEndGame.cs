using Microsoft.Xna.Framework.Input;

namespace MonoGwent;


public class SceneEndGame : IScene {

    public void Update(BattleManager bm) {
        // Check for previous input release
        if (!bm.Cursor.holding) {

            // Input > Start new game
            if (
                Keyboard.GetState().IsKeyDown(Keys.Enter)
            ) {
                bm.Cursor.Hold();
                bm.NewGame();
            }

        }
    }
}

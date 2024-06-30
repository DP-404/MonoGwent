using Microsoft.Xna.Framework.Input;

namespace MonoGwent;


public class SceneStartPhase : IScene {

    public void Update(BattleManager bm) {
        // Check for previous input release
        if (!bm.Cursor.holding) {

            // Input > Start turn
            if (
                Keyboard.GetState().IsKeyDown(Keys.Enter)
            ) {
                bm.Cursor.Hold();
                bm.StartTurn();
            }

        }
    }
}

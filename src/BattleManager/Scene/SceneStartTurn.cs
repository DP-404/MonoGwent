using Microsoft.Xna.Framework.Input;

namespace MonoGwent;


public class SceneStartTurn : IScene {

    public void Update(BattleManager bm) {
        // Check for previous input release
        if (!bm.Cursor.holding) {

            // Input > Play turn
            if (
                Keyboard.GetState().IsKeyDown(Keys.Enter)
            ) {
                bm.Cursor.Hold();
                bm.PlayTurn();
            }

        }
    }
}

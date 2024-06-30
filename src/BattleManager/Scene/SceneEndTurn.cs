using Microsoft.Xna.Framework.Input;

namespace MonoGwent;


public class SceneEndTurn : IScene {

    public void Update(BattleManager bm) {
        // Not Pass or Redraw > Start next turn
        if (
            bm.Current.has_played ||
            bm.Phase == BattleManager.REDRAW_PHASE
        ) {
            bm.StartTurn();
        }

        // Pass, await Input
        else if (
            bm.Current.has_passed &&
            Keyboard.GetState().IsKeyDown(Keys.Enter)
        ) {
            bm.Cursor.Move(Section.HAND);
            // If both passed > End phase / Start next turn
            if (bm.Rival.has_passed) {
                bm.EndPhase();
            } else {
                bm.StartTurn();
            }
        }
    }
}

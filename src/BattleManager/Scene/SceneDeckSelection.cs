using Microsoft.Xna.Framework.Input;

namespace MonoGwent;

public class SceneDeckSelection : IScene {

    public void Update(BattleManager bm) {
        // Check for previous input release
        if (!bm.Cursor.holding) {

            // Await for input
            if (
                bm.Current.name.Length != 0 &&
                Keyboard.GetState().IsKeyDown(Keys.Enter)
            ) {
                bm.SfxSelect.Play();
                bm.Cursor.Move(0);
                if (bm.Current == bm.Player1) {
                    bm.Current = bm.Rival;
                } else {
                    bm.NewGame();
                }
            }
            // Erase
            else if (
                bm.Current.name.Length != 0 &&
                Keyboard.GetState().IsKeyDown(Keys.Back)
            ) {
                bm.Cursor.Hold();
                bm.Current.name = bm.Current.name.Remove(bm.Current.name.Length-1);
            }
            // Move Right
            else if (
                Keyboard.GetState().IsKeyDown(Keys.Right)
            ) {
                bm.SfxSelect.Play();
                if (bm.Cursor.index == DecksDump.Count-1)
                {bm.Cursor.Move(0);} else {bm.Cursor.Move(bm.Cursor.index+1);}
                bm.Current.original_deck = DecksDump.GetDeck(bm.Cursor.index);
            }
            // Move Left
            else if (
                Keyboard.GetState().IsKeyDown(Keys.Left)
            ) {
                bm.SfxSelect.Play();
                if (bm.Cursor.index == 0)
                {bm.Cursor.Move(DecksDump.Count-1);} else {bm.Cursor.Move(bm.Cursor.index-1);}
                bm.Current.original_deck = DecksDump.GetDeck(bm.Cursor.index);
            }
            // Read Keys
            else if (
                bm.Current.name.Length != Player.MAX_NAME_LENGTH &&
                Keyboard.GetState().GetPressedKeys().Length != 0
            ) {
                var key_string = Keyboard.GetState().GetPressedKeys()[0].ToString();
                if (key_string.Length == 1) {
                    var key = key_string[0];
                    if (char.IsLetter(key)) {
                        bm.Cursor.Hold();
                        if (
                            Keyboard.GetState().IsKeyDown(Keys.LeftShift) ||
                            Keyboard.GetState().IsKeyDown(Keys.RightShift)
                        ) {
                            key = char.ToUpper(key);
                        } else {
                            key = char.ToLower(key);
                        }
                        bm.Current.name += key;
                    }
                }
            }

        }
    }
}

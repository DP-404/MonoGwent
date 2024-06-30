using System;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace MonoGwent;


public class ScenePlayTurn : IScene {

    public void Update(BattleManager bm) {
        // Check for previous input release
        if (!bm.Cursor.holding) {

            switch (bm.Cursor.section) {

                // Cursor in Hand
                case Section.HAND:

                    // Pass turn
                    if (
                        bm.Scene is not SceneEndTurn &&
                        Keyboard.GetState().IsKeyDown(Keys.Tab)
                    ) {
                        bm.Current.has_passed = true;
                        bm.EndTurn();
                    }

                    // Move Right
                    else if (
                        bm.Current.hand.Count != 0 &&
                        Keyboard.GetState().IsKeyDown(Keys.Right)
                    ) {
                        if (bm.Cursor.index == bm.Current.hand.Count-1)
                        {bm.Cursor.Move(0);} else {bm.Cursor.Move(bm.Cursor.index+1);}
                    }
                    // Move Left
                    else if (
                        bm.Current.hand.Count != 0 &&
                        Keyboard.GetState().IsKeyDown(Keys.Left)
                    ) {
                        if (bm.Cursor.index == 0)
                        {bm.Cursor.Move(bm.Current.hand.Count-1);} else {bm.Cursor.Move(bm.Cursor.index-1);}
                    }
                    // Select Card > Move to Field
                    else if (
                        bm.Current.hand.Count != 0 &&
                        Keyboard.GetState().IsKeyDown(Keys.Enter)
                    ) {
                        bm.SfxSelect.Play();
                        bm.Cursor.hand = bm.Cursor.index;
                        bm.Cursor.Move(Section.FIELD);
                    }
                    // Select Leader
                    else if (
                        Keyboard.GetState().IsKeyDown(Keys.RightShift)
                    ) {
                        bm.SfxSelect.Play();
                        bm.Cursor.Move(Section.LEADER);
                    }

                    break;

                // Cursor in Field
                case Section.FIELD:

                    // Move Down/Right
                    if (
                        // No Weather Card => Rows are a Column
                        (
                            bm.HandCard is not CardWeather &&
                            Keyboard.GetState().IsKeyDown(Keys.Down)
                        )
                            ||
                        // Weather Card => Rows are a Row
                        (
                            bm.HandCard is CardWeather &&
                            Keyboard.GetState().IsKeyDown(Keys.Right)
                        )
                    ) {
                        if (bm.Cursor.index == Enum.GetNames(typeof(RowType)).Length-1) {
                            bm.Cursor.Move(0);
                        } else {
                            bm.Cursor.Move(bm.Cursor.index+1);
                        }
                    }
                    // Move Up/Left
                    else if (
                        (
                        // No Weather Card => Rows are a Column
                            bm.HandCard is not CardWeather &&
                            Keyboard.GetState().IsKeyDown(Keys.Up)
                        )
                            ||
                        // Weather Card => Rows are a Row
                        (
                            bm.HandCard is CardWeather &&
                            Keyboard.GetState().IsKeyDown(Keys.Left)
                        )
                    ) {
                        if (bm.Cursor.index == 0) {
                            bm.Cursor.Move(Enum.GetNames(typeof(RowType)).Length-1);
                        } else {
                            bm.Cursor.Move(bm.Cursor.index-1);
                        }
                    }

                    // Selected card is Unit and no Field selected
                    else if (
                        bm.HandCard is CardUnit &&
                        bm.Cursor.field == Cursor.NONE &&
                        Keyboard.GetState().IsKeyDown(Keys.Enter)
                    ) {
                        // Selected card is Decoy > Move to Row
                        if (
                            ((CardUnit)bm.HandCard).is_decoy
                        ) {
                            if (bm.HandCard.types.Contains((RowType)bm.Cursor.index)) {
                                bm.SfxSelect.Play();
                                bm.Cursor.field = bm.Cursor.index;
                                bm.Cursor.Move(Section.ROW);
                            }
                        }
                        // Selected card is Unit > Play Card
                        else {
                            bm.PlayCard();
                        }
                    }

                    // Selected card is Weather
                    else if (
                        bm.HandCard is CardWeather &&
                        Keyboard.GetState().IsKeyDown(Keys.Enter)
                    ) {
                        bm.PlayCard();
                    }

                    // Selected card is Boost
                    else if (
                        bm.HandCard is CardBoost &&
                        Keyboard.GetState().IsKeyDown(Keys.Enter)
                    ) {
                        bm.PlayCard();
                    }

                    // Cancelling
                    else if (
                        Keyboard.GetState().IsKeyDown(Keys.Back)
                    ) {
                        bm.SfxCancel.Play();
                        bm.Cursor.Move(Section.HAND, bm.Cursor.hand);
                    }
                    break;

                // Cursor in Row
                case Section.ROW:

                    // Selected card is Decoy > Play Card
                    if (
                        bm.HandCard is CardUnit &&
                        ((CardUnit)bm.HandCard).is_decoy &&
                        bm.Current.rows[(RowType)bm.Cursor.field].Count != 0 &&
                        !((CardUnit)bm.Current.GetFieldCard((RowType)bm.Cursor.field, bm.Cursor.index)).is_decoy &&
                        Keyboard.GetState().IsKeyDown(Keys.Enter)
                    ) {
                        bm.PlayCard();
                    }

                    // Move Right
                    else if (
                        bm.Current.rows[(RowType)bm.Cursor.field].Count != 0 &&
                        Keyboard.GetState().IsKeyDown(Keys.Right)
                    ) {
                        if (bm.Cursor.index == bm.Current.rows[(RowType)bm.Cursor.field].Count-1)
                        {bm.Cursor.Move(0);} else {bm.Cursor.Move(bm.Cursor.index+1);}
                    }
                    // Move Left
                    else if (
                        bm.Current.rows[(RowType)bm.Cursor.field].Count != 0 &&
                        Keyboard.GetState().IsKeyDown(Keys.Left)
                    ) {
                        if (bm.Cursor.index == 0)
                        {bm.Cursor.Move(bm.Current.rows[(RowType)bm.Cursor.field].Count-1);} else {bm.Cursor.Move(bm.Cursor.index-1);}
                    }

                    // Cancelling
                    else if (
                        Keyboard.GetState().IsKeyDown(Keys.Back)
                    ) {
                        bm.SfxCancel.Play();
                        bm.Cursor.Move(Section.FIELD, bm.Cursor.field);
                    }
                    break;

                // bm.Cursor in Leader
                case Section.LEADER:

                    // Use Leader
                    if (
                        !bm.Current.leader.used &&
                        Keyboard.GetState().IsKeyDown(Keys.Enter)
                    ) {
                        bm.UseLeaderEffect();
                    }

                    // Cancelling
                    else if (
                        !bm.Cursor.holding &&
                        Keyboard.GetState().IsKeyDown(Keys.Back)
                    ) {
                        bm.SfxCancel.Play();
                        bm.Cursor.Move(Section.HAND);
                    }
                    break;
            }

        }
    }
}

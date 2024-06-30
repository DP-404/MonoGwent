using System;

namespace MonoGwent;

public class CardUnit : Card {
    public const int POWER_DECOY = 0;
    private const string TYPE_UNIT_NAME = "Unit";
    private const string TYPE_SILVER_NAME = " (Silver)";
    private const string TYPE_GOLDEN_NAME = " (Golden)";
    private const string TYPE_DECOY_NAME = "Decoy";

    public override string type_name {get =>
    (!is_decoy)? (TYPE_UNIT_NAME + (is_hero? TYPE_GOLDEN_NAME : TYPE_SILVER_NAME))
    : TYPE_DECOY_NAME;}
    public bool is_hero {get; init;}
    public int power;

    public bool is_decoy {get => power == POWER_DECOY? true : false;}

    public int GetPower(CardWeather weather, CardBoost boost) {
        if (is_hero) return power;
        var actual_power = power;
        if (boost is not null) actual_power += boost.bonus;
        if (weather is not null) actual_power = Math.Min(actual_power, weather.penalty);
        return actual_power;
    }

    public override bool PlayCard(BattleManager bm)
    {
        if (is_decoy) {
            var takeback_card = (CardUnit)bm.Current.GetFieldCard(
                (RowType)bm.Cursor.field,
                bm.Cursor.index
                );

            // If takeback card is Hero > Return
            if (takeback_card.is_hero) return false;

            // Take field card to hand
            bm.Current.rows[(RowType)bm.Cursor.field].Remove(takeback_card);
            bm.Current.hand.Add(takeback_card);

            // Place card on field card's place
            bm.Current.rows[(RowType)bm.Cursor.field].Insert(bm.Cursor.index, this);
            bm.Current.hand.Remove(bm.HandCard);
        } else {
            // Place card on field
            bm.Current.rows[(RowType)bm.Cursor.index].Add(this);
            bm.Current.hand.Remove(this);
        }

        bm.UseCardEffect(bm.Current, this);
        return true;
    }
}
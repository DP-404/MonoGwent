using System;

namespace MonoGwent;

public class CardUnit : Card {
    public const int POWER_DECOY = 0;
    public const int DEFAULT_MODIFIED_POWER = -1;
    private const string TYPE_UNIT_NAME = "Unit";
    private const string TYPE_SILVER_NAME = " (Silver)";
    private const string TYPE_GOLDEN_NAME = " (Golden)";
    private const string TYPE_DECOY_NAME = "Decoy";

    public override string type_name {get =>
    (!is_decoy)? (TYPE_UNIT_NAME + (is_hero? TYPE_GOLDEN_NAME : TYPE_SILVER_NAME))
    : TYPE_DECOY_NAME;}
    public bool is_hero {get; init;}
    public int power;
    public int modified_power = DEFAULT_MODIFIED_POWER;
    public bool is_decoy {get => power == POWER_DECOY? true : false;}

    public int ActualPower {get => is_decoy? POWER_DECOY : (modified_power != DEFAULT_MODIFIED_POWER? modified_power : power);}

    public int GetPower(CardWeather weather, CardBoost boost) {
        if (is_hero) return ActualPower;
        var final_power = ActualPower;
        if (boost is not null) final_power += boost.bonus;
        if (weather is not null) final_power = Math.Min(final_power, weather.penalty);
        return final_power;
    }
    public override void Dispose() {
        modified_power = DEFAULT_MODIFIED_POWER;
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
            bm.Current.Retrieve(takeback_card);

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
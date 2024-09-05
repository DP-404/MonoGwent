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
    public bool is_decoy {get => power == POWER_DECOY? true : false;}

    public new int power {
        get => modified_power;
        set => modified_power =
            is_hero?
                original_power
                : (value < 0)?
                    0
                    : value
            ;
    }
    public int ActualPower {get => 
        is_decoy?
            POWER_DECOY
            : is_hero?
                original_power
                : power;
    }

    public override CardUnit Copy() {
        return (CardUnit)base.Copy();
    }

    public int GetPower(CardWeather weather, CardBoost boost) {
        if (is_hero) return ActualPower;
        var final_power = ActualPower;
        if (boost is not null) final_power += boost.bonus;
        if (weather is not null) final_power = Math.Min(final_power, weather.penalty);
        return final_power;
    }
    public override void Dispose() {
        power = original_power;
        position = null;
    }
    public override bool PlayCard(BattleManager bm)
    {
        if (is_decoy) {
            var pos = (RowType)bm.Cursor.field;

            var takeback_card = (CardUnit)bm.Current.GetFieldCard(
                pos,
                bm.Cursor.index
                );

            // If takeback card is Hero > Return
            if (takeback_card.is_hero) return false;

            // Take field card to hand
            var index = bm.Current.field.IndexOf(takeback_card);
            bm.Current.field.Remove(takeback_card);
            bm.Current.Retrieve(takeback_card);
            takeback_card.Dispose();

            // Place card on field card's place
            position = pos;
            bm.Current.field.Insert(index, this);
            bm.Current.hand.Remove(bm.HandCard);
        } else {
            var pos = (RowType)bm.Cursor.index;

            // Place card on field
            position = pos;
            bm.Current.field.Add(this);
            bm.Current.hand.Remove(this);
        }

        bm.UseCardEffect(bm.Current, this);
        return true;
    }
}
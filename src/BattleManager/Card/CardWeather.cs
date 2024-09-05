using System;
using System.Linq;

namespace MonoGwent;

public class CardWeather : Card {
    public const int DISPEL_PENALTY = -1;
    public const int DEFAULT_PENALTY = 1;

    public override string type_name {get => (!is_dispel)? "Weather" : "Dispel";}
    public int penalty = DEFAULT_PENALTY;
    public bool is_dispel {get => penalty == DISPEL_PENALTY;}

    public override CardWeather Copy() {
        return (CardWeather)base.Copy();
    }
    public override bool PlayCard(BattleManager bm)
    {
        RowType pos;
        if (bm.Cursor.field == Cursor.NONE) {
            pos = (RowType)bm.Cursor.index;
        } else {
            pos = (RowType)bm.Cursor.field;
        }

        // Card is Weather
        if (!is_dispel) {
            bm.Current.hand.Remove(this);
            var old_weather = bm.Weathers[pos];
            if (old_weather.Item1 is not null) {
                old_weather.Item2.DisposeOf(old_weather.Item1);
            }
            bm.Weathers[pos] = new (this, bm.Current);
            position = pos;
        }
        // Card is Dispel
        else {
            var exists_weather = false;
            var is_single = types.Length != 0;
            if (is_single) {
                exists_weather = bm.Weathers[pos].Item1 is not null;
            } else {
                exists_weather = Enum.GetValues(typeof(RowType))
                    .Cast<RowType>()
                    .Select(row => bm.Weathers[row].Item1)
                    .Any(w => w is not null);
            }

            if (!exists_weather) return false;

            bm.Current.DisposeOf(this);
            bm.Current.hand.Remove(this);

            // Dispel All
            if (types.Length == 0) bm.ClearAllWeathers();
            // Dispel Single
            else bm.ClearWeather(pos);
        }

        return true;
    }
}

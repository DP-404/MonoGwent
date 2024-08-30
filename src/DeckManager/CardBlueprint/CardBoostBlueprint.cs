using System.Collections.Generic;

namespace MonoGwent;

public class CardBoostBlueprint : CardBlueprint {
    public override string type_name {get => "Boost";}
    public int bonus;

    public override Card GetCard() {
        return new CardBoost {
            name=name,
            description=description,
            faction=faction,
            img_name=image_name,
            img=image,
            types=types,
            bonus=bonus
        };
    }
}

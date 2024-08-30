using System.Collections.Generic;

namespace MonoGwent;

public class CardUnitBlueprint : CardBlueprint {
    public override string type_name {get => "Unit";}
    public int power;
    public bool is_hero = false;
    public List<IEffect> effects = new ();

    public override Card GetCard() {
        return new CardUnit {
            name=name,
            description=description,
            faction=faction,
            img_name=image_name,
            img=image,
            types=types,
            power=power,
            is_hero=is_hero,
            effects=effects
        };
    }
}

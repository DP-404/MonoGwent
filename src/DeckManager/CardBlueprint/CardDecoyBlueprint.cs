

namespace MonoGwent;

public class CardDecoyBlueprint : CardBlueprint {
    public override string type_name {get => "Decoy";}

    public override Card GetCard() {
        return new CardUnit {
            name=name,
            description=description,
            faction=faction,
            img_name=image_name,
            img=image,
            types=types,
            power=CardUnit.POWER_DECOY,
            is_hero=false
        };
    }
}

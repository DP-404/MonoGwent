

namespace MonoGwent;

public class CardDecoyBlueprint : CardBlueprint {
    public override string type_name {get => "Decoy";}

    public override Card GetCard() {
        return new CardUnit {
            blueprint=this,
            name=name,
            description=description,
            faction=faction,
            img_name=image_name,
            img=image,
            types=types,
            original_power=CardUnit.POWER_DECOY,
            power=CardUnit.POWER_DECOY,
            is_hero=false
        };
    }
}

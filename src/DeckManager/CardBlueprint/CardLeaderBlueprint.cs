using System.Collections.Generic;

namespace MonoGwent;

public class CardLeaderBlueprint : CardBlueprint {
    public override string type_name {get => "Leader";}

    public override Card GetCard() {
        return new CardLeader {
            blueprint=this,
            name=name,
            description=description,
            faction=faction,
            img_name=image_name,
            img=image,
            types=types,
            effects=GetEffects()
        };
    }
}

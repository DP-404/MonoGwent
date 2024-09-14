

namespace MonoGwent;

public class CardDispelBlueprint : CardBlueprint {
    public override string type_name {get => "Dispel";}

    public override Card GetCard() {
        return new CardWeather {
            blueprint=this,
            name=name,
            description=description,
            faction=faction,
            img_name=image_name,
            img=image,
            types=types,
            penalty=CardWeather.DISPEL_PENALTY,
            effects=GetEffects()
        };
    }
}

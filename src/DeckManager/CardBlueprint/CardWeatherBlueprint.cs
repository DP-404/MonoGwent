

namespace MonoGwent;

public class CardWeatherBlueprint : CardBlueprint {
    public override string type_name {get => "Weather";}
    public uint penalty = CardWeather.DEFAULT_PENALTY;

    public override Card GetCard() {
        return new CardWeather {
            name=name,
            description=description,
            faction=faction,
            img_name=image_name,
            img=image,
            types=types,
            penalty=(int)penalty
        };
    }
}

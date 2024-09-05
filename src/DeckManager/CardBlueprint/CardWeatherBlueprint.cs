

namespace MonoGwent;

public class CardWeatherBlueprint : CardBlueprint {
    public override string type_name {get => "Weather";}
    public int power = CardWeather.DEFAULT_PENALTY;

    public override Card GetCard() {
        return new CardWeather {
            blueprint=this,
            name=name,
            description=description,
            faction=faction,
            img_name=image_name,
            img=image,
            types=types,
            penalty=power
        };
    }
}

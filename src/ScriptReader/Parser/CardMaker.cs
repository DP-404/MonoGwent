using System;
using System.Collections.Generic;


namespace MonoGwent;

public class CardMaker
{

    public CardBlueprint blueprint = new();

    public void Create(List<Property> properties, List<Effect> effects)
    {
        var type = GetCardType(properties);
        switch (GetCardType(properties)) {
            case "Leader":
                blueprint = new CardLeaderBlueprint();
                break;
            case "Gold":
                blueprint = new CardUnitBlueprint() {is_hero=true};
                break;
            case "Silver":
                blueprint = new CardUnitBlueprint();
                break;
            case "Decoy":
                blueprint = new CardDecoyBlueprint();
                break;
            case "Weather":
                blueprint = new CardWeatherBlueprint();
                break;
            case "Dispel":
                blueprint = new CardDispelBlueprint();
                break;
            case "Boost":
                blueprint = new CardBoostBlueprint();
                break;
            default:
                throw new Exception($"Case mismatch: {type}");
        }

        SetProperties(blueprint, properties);

        foreach (var effect in effects)
            switch (blueprint) {
                case CardUnitBlueprint c:
                    c.effects.Add((IEffect)effect.Clone());
                    break;
                case CardLeaderBlueprint c:
                    c.effects.Add((IEffect)effect.Clone());
                    break;
            }

        CardsDump.blueprints.Add(blueprint);
    }

    // Add actual card properties values
    public void SetProperties(CardBlueprint blueprint, List<Property> properties)
    {
        foreach (var prop in properties) {
            // Add according to property
            switch (prop.type) {
                case "Type":
                    break;
                case "Name":
                    blueprint.name = (string)prop.value;
                    break;
                case "Faction":
                    blueprint.faction = (string)prop.value;
                    break;
                case "Power":
                    switch (blueprint) {
                        case CardUnitBlueprint c:
                            c.power = (int)prop.value;
                            break;
                        case CardWeatherBlueprint c:
                            c.penalty = (int)prop.value;
                            break;
                        case CardBoostBlueprint c:
                            c.bonus = (int)prop.value;
                            break;
                    }
                    break;
                case "Range":
                    foreach (var r in Enum.GetNames(typeof(RowType))) {
                        if (((string)prop.value).Contains(char.ToUpper(r[0])+r[1..].ToLower())) {
                            blueprint.types = [
                                .. blueprint.types,
                                (RowType)Enum.Parse(typeof(RowType), ((string)prop.value).ToUpper())
                            ];
                        }
                    }
                    break;
            }
        }
    }

    public string GetCardType(List<Property> properties)
    {
        foreach(var prop in properties)
        {
            if (prop.type == "Type")
                return (string)prop.value;
        }
        throw new Exception("Missing card type.");
    }
}
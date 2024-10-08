using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework.Graphics;

namespace MonoGwent;

public class CardBlueprint {

    public string name = "";
    public string description = "";
    public string faction = Card.NEUTRAL_FACTION;
    public virtual string type_name {get;}
    public string image_name {get => faction.ToLower().Replace(' ','-')+"_"+type_name.ToLower()+"_"+name.ToLower().Replace(' ','-');}
    public RowType[] types = [];
    public List<IEffect> effects = new ();

    public Texture2D image;

    public void LoadContent(GameTools gt) {
        string actual_image_name;
        if (File.Exists(Path.Join(
            "Content",
            Card.IMAGES_PATH,
            image_name+".xnb"
        ))) {
            actual_image_name = Path.Join(
                Card.IMAGES_PATH,
                image_name
            );
        } else {
            actual_image_name = Path.Join(
                Card.IMAGES_PATH,
                Card.DEFAULT_IMAGE+"_"+type_name.ToLower()
            );
        }
        image = gt.content.Load<Texture2D>(actual_image_name);
    }

    protected List<IEffect> GetEffects() {
        List<IEffect> new_effects = new();
        foreach (var e in effects)
            new_effects.Add((IEffect)e.Clone());
        return new_effects;
    }
    public virtual Card GetCard() {
        throw new NotImplementedException();
    }
}

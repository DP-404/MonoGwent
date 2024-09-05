using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGwent;

public class Deck {

    public string name { get => leader.faction;}
    private string image_name { get => name.ToLower().Replace(' ','-')+"_cover";}
    public string owner;
    private Dictionary<CardBlueprint,int> card_blueprints;
    private CardLeaderBlueprint leader_blueprint;

    public CardLeader leader;
    private List<Card> cards = new();

    public List<Card> Cards {get => cards;}
    public int Count {get => cards.Count;}

    public Texture2D img;

    public Deck() {}
    public Deck(Dictionary<CardBlueprint,int> card_blueprints, CardLeaderBlueprint leader_blueprint) {
        this.card_blueprints = card_blueprints;
        this.leader_blueprint = leader_blueprint;
    }
    public void SetOwner(string owner) {
        foreach (var c in cards) {
            c.owner = owner;
        }
    }
    public void Populate() {
        cards = new();
        foreach (var c in card_blueprints) {
            if (
                c.Key.faction != leader_blueprint.faction
                && c.Key.faction != Card.NEUTRAL_FACTION
            ) {
                throw new Exception($"Leader card faction mismatch: [{c.Key.faction}]{c.Key.name}");
            }
            for (int i = c.Value; i > 0; i--) {
                Add(c.Key.GetCard());
            }
        }
        leader = (CardLeader)leader_blueprint.GetCard();
    }
    public void Add(Card card) {
        cards.Add(card);
    }
    public Card Take() {
        var c = cards[^1];
        cards.Remove(c);
        return c;
    }
    public void Remove(Card card) {
        cards.Remove(card);
    }
    public void Shuffle() {
        var shuffled = new Deck();
        foreach (var c in cards.OrderBy(x=>Random.Shared.Next())) shuffled.Add(c);
        shuffled.leader = leader;
        Copy(shuffled);
    }

    public void Copy(Deck deck) {
        cards.Clear();
        foreach (var i in deck.cards) Add(i.Copy());
        leader = deck.leader.Copy();
        img = deck.img;
    }
    public Deck GetCopy() {
        var deck = new Deck();
        deck.Copy(this);
        return deck;
    }

    public void LoadContent(GameTools gt) {
        string actual_image_name;
        if (File.Exists(Path.Join(
            "Content",
            Card.IMAGES_PATH,
            image_name+".xnb"
        ))) {
            actual_image_name = image_name;
        } else {
            actual_image_name = image_name.Replace(
                name.ToLower().Replace(' ','-'),
                Card.DEFAULT_IMAGE
            );
        }
        img = gt.content.Load<Texture2D>(Path.Join(Card.IMAGES_PATH, actual_image_name));
    }
}
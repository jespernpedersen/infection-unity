using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Trait
{
    public CharacterTraits trait { get; }
    public string name { get; }
    public Color colour { get; }
    public string icon { get; }

    public Trait(CharacterTraits trait, string name, Color colour, string icon = "")
    {
        this.trait = trait;
        this.name = name;
        this.colour = colour;
        this.icon = icon;
    }

    public Trait(){}
}

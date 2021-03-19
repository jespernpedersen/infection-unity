using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TraitsList
{
    public List<Trait> list = new List<Trait>();

    public TraitsList()
    {
        list.Add(new Trait(CharacterTraits.AntiSocial, "Anti-Social", Color.red));
        list.Add(new Trait(CharacterTraits.FreeCogher, "Free Cougher", Color.green));
        list.Add(new Trait(CharacterTraits.Germophobic, "Germophobic", Color.red));
        list.Add(new Trait(CharacterTraits.GoodHygiene, "Good Hygene", Color.red));
        list.Add(new Trait(CharacterTraits.Kind, "Kind", Color.red));
        list.Add(new Trait(CharacterTraits.Social, "Social", Color.green));
        list.Add(new Trait(CharacterTraits.Spitter, "Spitter", Color.green));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TraitsList
{
    public List<Trait> list = new List<Trait>();

    public TraitsList()
    {
        list.Add(new Trait(CharacterTraits.AntiSocial, "Anti-Social", new Color(192, 0, 0, 0.3f)));
        list.Add(new Trait(CharacterTraits.FreeCogher, "Free Cougher", new Color(0, 192, 0, 0.3f)));
        list.Add(new Trait(CharacterTraits.Germophobic, "Germophobic", new Color(192, 0, 0, 0.3f)));
        list.Add(new Trait(CharacterTraits.GoodHygiene, "Good Hygene", new Color(192, 0, 0, 0.3f)));
        list.Add(new Trait(CharacterTraits.Kind, "Kind", new Color(192, 0, 0, 0.3f)));
        list.Add(new Trait(CharacterTraits.Social, "Social", new Color(0, 192, 0, 0.3f)));
        list.Add(new Trait(CharacterTraits.Spitter, "Spitter", new Color(0, 192, 0, 0.3f)));
    }
}

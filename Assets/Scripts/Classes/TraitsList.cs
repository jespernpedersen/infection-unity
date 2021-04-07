using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    /// <summary>
    /// Finds a trait by it's trait type
    /// </summary>
    /// <param name="key">The trait</param>
    /// <returns>Trait object, if not found returns an empty trait object</returns>
    public Trait GetTrait(CharacterTraits key)
    {
        foreach (Trait trait in list)
        {
            if (trait.trait == key) return trait;
        }

        return new Trait();
    }
}

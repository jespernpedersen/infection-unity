using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade
{
    public int id { get; }
    public string name { get; }
    public string description { get; }
    public UpgradeType type { get; }
    public float value { get; }
    public Vector2 range { get; }
    public int[] targetMutations { get;  }

    public Upgrade() { }
    public Upgrade(int id, string name, UpgradeType type, string description, float value, int[] targetMutations) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.type = type;
        this.value = value;
        this.targetMutations = targetMutations;
    }

    public Upgrade(int id, string name, string description, UpgradeType type, Vector2 range, int[] targetMutations)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.type = type;
        this.range = range;
        this.targetMutations = targetMutations;
    }

}

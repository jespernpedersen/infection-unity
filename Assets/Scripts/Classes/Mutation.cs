using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mutation
{
    public int id;
    public MutationType type;
    public CharacterStates action;
    public string name;
    public string description;
    public int intensity;
    public float range;
    /// <summary>
    /// Frequency of ocourence of [1, intensity] times in between every [x,y] seconds
    /// </summary>
    public Vector2 frequency;
    /// <summary>
    /// Duration of the hability in seconds
    /// </summary>
    public float duration;
    /// <summary>
    /// The mutations required to be unlocked before this mutation can be achieved
    /// </summary>
    public int[] dependencies;

    public Mutation() {}

    public Mutation(int id, MutationType type, string name, string description, float duration)
    {
        this.id = id;
        this.type = type;
        this.name = name;
        this.description = description;
        this.duration = duration;
    }


    public Mutation(int id, MutationType type, CharacterStates action, string name, string description, int intensity, float range, Vector2 frequency)
    {
        this.id = id;
        this.type = type;
        this.action = action;
        this.name = name;
        this.description = description;
        this.intensity = intensity;
        this.range = range;
        this.frequency = frequency;
    }

    public Mutation(int id, MutationType type, CharacterStates action, string name, string description, int intensity, float range, Vector2 frequency, float duration, int[] dependencies)
    {
        this.id = id;
        this.type = type;
        this.name = name;
        this.description = description;
        this.intensity = intensity;
        this.range = range;
        this.frequency = frequency;
        this.duration = duration;
        this.dependencies = dependencies;
    }

}


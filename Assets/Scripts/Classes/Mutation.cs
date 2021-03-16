using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mutation
{
    public int id;
    public string name;
    public string description;
    public int value;

    public Mutation(int id, string name, string description, int value) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.value = value;
    }
}


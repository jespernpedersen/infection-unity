using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusController : MonoBehaviour
{
    public float points;
    public float growth;
    public float infectCount;

    public List<Mutation> mutations;
    public List<Upgrade> upgrades;

    private void Start()
    {
        // make a surface spreadable virus
        mutations.Add(SceneSingleton.Instance.mutationsList.mutations[1]);
        mutations.Add(SceneSingleton.Instance.mutationsList.mutations[2]);
    }

    public Mutation FindMutation(string name)
    {
        Mutation result = new Mutation();
        foreach(Mutation mutation in mutations)
        {
            if(mutation.name == name)
            {
                result = mutation;
                break;
            }
        }

        return result;
    }

    public Mutation FindMutation(int id)
    {
        Mutation result = new Mutation();
        foreach (Mutation mutation in mutations)
        {
            if (mutation.id == id)
            {
                result = mutation;
                break;
            }
        }

        return result;
    }
}

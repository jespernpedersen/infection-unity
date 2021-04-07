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

    public GameObject airbornePrefab;

    private void Start()
    {
        // make a surface spreadable virus
        mutations.Add(SceneSingleton.Instance.mutationsList.mutations[0]);
        mutations.Add(SceneSingleton.Instance.mutationsList.mutations[1]);
        mutations.Add(SceneSingleton.Instance.mutationsList.mutations[2]);
        mutations.Add(SceneSingleton.Instance.mutationsList.mutations[3]);
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

    /// <summary>
    /// Returns a symptom based on the action it causes in the character
    /// </summary>
    /// <param name="effect">The effect caused in the character</param>
    /// <returns>Mutation object or if not found, returns an empty Mutation</returns>
    public Mutation GetMutationByState(CharacterStates effect)
    {
        Mutation result = new Mutation();
        foreach (Mutation mutation in mutations)
        {
            if (mutation.action == effect)
            {
                result = mutation;
                break;
            }
        }

        return result;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutationsList
{
    public Mutation[] mutations;

    // Start is called before the first frame update
    public MutationsList()
    {
        mutations = new Mutation[] {
                new Mutation(0, MutationType.Ability, "Airborne", "Allows the virus to survive in the air for a time.", 10f),
                new Mutation(1, MutationType.Ability, "Droplet", "Allows the virus to survive in surfaces for a time.", 10f),
                new Mutation(2, MutationType.Symptom, CharacterStates.Cough, "Cough", "Causes victims to cough and contaminate his hands. Free coughers can directly infect other people and contaminate surfaces.", 1, 5f, new Vector2(5f, 15f)),
                new Mutation(3, MutationType.Symptom, CharacterStates.Cough, "Sneeze", "Causes victims to sneeze and contaminate his hands. Creates airborn particles", 1, 3f, new Vector2(5f, 15f)),
                new Mutation(4, MutationType.Ability, "Sensing", "Allows you to trigger an hability", 10f)
    };
       
    }

}

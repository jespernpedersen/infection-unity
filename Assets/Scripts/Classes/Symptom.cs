using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Symptom
{
    /// <summary>
    /// Time in seconds when this symtom will be triggered
    /// </summary>
    public float triggerAt { get; private set; }
    public CharacterStates action { get; private set; }
    public Vector2 frequency { get; private set; }
    public int intensity { get; private set; }
    public delegate IEnumerator TriggerSymptom(int intensity);
    public TriggerSymptom triggerSymptom;

    public Symptom() { }
    public Symptom(Mutation mutation, TriggerSymptom symptom) {
        action = mutation.action;
        frequency = mutation.frequency;
        intensity = mutation.intensity;

        this.triggerSymptom = symptom;

        Refresh();
    }

    public void Refresh()
    {
        triggerAt = Time.time + Random.Range(frequency.x, frequency.y);
    }

    public IEnumerator Wait(MonoBehaviour instance)
    {
        yield return new WaitForSeconds(triggerAt);
        Refresh();
        instance.StartCoroutine(triggerSymptom(intensity));

        instance.StartCoroutine(Wait(instance));
    }
}

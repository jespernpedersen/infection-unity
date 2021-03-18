using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Objective
{
    private ObjectiveType type;
    public string description { get; private set; }
    public int counter { get; private set; }
    public int goal { get; private set; }
    public bool isComplete { get; private set; }
    public GameObject target { get; private set; }
    public Text textUi;

    public delegate void CompleteCallback();
    public delegate void UpdateCallback();

    private List<CompleteCallback> objCompleteCallback = new List<CompleteCallback>();
    private List<UpdateCallback> objUpdateCallback = new List<UpdateCallback>();

    public Objective()
    {
        isComplete = false;
        counter = 0;
        goal = 1;
    }

    public void Complete(GameObject target = null)
    {
        switch (type)
        {
            case ObjectiveType.InfectTarget:
                
                if (target == null) return;

                checkTarget(target);
                break;
            case ObjectiveType.InfectAmount:
                Increase(1);
                break;
        }
    }

    private void Increase(int amount)
    {
        counter += amount;

        foreach (UpdateCallback update in objUpdateCallback)
        {
            update();
        }

        if (counter >= goal)
        {
            isComplete = true;

            foreach (CompleteCallback complete in objCompleteCallback)
            {
                complete();

            }
        }

        UpdateUI();

    }

    private void checkTarget(GameObject target)
    {

        if (target == this.target)
        {
            Increase(1);
            return;
        }

    }

    /// <summary>
    /// Subscribe to objective update event
    /// </summary>
    /// <param name="callback">Callback function (must be public)</param>
    public void onUpdate(UpdateCallback callback)
    {
        objUpdateCallback.Add(callback);
    }

    /// <summary>
    /// Subscribe to objective complete event
    /// </summary>
    /// <param name="callback">Callback function (must be public)</param>
    public void onComplete(CompleteCallback callback)
    {
        objCompleteCallback.Add(callback);
    }

    private void UpdateUI()
    {
        string text = description;
      
        if( type == ObjectiveType.InfectAmount) { 
                text += "<color=gray>" + counter.ToString() + "<color>/" + goal;
        }

        if (isComplete)
        {
            text = "<olor=green>" + text + "</color>";
        }

        textUi.text = text;
    }

}

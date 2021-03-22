using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Objective
{
    [SerializeField]
    private ObjectiveType type;
    [SerializeField]
    private string description;
    private int counter;
    [SerializeField]
    private int goal;
    private bool isComplete;
    [SerializeField]
    private GameObject target;
    [HideInInspector]
    public Text textUi;

    public bool IsComplete { get
        {
            return isComplete;
        } }

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

        if (this.target == null || target == this.target)
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

    public void UpdateUI()
    {
        string text = "<color=grey>" + counter.ToString() + "</color>/" + goal;


        if (isComplete)
        {
            text = "<color=green>" + counter.ToString() + "/" + goal+ "</color>";
        }

        textUi.text = text;
    }

}

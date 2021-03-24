using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DoorController : MonoBehaviour, iInteractable, iInfectable
{
    private Animator animator;
    private SpriteRenderer sprite;

    private bool isOpen = false;
    [SerializeField]
    private bool isInfected = false;
    public bool IsInfected
    {
        get
        {
            return isInfected;
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {

        if (isInfected)
        {
            isInfected = false;// to force the infect method to run
            Infect();
        }
    }

    public void ChangeColour(float timeSpeed)
    {
        if (timeSpeed == 0)
        {
            sprite.color = Color.green;
            return;
        }

        sprite.color = Color.white;
    }

    public void Interact(iInfectable human = null)
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);

        //the door is infected and the thing that interacted with the door is infectable
        if (isInfected && human != null)
        {
            human.Infect();
        }
      
    }

    public void Infect(float duration = -1)
    {
        if (isInfected) return;

        isInfected = true;
        SceneSingleton.Instance.level.onTimeChange(ChangeColour);

        if (duration != -1)
        {
            StartCoroutine(Desinfect(duration));
        }
    }

    public IEnumerator Desinfect(float infectionDuration)
    {
        yield return new WaitForSeconds(infectionDuration);
        isInfected = false;
    }
}

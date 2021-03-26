using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class DoorController : MonoBehaviour, iInteractable, iInfectable
{
    private Animator animator;
    private SpriteRenderer sprite;
    private BoxCollider2D boxCollider;
    public Interactable objectType { get; }

    private bool isOpen = false;
    [SerializeField]
    private bool isLocked = false;
    public bool IsOpen
    {
        get
        {
            return isOpen;
        }
    }
    public bool IsLocked
    {
        get
        {
            return isLocked;
        }
    }
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
        boxCollider = GetComponents<BoxCollider2D>()[1];
    }

    private void Start()
    {

        if (isInfected)
        {
            isInfected = false;// to force the infect method to run
            Infect();
        }
    }

    /// <summary>
    /// Allows the player to interact with the object on mouse click
    /// </summary>
    private void OnMouseDown()
    {
        Interact();
    }

    private void OnMouseOver()
    {
        Debug.Log("Mouse down detected");
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
    }
    private void OnMouseExit()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, 1);
    }

    public void ChangeColour(float timeSpeed)
    {
        if (timeSpeed == 0 && isInfected)
        {
            sprite.color = new Color(0, 255, 0, sprite.color.a);
            return;
        }

        sprite.color = new Color(255, 255, 255, sprite.color.a);
    }

    public bool Interact(iInfectable human = null)
    {
        //the door is infected and the thing that interacted with the door is infectable
        if (isInfected && human != null)
        {
            human.Infect();
        }

        if (isLocked) return false; 

        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
        boxCollider.enabled = !isOpen;

        return true;
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

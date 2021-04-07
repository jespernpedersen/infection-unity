using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AudioSource))]
public class DoorController : MonoBehaviour, iInteractable, iInfectable
{
    private Animator animator;
    private SpriteRenderer sprite;
    private BoxCollider2D interactableArea;
    private BoxCollider2D boxCollider;
    private AudioSource source;

    public Interactable objectType { get; }
    [SerializeField]
    private bool isOpen = false;
    [SerializeField]
    private bool isLocked = false;
    [SerializeField]
    private AudioClip doorSound;
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
        animator = transform.GetChild(0).GetComponent<Animator>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        interactableArea = colliders[0];
        boxCollider = colliders[1];
        source = GetComponent<AudioSource>();
        source.clip = doorSound;
        source.time = 0.9f;
    }

    private void Start()
    {

        if (isInfected)
        {
            isInfected = false;// to force the infect method to run
            Infect();
        }

        if (isOpen) Interact();
    }

    public bool Interact(CharacterController human = null)
    {
        //the door is infected and the thing that interacted with the door is infectable
        if (isInfected && human != null)
        {
            // germophobes can't be infected by touching infected surfaces
            if (!human.Traits.Contains(CharacterTraits.Germophobic)){
                human.Infect();
            }
        }

        if (isLocked) return false;

        isOpen = !isOpen;
        if (isOpen)
        {
            interactableArea.size = new Vector2(1.7f, interactableArea.size.y);
            interactableArea.offset = new Vector2(-0.71f, 0);
        } else
        {
            interactableArea.size = new Vector2(0.43f, interactableArea.size.y);
            interactableArea.offset = Vector2.zero;
        }

        animator.SetBool("isOpen", isOpen);
        boxCollider.enabled = !isOpen;
        source.Play();

        return true;
    }

    private void OnMouseDown()
    {
        Interact();
    }

    private void OnMouseOver()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
    }
    private void OnMouseExit()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, 1);
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

    public void ChangeColour(float timeSpeed)
    {
        if (timeSpeed == 0 && isInfected)
        {
            sprite.color = new Color(0, 255, 0, sprite.color.a);
            return;
        }

        sprite.color = new Color(255, 255, 255, sprite.color.a);
    }
}

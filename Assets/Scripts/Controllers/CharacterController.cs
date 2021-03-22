using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class CharacterController : MonoBehaviour, iInfectable
{

    private SpriteRenderer sprite;
    private Animator animator;

    public float walkingSpeed = 3;
    private bool isInfected = false;

    private bool inAction = false;
    private CharacterStates state;

    [SerializeField]
    private GameObject traitPrefab;

    [SerializeField]
    private List<Action> routine = new List<Action>();
    private int curAction = -1;

    [SerializeField]
    private string HumanName = "";
    [SerializeField]
    private List<CharacterTraits> traits = new List<CharacterTraits>();

    private GameObject canvas;

    private bool isInfect = false;
    public bool IsInfected { 
        get{
            return isInfected;
        } 
    }

    public float infectionDuration { get; set; }

    private void Awake()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
        canvas = transform.GetChild(0).gameObject;

    }

    // Start is called before the first frame update
    void Start()
    {
        //subscribe to onTimeChange
        SceneSingleton.Instance.level.onTimeChange(ShowInterface);

        Transform traitsGrid = canvas.transform.GetChild(1);
        foreach (CharacterTraits trait in traits)
        {
            GameObject newTrait = Instantiate(traitPrefab, traitsGrid);
            Trait traitReference = new Trait();
            foreach (Trait traitModel in SceneSingleton.Instance.traitsList.list)
            {
                if (traitModel.trait == trait)
                {
                    traitReference = traitModel;
                    break;
                }
            }

            newTrait.GetComponent<Image>().color = traitReference.colour;
            newTrait.transform.GetChild(1).GetComponent<Text>().text = traitReference.name;
           
        }

        ShowInterface(SceneSingleton.Instance.level.TimeSpeed);
        MakeDecision();
    } 

    private void MakeDecision()
    {
        if (inAction || routine.Count == 0) return;

        ++curAction;
        if (curAction >= routine.Count) curAction = 0;

        switch (routine[curAction].action)
        {
            case CharacterStates.Idle:
                StartCoroutine(Idle(routine[curAction].duration));
                break;
            case CharacterStates.Move:

                Vector2 pos = routine[curAction].coordenate != null ?
                    routine[curAction].coordenate :
                    new Vector2(transform.position.x, transform.position.y);

                StartCoroutine(MoveTo(pos));
                break;
            case CharacterStates.Interact:
            
                iInteractable obj = routine[curAction].interactWith != null ? 
                                        routine[curAction].interactWith.GetComponent(typeof(iInteractable)) as iInteractable
                                        : null;

                StartCoroutine(Interact(obj));

                break;
            default:
                break;
        }
         
    }

    /// <summary>
    /// Moves the character in one direction
    /// </summary>
    /// <param name="direction">1 = left; -1 = right</param>
    private IEnumerator MoveTo(Vector2 point) 
    {
        if (inAction) yield return null;
        
        inAction = true;
        state = CharacterStates.Move;

        int direction = transform.position.x < point.x ? 1 : -1;

        sprite.flipX = direction > 0 ? false : true;
        point.y = transform.position.y;

        animator.SetBool("doWalk", true);

        while(Vector2.Distance(transform.position, point) > 0.5f)
        {
            transform.Translate((transform.right * ( walkingSpeed * direction )) * Time.deltaTime);
            yield return new WaitForEndOfFrame();

        }
        animator.SetBool("doWalk", false);

        state = CharacterStates.Idle;
        inAction = false;
        MakeDecision();
    }

    /// <summary>
    /// Sets the character into an idle state
    /// </summary>
    /// <param name="duration">duration of idle</param>
    /// <returns></returns>
    private IEnumerator Idle(float duration = 0)
    {
        if (inAction) yield return null;

        state = CharacterStates.Idle;

        inAction = true;
        yield return new WaitForSeconds(duration);
        inAction = false;

        MakeDecision();
    }

    private IEnumerator Interact(iInteractable objToInteract = null)
    {
        if (inAction) yield return null;

        inAction = true;
        animator.SetBool("doInteract", true);

        state = CharacterStates.Interact;

        // code to interact with objects

        yield return new WaitForSeconds(1f);


        if (objToInteract != null)
        {
            objToInteract.Interact(this);
        }

        inAction = false;
        animator.SetBool("doInteract", false);

        MakeDecision();

    }

    public void Infect(float duration = -1)
    {
        isInfected = true;
        SceneSingleton.Instance.level.onHumanInfected(gameObject);
    }

    public IEnumerator Desinfect(float waitBeforeDesinfect = -1)
    {
        isInfected = false;
        yield return null;
    }

    /// <summary>
    /// Shows/hides the character traits and name according to game+s timeflow
    /// </summary>
    /// <param name="timeSpeed">0 = paused => show; 1 = resumed => hide</param>
    public void ShowInterface(float timeSpeed)
    {
        bool showCanvas = timeSpeed == 1 ? false : true;
        canvas.SetActive(showCanvas);
    }
}

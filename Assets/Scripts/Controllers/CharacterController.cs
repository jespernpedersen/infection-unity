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

    private bool inAction = false;//prevents actions from playing while this plays
    private CharacterStates state;
    private Coroutine curAction;

    [SerializeField]
    private GameObject traitUi;
    [SerializeField]
    private GameObject particles;
    [SerializeField]
    private ParticleSystem coughParticles;
    [SerializeField]
    private ParticleSystem sneezeParticles;

    [SerializeField]
    private List<Action> routine = new List<Action>();
    private int curActionIndex = -1;

    [SerializeField]
    private string HumanName = "";
    [SerializeField]
    private List<CharacterTraits> traits = new List<CharacterTraits>();

    [SerializeField]
    private List<Symptom> symptoms = new List<Symptom>();


    private GameObject canvas;

    private bool isInfected = false;
    public bool IsInfected
    {
        get
        {
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
            GameObject newTrait = Instantiate(traitUi, traitsGrid);
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

    /// <summary>
    /// Decides what action to play next.
    /// </summary>
    /// <param name="increment">Will play the next action in routine if true</param>
    private void MakeDecision(bool increment = true)
    {
        if (inAction || routine.Count == 0) return;

        if (increment)
        {
            ++curActionIndex;
        }

        if (curActionIndex >= routine.Count) curActionIndex = 0;

        switch (routine[curActionIndex].action)
        {
            case CharacterStates.Idle:
                curAction = StartCoroutine(Idle(routine[curActionIndex].duration));
                break;
            case CharacterStates.Move:

                Vector2 pos = routine[curActionIndex].coordenate != null ?
                    routine[curActionIndex].coordenate :
                    new Vector2(transform.position.x, transform.position.y);

                curAction = StartCoroutine(MoveTo(pos));
                break;
            case CharacterStates.Interact:

                iInteractable obj = routine[curActionIndex].interactWith != null ?
                                        routine[curActionIndex].interactWith.GetComponent(typeof(iInteractable)) as iInteractable
                                        : null;

                curAction = StartCoroutine(Interact(obj));

                break;
            case CharacterStates.Sit:
                curAction = StartCoroutine(Sit());
                break;
            case CharacterStates.Sneeze:
                curAction = StartCoroutine(Sneeze());
                break;
            case CharacterStates.Cough:
                curAction = StartCoroutine(Cough());
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// Moves the character in one direction
    /// </summary>
    /// <param name="direction">1 = right; -1 = left</param>
    private IEnumerator MoveTo(Vector2 point)
    {
        if (inAction) yield return null;

        inAction = true;
        state = CharacterStates.Move;

        int direction = transform.position.x < point.x ? 1 : -1;
        
        particles.transform.eulerAngles = new Vector3(0, (-90 * direction) + 90, particles.transform.eulerAngles.z); 

        sprite.flipX = direction > 0 ? false : true;
        point.y = transform.position.y;

        animator.SetBool("doWalk", true);

        while (Vector2.Distance(transform.position, point) > 0.5f)
        {
            transform.Translate((transform.right * (walkingSpeed * direction)) * Time.deltaTime);
            yield return new WaitForEndOfFrame();

        }
        animator.SetBool("doWalk", false);

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
        Debug.Log(duration + " == " + (Time.time + duration));
        inAction = true;
        yield return new WaitForSeconds(duration);
        inAction = false;
        Debug.Log(Time.time);
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

    private IEnumerator Sit()
    {
        if (inAction) yield return null;

        Vector2 startingCharacterPos = transform.position;
        inAction = true;
        animator.SetBool("doSit", true);
        transform.position = routine[curActionIndex].coordenate;

        yield return new WaitForSeconds(routine[curActionIndex].duration);

        transform.position = startingCharacterPos;
        animator.SetBool("doSit", false);
        inAction = false;

        MakeDecision();

    }

    /// <summary>
    /// Causes a single sneeze. This halts current animations, that are resumed after the sneeze
    /// </summary>
    /// <returns></returns>
    public IEnumerator Sneeze(int intensity = 1)
    {
        CharacterStates previousAction = CharacterStates.None;
        if (inAction)
        {
            previousAction = routine[curActionIndex].action;
            StopCoroutine(curAction);
        }

        inAction = true;

        yield return new WaitForSeconds(0.3f);
        animator.SetBool("doSneeze", true);
        yield return new WaitForSeconds(0.1f);

        animator.SetBool("doSneeze", false);
        yield return new WaitForSeconds(0.5f);

        inAction = false;

        MakeDecision(previousAction == CharacterStates.None);//replay the interrupted action

    }

    public IEnumerator Cough(int intensity = 1)
    {
        CharacterStates previousAction = CharacterStates.None;
        if (inAction)
        {
            StopCoroutine(curAction);
            previousAction = routine[curActionIndex].action;
        }
        else inAction = true;

        intensity = Random.Range(1, intensity);

        for (int i = 0; i < intensity; i++)
        {
            animator.SetBool("doSneeze", true);
            coughParticles.Play();
            yield return new WaitForSeconds(0.2f);
            animator.SetBool("doSneeze", false);
            yield return new WaitForSeconds(0.2f);
        }

        inAction = false;

        MakeDecision(previousAction == CharacterStates.None);//replay the interrupted action
    }

    /// <summary>
    /// Infects the character and adds the symptoms
    /// </summary>
    /// <param name="duration"></param>
    public void Infect(float duration = -1)
    {
        if (isInfected) return;

        isInfected = true;
        SceneSingleton.Instance.level.onHumanInfected(gameObject);

        foreach (Mutation mutation in SceneSingleton.Instance.virus.mutations)
        {
            if (mutation.type == MutationType.Symptom)
            {
                switch (mutation.action)
                {
                    case CharacterStates.Cough:
                        symptoms.Add(new Symptom(mutation, Cough));
                        break;
                    case CharacterStates.Sneeze:
                        symptoms.Add(new Symptom(mutation, Sneeze));
                        break;
                    default: break;
                }

                StartCoroutine(symptoms[symptoms.Count - 1].Wait(this));
            }
        }
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

        if (isInfected && timeSpeed == 0)
        {
            sprite.color = Color.green;
        } else
        {
            sprite.color = Color.white;
        }
    }

}

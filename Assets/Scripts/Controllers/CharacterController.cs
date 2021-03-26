using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class CharacterController : MonoBehaviour, iInfectable
{

    private SpriteRenderer sprite;
    private Animator animator;
    private Rigidbody2D rg;

    public float walkingSpeed = 3;
    private Vector3 moveToTarget;

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
    private GameObject airbornCloud;

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
        rg = gameObject.GetComponent<Rigidbody2D>();
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

    private void FixedUpdate()
    {
        if(state == CharacterStates.Move)
        {

            moveToTarget.y = transform.position.y;
            if (Vector2.Distance(transform.position, moveToTarget) > 0.5f)
            {
                int direction = transform.position.x < moveToTarget.x ? 1 : -1;
                rg.MovePosition(transform.position + (transform.right * (walkingSpeed * direction) * Time.deltaTime));
                return;
            }

            moveToTarget = Vector3.zero;
            animator.SetBool("doWalk", false);

            state = CharacterStates.Idle;

            inAction = false;
            MakeDecision();
        }
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

                MoveTo(pos);
                break;
            case CharacterStates.Interact:
                curAction = StartCoroutine(Interact(routine[curActionIndex].interactWith.transform));
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
    private void MoveTo(Vector2 point)
    {
        if (inAction) return;

        inAction = true;
        state = CharacterStates.Move;
        moveToTarget = point;

        int direction = transform.position.x < point.x ? 1 : -1;

        particles.transform.eulerAngles = new Vector3(0, (-90 * direction) + 90, particles.transform.eulerAngles.z);

        sprite.flipX = direction > 0 ? false : true;
        point.y = transform.position.y;

        animator.SetBool("doWalk", true);

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="objToInteract">Object to interact with</param>
    /// <param name="executeNextBehaviour">true= play next routine action; false = repeat current action</param>
    /// <returns></returns>
    private IEnumerator Interact(Transform objToInteract = null, bool executeNextBehaviour = false)
    {
        if (inAction) yield return null;

        inAction = true;
        animator.SetBool("doInteract", true);

        state = CharacterStates.Interact;

        // code to interact with objects

        yield return new WaitForSeconds(1f);


        if (objToInteract != null)
        {
            iInteractable interactable = objToInteract.GetComponent(typeof(iInteractable)) as iInteractable;
            interactable.Interact(this);

            Mutation mutation = SceneSingleton.Instance.virus.FindMutation(1);

            // if the virus spreads by touch
            if (isInfected && mutation.id != -1)
            {
                iInfectable infectable = objToInteract.GetComponent(typeof(iInfectable)) as iInfectable;
                infectable.Infect(mutation.duration);
            }
        }

        inAction = false;
        animator.SetBool("doInteract", false);

        MakeDecision(executeNextBehaviour);

    }

    private IEnumerator Sit()
    {
        if (inAction) yield return null;

        routine[curActionIndex].coordenate.y = transform.position.y;
        if (Vector2.Distance(transform.position, routine[curActionIndex].coordenate) < 0.5f)
        {
            state = CharacterStates.Sit;

            Vector2 startingCharacterPos = transform.position;
            inAction = true;
            animator.SetBool("doSit", true);
            transform.position = routine[curActionIndex].coordenate;

            yield return new WaitForSeconds(routine[curActionIndex].duration);

            transform.position = startingCharacterPos;
            animator.SetBool("doSit", false);
            inAction = false;
        }

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

        state = CharacterStates.Sneeze;

        intensity = Random.Range(1, intensity);

        float duration = SceneSingleton.Instance.virus.FindMutation(0).duration;//airborne transmition mutation
        float range = SceneSingleton.Instance.virus.GetMutationByState(CharacterStates.Sneeze).range;// snee symtom mutation

        for (int i = 0; i < intensity; i++)
        {
            yield return new WaitForSeconds(0.3f);
            animator.SetBool("doSneeze", true);
            yield return new WaitForSeconds(0.1f);
            sneezeParticles.Play();

            int direction = sprite.flipX ? -1 : 1;

            animator.SetBool("doSneeze", false);
            yield return new WaitForSeconds(0.5f);
            GameObject cloud = Instantiate(SceneSingleton.Instance.virus.airbornePrefab , new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z), Quaternion.identity);
            cloud.GetComponent<Rigidbody2D>().AddForce(Vector2.right * direction * 10);// move the cloud forward, slowly
        }

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

        state = CharacterStates.Cough;

        intensity = Random.Range(1, intensity);

        float duration = SceneSingleton.Instance.virus.FindMutation(1).duration;//dropled based transmition mutation
        float range = SceneSingleton.Instance.virus.FindMutation(2).range;// cough symtom mutation

        for (int i = 0; i < intensity; i++)
        {
            animator.SetBool("doSneeze", true);
            coughParticles.Play();
            yield return new WaitForSeconds(0.2f);

            if (traits.Contains(CharacterTraits.FreeCogher))
            {
                Collider2D[] allOverlappingColliders = Physics2D.OverlapCircleAll(transform.position, range / 2);

                foreach (Collider2D col in allOverlappingColliders)
                {
                    iInfectable infetable = col.transform.GetComponent(typeof(iInfectable)) as iInfectable;

                    if (infetable == null) continue;

                    //if the character is facing right and the object is in front of it
                    //or if the character is facing left and the object is in front of it
                    if (sprite.flipX == false && col.transform.position.x > transform.position.x ||
                        sprite.flipX == true && col.transform.position.x < transform.position.x)
                    {
                        infetable.Infect(duration);
                    }

                }
            }

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
        }
        else
        {
            sprite.color = Color.white;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state != CharacterStates.Move) return;

        iInteractable interactable = collision.transform.GetComponent(typeof(iInteractable)) as iInteractable;

        StopAllCoroutines();
        inAction = false;

        if (interactable.objectType == Interactable.Door && !collision.transform.GetComponent<DoorController>().IsOpen)
        {
            // stop all animations
            foreach(AnimatorControllerParameter parameter in animator.parameters)
            {
                animator.SetBool(parameter.name, false);
            }

            StartCoroutine(Interact(collision.transform, collision.transform.GetComponent<DoorController>().IsLocked));
        }
    }

}

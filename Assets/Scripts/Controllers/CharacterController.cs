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
    private AudioSource audioSource;

    public float walkingSpeed = 3;
    public float interactionDistance = 1.4f;
    private float spreadDistanceTalk = 2f;
    private Vector3 moveToTarget;

    [SerializeField]
    private AudioClip talking;
    [SerializeField]
    private AudioClip coughingSound;
    private float audioVolume;

    private bool isTalking = false;
    private bool inAction = false;//prevents actions from playing while this plays
    private CharacterStates state;
    private Coroutine curAction;

    [SerializeField]
    private Text nameUi;
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
    private GameObject speechBallon;

    [SerializeField]
    private List<Action> routine = new List<Action>();
    private int curActionIndex = -1;

    [SerializeField]
    private string HumanName = "";
    [SerializeField]
    private List<CharacterTraits> traits = new List<CharacterTraits>();
    [SerializeField]
    private List<Symptom> symptoms = new List<Symptom>();

    private bool makeDecisionAfterMoving = true;

    public List<CharacterTraits> Traits
    {
        get
        {
            return traits;
        }
    }

    private GameObject canvas;
    [SerializeField]
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
        nameUi.text = HumanName;
        audioSource = GetComponent<AudioSource>();
        audioVolume = audioSource.volume;

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
        
        if(isInfected) Infect();

        ShowInterface(SceneSingleton.Instance.level.TimeSpeed);
        MakeDecision();
    }

    private void FixedUpdate()
    {
        if(state == CharacterStates.Move)
        {

            moveToTarget.y = transform.position.y;
            if (Vector2.Distance(transform.position, moveToTarget) >= interactionDistance)
            {
                int direction = transform.position.x < moveToTarget.x ? 1 : -1;
                rg.MovePosition(transform.position + (transform.right * (walkingSpeed * direction) * Time.deltaTime));
                return;
            }

            moveToTarget = Vector3.zero;
            animator.SetBool("doWalk", false);

            state = CharacterStates.Idle;

            inAction = false;
            MakeDecision(makeDecisionAfterMoving);
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
                curAction = StartCoroutine(Idle(routine[curActionIndex].coordenate));
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
            case CharacterStates.Talk:
                state = CharacterStates.Talk;//this is set here because it should only be considered a state when is part of the routine. Spontanious and simultanious actions won't count as current state/activity
                curAction = StartCoroutine(Talk(routine[curActionIndex].duration, routine[curActionIndex].interactWith));
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
    /// Moves the character to a position and goes into an idle state
    /// </summary>
    /// <param name="pos">Position coordinates to idle</param>
    /// <param name="duration">duration of idle</param>
    /// <returns></returns>
    private IEnumerator Idle(Vector2 pos, float duration = 0)
    {
        if (inAction) yield break;


        makeDecisionAfterMoving = false;
        pos = new Vector2(pos.x, transform.position.y);
        inAction = false;
        MoveTo(pos);
        while (Vector2.Distance(pos, transform.position) > interactionDistance)
        {
            yield return new WaitForEndOfFrame();
        }

        stopAllAnimations();
        state = CharacterStates.Idle;

        inAction = true;
        if (duration == 0)
        {
            duration = routine[curActionIndex].duration;
        }
        yield return new WaitForSeconds(duration);
        inAction = false;

        MakeDecision();
    }

    /// <summary>
    /// Makes a humen interact with an iInteractable object. If it is too far, the human will move closer
    /// </summary>
    /// <param name="objToInteract">Object to interact with</param>
    /// <param name="executeNextBehaviour">true= play next routine action; false = repeat current action</param>
    /// <returns>WaitForSeconds</returns>
    public IEnumerator Interact(Transform objToInteract = null, bool executeNextBehaviour = true)
    {
        CharacterStates previousAction = CharacterStates.None;
        if (inAction)
        {
            previousAction = routine[curActionIndex].action;
            StopCoroutine(curAction);
            stopAllAnimations();
        }

        makeDecisionAfterMoving = false;
        Vector2 pos = transform.position;
        pos.x = objToInteract.position.x;
        inAction = false;
        MoveTo(pos);
        while (Vector2.Distance(pos, transform.position) > interactionDistance)
        {
            yield return new WaitForEndOfFrame();
        }
        
        inAction = true;
        stopAllAnimations();
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
            if (isInfected && mutation.id != -1 && !traits.Contains(CharacterTraits.Germophobic))
            {
                iInfectable infectable = objToInteract.GetComponent(typeof(iInfectable)) as iInfectable;
                infectable.Infect(mutation.duration);
            }
        }

        inAction = false;
        animator.SetBool("doInteract", false);

        if(previousAction != CharacterStates.None)
        {
            executeNextBehaviour = false;
        }

        makeDecisionAfterMoving = true;
        MakeDecision(executeNextBehaviour);

    }

    private IEnumerator Sit()
    {
        if (inAction) yield break;

        makeDecisionAfterMoving = false;

        routine[curActionIndex].coordenate.y = transform.position.y;

        MoveTo(routine[curActionIndex].coordenate);
        while (Vector2.Distance(routine[curActionIndex].coordenate, transform.position) > interactionDistance)
        {
            yield return new WaitForEndOfFrame();
        }

        stopAllAnimations();

        if (Vector2.Distance(transform.position, routine[curActionIndex].coordenate) <= interactionDistance)
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
            audioSource.Stop();
            audioSource.clip = coughingSound;
            audioSource.Play();
            coughParticles.Play();
            yield return new WaitForSeconds(coughingSound.length);


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
    /// Makes a character talk with other character. This action can happen simultaniously with actions other than cough or sneeze
    /// </summary>
    /// <param name="duration">Duration of conversation</param>
    /// <returns></returns>
    public IEnumerator Talk(float duration, GameObject target = null)
    {

        // if the character is anti-social, abort action
        if(traits.Contains(CharacterTraits.AntiSocial))
        {
            if (state == CharacterStates.Talk) MakeDecision();

            yield break;
        }

        if (inAction && (state == CharacterStates.Cough || state == CharacterStates.Sneeze))
        {
            yield break;
        }

        isTalking = true;
        speechBallon.SetActive(true);
        audioSource.Stop();
        audioSource.clip = talking;
        audioSource.time = Random.Range(0.1f, talking.length);
        audioSource.Play();
        yield return new WaitForSeconds(duration);
        audioSource.Stop();
        speechBallon.SetActive(false);
        isTalking = false;

        if (isInfected)
        {
            float range = (traits.Contains(CharacterTraits.Spitter)) ? spreadDistanceTalk * 2 : spreadDistanceTalk;
            Collider2D[] allOverlappingColliders = Physics2D.OverlapCircleAll(transform.position, range);

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

        if (target != null)
            target.SendMessage("AnswerConversation");

        if (state == CharacterStates.Talk) MakeDecision();

    }

    public void AnswerConversation()
    {
        StartCoroutine(Talk(1f));
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

        if (timeSpeed == 0)
        {
            audioSource.volume = 0;

            if (isInfected)
            {
                sprite.color = Color.green;
            }

        } else
        {
            audioSource.volume = audioVolume;
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
            stopAllAnimations();

            StartCoroutine(Interact(collision.transform, collision.transform.GetComponent<DoorController>().IsLocked));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterController person = collision.transform.GetComponent<CharacterController>();

        if (person == null || !traits.Contains(CharacterTraits.Social)) return;

        // if the person is in front of this human, speak to it
        if(!sprite.flipX && person.transform.position.x > transform.position.x ||
            sprite.flipX && person.transform.position.x < transform.position.x)
        {
            StartCoroutine(Talk(2f, person.gameObject));
        }

    }

    private void stopAllAnimations()
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            animator.SetBool(parameter.name, false);
        }
    }

}

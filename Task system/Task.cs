using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Task : MonoBehaviour
{

    [TextArea(5, 5)] public string TaskDescription;


    public bool InitiateOnAwake = false;
   [Tooltip("Initiate event will be called, but conditions will initiate later.")] 
    public float StartCompletionDetectingDelay = 1f;

    [SerializeReference]
    private List<TaskCondition> conditions = new();
    [Header("Initiation")]
    public UnityEvent OnInitiated;

    [Tooltip("Rigidbodies get overridden at initiation, and reverted to normal on completion.")]
    public RigidbodyValueOverrider[] rigidbodyValueOverrides;    

    [Header("Completion")]
    public UnityEvent OnCompleted;

    public enum ConditionType
    {
        AND,
        OR,
        OneHot
    }
    public ConditionType conditionType = ConditionType.AND;

    private delegate void InitiateUpdate();
    private event InitiateUpdate initiateUpdate;

    private delegate void InitiateFixedUpdate();
    private event InitiateFixedUpdate initiateFixedUpdate;

    private bool isStarted = false;

    public virtual void Awake()
    {
        foreach (var rbOverride in rigidbodyValueOverrides)
        {
            if (rbOverride.rb == null) continue;
            if (rbOverride.IsLocalConstraint) 
                initiateUpdate += rbOverride.LocalSpaceUpdater;
            else
            {
                OnInitiated.AddListener(delegate { rbOverride.SetOverrideConstraints(); });
                OnCompleted.AddListener(delegate { rbOverride.RevertOverrideConstraints(); });
            }
        }

        if (InitiateOnAwake) Initiate();
        else { enabled = false; }
    }

    private void Start()
    {
        isStarted = true;
    }

    public void Initiate()
    {
        StartCoroutine(InitiateCorou());
    }

    public IEnumerator InitiateCorou()
    {
        enabled = true;
        yield return new WaitUntil(() => isStarted);
        yield return null;

        OnInitiated?.Invoke();

        yield return new WaitForSeconds(StartCompletionDetectingDelay);

        initiateUpdate += UpdateConditions;
    }

    void Update()
    {
        initiateUpdate?.Invoke();
    }

    void FixedUpdate()
    {
        initiateFixedUpdate?.Invoke();
    }

    private void UpdateConditions()
    {
        bool result = false;
        switch (conditionType)
        {
            case ConditionType.AND:
                if (conditions.TrueForAll(cond => cond.IsMet()))
                    result = true;
                break;

            case ConditionType.OR:
                if (conditions.Any(cond => cond.IsMet()))
                    result = true;
                break;

            case ConditionType.OneHot:
                if (conditions.Count(cond => cond.IsMet()) == 1)
                    result = true;
                break;
        }

        if (result)
        {
            Debug.Log("Task completed!");
            OnCompleted?.Invoke();

            enabled = false; // disable checking
        }
    }
}
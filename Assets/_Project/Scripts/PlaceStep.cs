using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Put this on a socket. It refuses every object until it is this step's turn (select filter),
/// and reports the step as done when something is placed (select listener).
/// Fields: the socket on the same object, the Trainer's ScenarioController, and the index of the step this socket completes.
/// </summary>
public class PlaceStep : MonoBehaviour, IXRSelectFilter
{
    [SerializeField] XRSocketInteractor socket;
    [SerializeField] ScenarioController scenario;
    [SerializeField] int stepIndex;
    [Tooltip("If off, the socket accepts objects at any time and only reports when it is this step's turn")]
    [SerializeField] bool gateByStep = true;

    public bool canProcess => isActiveAndEnabled && gateByStep;

    void Reset() => socket = GetComponent<XRSocketInteractor>();

    void OnEnable()
    {
        socket.selectFilters.Add(this);
        socket.selectEntered.AddListener(OnPlaced);
        if (scenario) scenario.OnStepChanged += OnStep;
    }

    void OnDisable()
    {
        socket.selectFilters.Remove(this);
        socket.selectEntered.RemoveListener(OnPlaced);
        if (scenario) scenario.OnStepChanged -= OnStep;
    }

    // An ungated socket may already hold its object when its step comes up; count it then.
    void OnStep(ScenarioStep step, int index)
    {
        if (index == stepIndex && socket.hasSelection)
            scenario.Complete(stepIndex, socket.firstInteractableSelected.transform.name);
    }

    // The filter: the socket only catches objects while this step is active.
    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        => scenario.CurrentStep == stepIndex;

    void OnPlaced(SelectEnterEventArgs args)
        => scenario.Complete(stepIndex, args.interactableObject.transform.name);
}

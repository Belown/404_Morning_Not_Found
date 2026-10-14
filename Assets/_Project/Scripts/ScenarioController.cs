using System;
using UnityEngine;

/// <summary>
/// Walks the user through a list of ScenarioStep assets. Step components (PlaceStep, PressStep) call Complete().
/// Presenters (ShowPanel, Highlight, StepLogger) listen to OnStepChanged and never know who fired it.
/// Exactly one per scene, on an object called Trainer.
/// </summary>
public class ScenarioController : MonoBehaviour
{
    [SerializeField] ScenarioStep[] steps;
    [SerializeField] StepLogger logger;

    /// <summary>Index of the active step. Equals steps.Length when the scenario is finished.</summary>
    public int CurrentStep { get; private set; }

    /// <summary>The active step asset, or null when finished.</summary>
    public ScenarioStep Current => CurrentStep < steps.Length ? steps[CurrentStep] : null;

    public bool IsFinished => CurrentStep >= steps.Length;

    /// <summary>Fired on start and after every completed step. Arguments: the new step (null when finished) and its index.</summary>
    public event Action<ScenarioStep, int> OnStepChanged;

    void Start() => Announce();

    /// <summary>Called by a step component when its step was done. Out of order calls are ignored and logged.</summary>
    public void Complete(int stepIndex, string objectName)
    {
        if (IsFinished) return;
        if (stepIndex != CurrentStep)
        {
            if (logger) logger.Log("step_out_of_order", steps[stepIndex].stepId, objectName);
            return;
        }
        var step = steps[CurrentStep];
        bool expected = string.IsNullOrEmpty(step.expectedObjectName) || step.expectedObjectName == objectName;
        if (logger) logger.Log(expected ? "step_done" : "step_done_wrong_object", step.stepId, objectName);
        CurrentStep++;
        Announce();
    }

    /// <summary>Start the scenario again, for a new participant.</summary>
    public void Restart()
    {
        CurrentStep = 0;
        if (logger) logger.Log("scenario_restart", "", "");
        Announce();
    }

    void Announce()
    {
        if (logger) logger.Log(IsFinished ? "scenario_finished" : "step_start", Current ? Current.stepId : "", "");
        OnStepChanged?.Invoke(Current, CurrentStep);
    }
}

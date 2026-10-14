using UnityEngine;

/// <summary>
/// One step of the scenario, as data. Designers create these assets (right click, Create, XR Hackathon, Scenario Step),
/// fill in the text and drag them into the ScenarioController in order. No code needed to add or reorder steps.
/// </summary>
[CreateAssetMenu(menuName = "XR Hackathon/Scenario Step", fileName = "Step_")]
public class ScenarioStep : ScriptableObject
{
    [Tooltip("Short name used in the log file, e.g. install_filter")]
    public string stepId = "step";

    [Tooltip("What the panel shows the user while this step is active")]
    [TextArea(2, 4)] public string instruction = "Pick up the part and place it into the socket.";

    [Tooltip("Name of the scene object to highlight during this step (optional)")]
    public string targetName = "";

    [Tooltip("Name of the object the user is expected to use (optional, for error counting)")]
    public string expectedObjectName = "";
}

using TMPro;
using UnityEngine;

/// <summary>
/// Assigns one of two marketing framings per session and writes it into the message panel.
/// The assignment is logged, so the session data can be split by condition later.
/// Instructors can force a condition for demonstrations; Auto draws one at random per session.
/// </summary>
public class MarketingCondition : MonoBehaviour
{
    public enum Mode { Auto, ForceA, ForceB }

    [SerializeField] Mode mode = Mode.Auto;
    [SerializeField] TMP_Text headline;
    [SerializeField] TMP_Text body;
    [SerializeField] StepLogger logger;

    [Header("Condition A - sustainability framing")]
    [SerializeField] string headlineA = "DESIGNED TO STAY.";
    [TextArea(2, 4)]
    [SerializeField] string bodyA = "A modular approach to personal audio, designed with long-term use in mind.";

    [Header("Condition B - performance framing")]
    [SerializeField] string headlineB = "DESIGNED TO BE HEARD.";
    [TextArea(2, 4)]
    [SerializeField] string bodyB = "Transform everyday spaces through rich sound, intuitive control, and adaptive ambience.";

    /// <summary>"A" or "B" for the running session.</summary>
    public string Condition { get; private set; }

    void Start()
    {
        Condition = mode switch
        {
            Mode.ForceA => "A",
            Mode.ForceB => "B",
            _ => Random.value < 0.5f ? "A" : "B"
        };
        if (headline) headline.text = Condition == "A" ? headlineA : headlineB;
        if (body) body.text = Condition == "A" ? bodyA : bodyB;
        if (logger) logger.Log("condition_assigned", Condition, "");
    }
}

using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Runs the evaluation: three 1-to-7 ratings answered on the same row of buttons, then the
/// simulated purchase decision. Wire the seven rating buttons to Rate(int) and the two
/// decision buttons to DecideBuy()/DecidePass() in the Inspector. Every response is logged;
/// OnCompleted fires once when the decision is in, for the insight panel and the scenario.
/// </summary>
public class EvaluationStation : MonoBehaviour
{
    [SerializeField] TMP_Text question;
    [SerializeField] TMP_Text hint;
    [SerializeField] GameObject decisionGroup;
    [SerializeField] StepLogger logger;

    static readonly (string id, string text)[] Questions =
    {
        ("appeal", "How appealing is ECHO ONE to you?"),
        ("value", "How valuable does it feel?"),
        ("intent", "How likely would you be to consider buying it?")
    };

    /// <summary>Answers in question order; 0 while unanswered.</summary>
    public int[] Ratings { get; } = new int[3];

    /// <summary>True once the purchase decision was made.</summary>
    public bool IsComplete { get; private set; }

    /// <summary>"buy" (ECHO ONE), "lite" (the cheaper ECHO LITE) or "pass" after completion.</summary>
    public string Decision { get; private set; } = "";

    /// <summary>Fired once, after the purchase decision.</summary>
    public event Action OnCompleted;

    int current;

    void Start()
    {
        if (decisionGroup) decisionGroup.SetActive(false);
        ShowQuestion();
    }

    public void Rate(int value)
    {
        if (IsComplete || current >= Questions.Length) return;
        Ratings[current] = Mathf.Clamp(value, 1, 7);
        if (logger) logger.Log("rating", Questions[current].id, Ratings[current].ToString());
        current++;
        ShowQuestion();
    }

    public void DecideBuy() => Decide("buy");
    public void DecideLite() => Decide("lite");
    public void DecidePass() => Decide("pass");

    void Decide(string choice)
    {
        if (IsComplete || current < Questions.Length) return;
        IsComplete = true;
        Decision = choice;
        if (logger) logger.Log("purchase_decision", "", choice);
        if (question) question.text = choice switch
        {
            "buy" => "ECHO ONE it is. Your session summary is waiting on the opposite wall.",
            "lite" => "ECHO LITE it is - good value is a fair call. Your summary is on the opposite wall.",
            _ => "Not for you, and that is a fine answer. Your summary is on the opposite wall."
        };
        if (hint) hint.text = "";
        if (decisionGroup) decisionGroup.SetActive(false);
        OnCompleted?.Invoke();
    }

    void ShowQuestion()
    {
        if (current < Questions.Length)
        {
            if (question) question.text = $"{current + 1} / 3   {Questions[current].text}";
            if (hint) hint.text = "1 = not at all      7 = extremely";
        }
        else
        {
            if (question) question.text = "Which one would you take home?";
            if (hint) hint.text = "ECHO LITE is the cheaper sibling, described on the card but not on display. A simulated choice - no real money involved.";
            if (decisionGroup) decisionGroup.SetActive(true);
        }
    }
}

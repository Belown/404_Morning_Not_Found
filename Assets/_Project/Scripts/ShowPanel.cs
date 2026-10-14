using TMPro;
using UnityEngine;

/// <summary>
/// Controls the world space panel. Show(text) and Hide() can be wired from any event in the Inspector.
/// If a ScenarioController is assigned, the panel follows the scenario and shows each step's instruction.
/// </summary>
public class ShowPanel : MonoBehaviour
{
    [SerializeField] GameObject panelRoot;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text body;
    [SerializeField] ScenarioController scenario;   // optional
    [SerializeField] string finishedText = "Done. Thank you.";

    void OnEnable()  { if (scenario) scenario.OnStepChanged += OnStep; }
    void OnDisable() { if (scenario) scenario.OnStepChanged -= OnStep; }

    void OnStep(ScenarioStep step, int index)
    {
        if (step == null) { Show(finishedText); return; }
        if (title) title.text = $"Step {index + 1}";
        Show(step.instruction);
    }

    public void Show(string text)
    {
        if (body) body.text = text;
        if (panelRoot) panelRoot.SetActive(true);
    }

    public void Hide()
    {
        if (panelRoot) panelRoot.SetActive(false);
    }
}

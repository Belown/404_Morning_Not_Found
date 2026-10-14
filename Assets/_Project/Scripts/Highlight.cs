using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Highlights the target of the active scenario step by swapping its material, and restores it afterwards.
/// Targets are found by name among the renderers listed in the Inspector, so there is no scene search at runtime.
/// </summary>
public class Highlight : MonoBehaviour
{
    [SerializeField] ScenarioController scenario;
    [SerializeField] Material highlightMaterial;
    [SerializeField] Renderer[] targets;

    readonly Dictionary<Renderer, Material> original = new();
    Renderer current;

    void OnEnable()  { if (scenario) scenario.OnStepChanged += OnStep; }
    void OnDisable() { if (scenario) scenario.OnStepChanged -= OnStep; Restore(); }

    void OnStep(ScenarioStep step, int index)
    {
        Restore();
        if (step == null || string.IsNullOrEmpty(step.targetName)) return;
        foreach (var r in targets)
        {
            if (!r || r.name != step.targetName) continue;
            original[r] = r.material;
            r.material = highlightMaterial;
            current = r;
            return;
        }
        Debug.LogWarning($"Highlight: no renderer named {step.targetName} in the targets list");
    }

    void Restore()
    {
        if (current && original.TryGetValue(current, out var m)) current.material = m;
        current = null;
    }
}

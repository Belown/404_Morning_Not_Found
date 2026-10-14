using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Put this on a button (an XR Simple Interactable, e.g. the PokeButton prefab).
/// Pressing it completes the given step. Presses at other times are logged as out of order and ignored.
/// </summary>
public class PressStep : MonoBehaviour
{
    [SerializeField] XRBaseInteractable button;
    [SerializeField] ScenarioController scenario;
    [SerializeField] int stepIndex;

    void Reset() => button = GetComponent<XRBaseInteractable>();

    void OnEnable() => button.selectEntered.AddListener(OnPressed);
    void OnDisable() => button.selectEntered.RemoveListener(OnPressed);

    void OnPressed(SelectEnterEventArgs args) => scenario.Complete(stepIndex, name);
}

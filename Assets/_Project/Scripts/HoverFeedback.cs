using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Simple visual feedback without the affordance system: swap the material while hovered, play a clip on select.
/// Put it on any interactable. Fields: the interactable on the same object, its renderer, a hover material, an optional audio source and clip.
/// </summary>
public class HoverFeedback : MonoBehaviour
{
    [SerializeField] XRBaseInteractable interactable;
    [SerializeField] Renderer target;
    [SerializeField] Material hoverMaterial;
    [SerializeField] AudioSource audioSource;   // optional
    [SerializeField] AudioClip selectClip;      // optional

    Material original;

    void Reset()
    {
        interactable = GetComponent<XRBaseInteractable>();
        target = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
        interactable.selectEntered.AddListener(OnSelect);
    }

    void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEnter);
        interactable.hoverExited.RemoveListener(OnHoverExit);
        interactable.selectEntered.RemoveListener(OnSelect);
        if (target && original) target.material = original;
    }

    void OnHoverEnter(HoverEnterEventArgs a)
    {
        if (!target || !hoverMaterial) return;
        original = target.material;
        target.material = hoverMaterial;
    }

    void OnHoverExit(HoverExitEventArgs a)
    {
        if (target && original) target.material = original;
    }

    void OnSelect(SelectEnterEventArgs a)
    {
        if (audioSource && selectClip && !audioSource.isPlaying) audioSource.PlayOneShot(selectClip);
    }
}

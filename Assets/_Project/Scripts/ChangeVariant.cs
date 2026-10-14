using UnityEngine;

/// <summary>
/// Cycles a renderer through a list of materials. Wire Next() to a button's Select Entered event.
/// Put it on the product. Fields: the product's renderer and two or more materials.
/// </summary>
public class ChangeVariant : MonoBehaviour
{
    [SerializeField] Renderer target;
    [SerializeField] Material[] variants;
    [SerializeField] StepLogger logger;   // optional
    [Tooltip("Optional light that takes each variant's glow color, so a light-mood change is visible around the product")]
    [SerializeField] Light tintLight;

    int index;

    void Reset() => target = GetComponent<Renderer>();

    public int Index => index;

    /// <summary>How many times the user changed the variant this session.</summary>
    public int Changes { get; private set; }

    public void Next()
    {
        if (variants == null || variants.Length == 0 || !target) return;
        index = (index + 1) % variants.Length;
        target.material = variants[index];
        Tint();
        Changes++;
        if (logger) logger.Log("variant_changed", variants[index].name);
    }

    public void Set(int i)
    {
        if (variants == null || i < 0 || i >= variants.Length || !target) return;
        index = i;
        target.material = variants[index];
        Tint();
        Changes++;
        if (logger) logger.Log("variant_set", variants[index].name);
    }

    void Tint()
    {
        if (!tintLight) return;
        var m = variants[index];
        var c = m.HasProperty("_EmissionColor") ? m.GetColor("_EmissionColor") : m.color;
        // Glow colors are HDR (brighter than 1); a light only needs the hue.
        float max = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
        if (max > 1f) c /= max;
        c.a = 1f;
        tintLight.color = c;
    }
}

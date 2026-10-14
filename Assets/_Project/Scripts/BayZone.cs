using UnityEngine;

/// <summary>
/// A demonstration bay that reacts to the visitor. When the head enters the zone the bay's
/// product wakes up (light ring mood, soundtrack fades in) and dwell time starts counting;
/// leaving fades everything back down. With pulseWithAudio the ring follows the music's
/// loudness - a cheap envelope follower, no audio analysis package needed.
/// The zone is a box around this transform, so no colliders or physics are involved.
/// </summary>
public class BayZone : MonoBehaviour
{
    [SerializeField] string bayId = "FOCUS";
    [SerializeField] Vector3 size = new(3f, 3f, 3f);
    [SerializeField] Transform head;               // the Main Camera; found automatically if empty
    [SerializeField] Renderer productRing;         // light ring of the bay's product
    [SerializeField] Color ringColor = Color.white;
    [SerializeField] float ringIntensity = 2.5f;
    [SerializeField] AudioSource soundtrack;       // optional
    [SerializeField] float soundtrackVolume = 0.5f;
    [SerializeField] bool pulseWithAudio;
    [SerializeField] StepLogger logger;            // optional

    /// <summary>Seconds the visitor has spent inside this bay.</summary>
    public float DwellSeconds { get; private set; }

    public bool Inside { get; private set; }
    public string BayId => bayId;

    static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");
    readonly float[] samples = new float[128];
    MaterialPropertyBlock block;
    float glow;      // 0 = asleep, 1 = awake, eased
    float envelope;

    void Awake()
    {
        block = new MaterialPropertyBlock();
        if (!head && Camera.main) head = Camera.main.transform;
        if (soundtrack) soundtrack.volume = 0f;
        ApplyGlow();
    }

    void Update()
    {
        if (head) Tick(head.position, Time.deltaTime);
    }

    /// <summary>Advances the zone by dt with the head at headPosition. Public so tests can drive it.</summary>
    public void Tick(Vector3 headPosition, float dt)
    {
        var local = transform.InverseTransformPoint(headPosition);
        bool inside = Mathf.Abs(local.x) <= size.x / 2f && Mathf.Abs(local.z) <= size.z / 2f
                      && local.y >= 0f && local.y <= size.y;

        if (inside != Inside)
        {
            Inside = inside;
            if (inside && soundtrack && !soundtrack.isPlaying) soundtrack.Play();
            if (logger) logger.Log(inside ? "bay_entered" : "bay_left", bayId, "");
        }
        if (Inside) DwellSeconds += dt;

        glow = Mathf.MoveTowards(glow, Inside ? 1f : 0f, dt / 0.8f);
        if (soundtrack)
        {
            soundtrack.volume = glow * soundtrackVolume;
            if (glow <= 0f && soundtrack.isPlaying) soundtrack.Stop();
        }
        if (pulseWithAudio && soundtrack && soundtrack.isPlaying)
        {
            soundtrack.GetOutputData(samples, 0);
            float sum = 0f;
            for (int i = 0; i < samples.Length; i++) sum += samples[i] * samples[i];
            float rms = Mathf.Sqrt(sum / samples.Length);
            // Fast attack, slow release: reads as "breathing with the beat", not flicker.
            envelope = rms > envelope ? Mathf.Lerp(envelope, rms, 0.6f) : Mathf.Lerp(envelope, rms, 0.08f);
        }
        else envelope = 0f;
        ApplyGlow();
    }

    void ApplyGlow()
    {
        if (!productRing) return;
        float intensity = Mathf.Lerp(0.15f, ringIntensity, glow) * (1f + envelope * 6f);
        productRing.GetPropertyBlock(block);
        block.SetColor(EmissionId, ringColor * intensity);
        productRing.SetPropertyBlock(block);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(0.5f, 0.85f, 0.8f, 0.6f);
        Gizmos.DrawWireCube(new Vector3(0f, size.y / 2f, 0f), size);
    }
}

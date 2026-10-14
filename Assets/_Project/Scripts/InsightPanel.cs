using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// The payoff of the experience: when the evaluation completes, this panel reveals the session's
/// own story. The left column is data (mission, time per bay, configuration, ratings, choice);
/// the right column links each part to a consumer-behaviour idea, worded as a possibility,
/// never as a finding - one session describes one person.
/// Bars are drawn with rich text, so there is no chart library to learn.
/// </summary>
public class InsightPanel : MonoBehaviour
{
    [SerializeField] GameObject panelRoot;
    [SerializeField] TMP_Text dataColumn;
    [SerializeField] TMP_Text meaningColumn;
    [SerializeField] EvaluationStation evaluation;
    [SerializeField] MarketingCondition condition;
    [SerializeField] MissionSelector mission;          // optional
    [SerializeField] BayZone[] bays;                   // optional
    [SerializeField] ChangeVariant finish;             // optional
    [SerializeField] ChangeVariant lightMood;          // optional
    [SerializeField] Renderer shellRenderer;
    [SerializeField] Renderer ringRenderer;
    [SerializeField] ScenarioController scenario;
    [SerializeField] int completesStep = 4;

    [Header("Colors, as hex without #")]
    [SerializeField] string accent = "2F9584";
    [SerializeField] string track = "D8D2C8";
    [SerializeField] string muted = "5C646C";

    void OnEnable()
    {
        if (panelRoot) panelRoot.SetActive(false);
        if (evaluation != null) evaluation.OnCompleted += Reveal;
    }

    void OnDisable()
    {
        if (evaluation != null) evaluation.OnCompleted -= Reveal;
    }

    void Reveal()
    {
        if (scenario) scenario.Complete(completesStep, "Evaluation");
        if (!dataColumn || !panelRoot) return;
        dataColumn.text = BuildData();
        if (meaningColumn) meaningColumn.text = BuildMeaning();
        panelRoot.SetActive(true);
    }

    string BuildData()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<size=70%><color=#{accent}><b>YOUR SESSION</b></color></size>");
        if (mission && mission.Chosen >= 0) Row(sb, "Shopping for", mission.Current.title);
        if (shellRenderer) Row(sb, "Finish", MaterialLabel(shellRenderer));
        if (ringRenderer) Row(sb, "Light mood", MaterialLabel(ringRenderer));
        if (condition) Row(sb, "Message seen", condition.Condition == "A" ? "A - sustainability" : "B - performance");

        if (bays != null && bays.Length > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"<size=70%><color=#{accent}><b>TIME IN EACH SPACE</b></color></size>");
            float max = Mathf.Max(1f, bays.Max(b => b.DwellSeconds));
            foreach (var bay in bays)
                sb.AppendLine($"{bay.BayId}<pos=40%>{Bar(bay.DwellSeconds / max, 10)} {bay.DwellSeconds:0}s");
        }

        if (evaluation != null)
        {
            sb.AppendLine();
            sb.AppendLine($"<size=70%><color=#{accent}><b>YOUR RATINGS</b></color></size>");
            sb.AppendLine($"Appeal<pos=40%>{Bar(evaluation.Ratings[0] / 7f, 10)} {evaluation.Ratings[0]}/7");
            sb.AppendLine($"Value<pos=40%>{Bar(evaluation.Ratings[1] / 7f, 10)} {evaluation.Ratings[1]}/7");
            sb.AppendLine($"Purchase intent<pos=40%>{Bar(evaluation.Ratings[2] / 7f, 10)} {evaluation.Ratings[2]}/7");
            sb.AppendLine();
            Row(sb, "Your choice", evaluation.Decision switch
            {
                "buy" => "ECHO ONE, €249",
                "lite" => "ECHO LITE, €149",
                _ => "Neither"
            });
        }
        return sb.ToString();
    }

    string BuildMeaning()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<size=70%><color=#{accent}><b>WHAT IT CAN SUGGEST</b></color></size>");

        int changes = (finish ? finish.Changes : 0) + (lightMood ? lightMood.Changes : 0);
        sb.AppendLine($"<b>Making it yours.</b> You changed the design {changes} time{(changes == 1 ? "" : "s")}. " +
                      "Studies of psychological ownership suggest people can value what they helped shape a little more.");
        sb.AppendLine();
        var top = bays != null && bays.Length > 0 ? bays.OrderByDescending(b => b.DwellSeconds).First() : null;
        if (top != null && top.DwellSeconds > 1f)
        {
            sb.AppendLine($"<b>Seeing it in use.</b> You lingered longest in {top.BayId}. " +
                          "Picturing a product in your own routine is one way experiences make value tangible.");
            sb.AppendLine();
        }
        if (condition)
        {
            sb.AppendLine("<b>Framing.</b> Others see the other message. Only comparing many visitors across both " +
                          "can show whether a framing shifts value - one visit cannot.");
            sb.AppendLine();
        }
        sb.AppendLine("<b>Reference prices.</b> The €149 option is a reference point: it changes how €249 feels, " +
                      "whichever you pick.");
        sb.AppendLine();
        sb.AppendLine($"<size=80%><color=#{muted}>Every interaction tells a story. This one describes you, " +
                      "not people in general.</color></size>");
        return sb.ToString();
    }

    void Row(StringBuilder sb, string label, string value)
        => sb.AppendLine($"{label}<pos=40%><b>{value}</b>");

    string Bar(float fraction, int cells)
    {
        int filled = Mathf.Clamp(Mathf.RoundToInt(fraction * cells), 0, cells);
        return $"<color=#{accent}>{new string('█', filled)}</color><color=#{track}>{new string('█', cells - filled)}</color>";
    }

    static string MaterialLabel(Renderer renderer)
    {
        // Material names like "Shell_Terracotta" or "Ring_Mint" read fine after the prefix.
        var name = renderer.sharedMaterial ? renderer.sharedMaterial.name : "";
        name = name.Replace(" (Instance)", "");
        int cut = name.IndexOf('_');
        return cut >= 0 ? name[(cut + 1)..] : name;
    }
}

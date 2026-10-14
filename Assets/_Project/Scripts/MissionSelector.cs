using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Gives the visit a purpose: the participant says what they are shopping for before exploring.
/// Wire each mission button's Select Entered event to Choose(index). The choice is logged, the
/// matching bay's marker switches on, and listeners (the insight panel) can read it later.
/// </summary>
public class MissionSelector : MonoBehaviour
{
    [Serializable]
    public struct Mission
    {
        public string id;          // short log name, e.g. work_from_home
        public string title;       // what the participant reads, e.g. Working from home
        public string bay;         // the bay that serves this need, e.g. FOCUS
        public GameObject marker;  // switched on to point at that bay (optional)
    }

    [SerializeField] Mission[] missions;
    [SerializeField] TMP_Text confirmation;   // optional
    [SerializeField] StepLogger logger;       // optional

    /// <summary>Index of the chosen mission, -1 before a choice.</summary>
    public int Chosen { get; private set; } = -1;

    public Mission Current => Chosen >= 0 ? missions[Chosen] : default;

    public event Action<Mission> OnChosen;

    void Start()
    {
        foreach (var m in missions)
            if (m.marker) m.marker.SetActive(false);
    }

    public void Choose(int index)
    {
        if (missions == null || index < 0 || index >= missions.Length) return;
        if (Chosen >= 0 && missions[Chosen].marker) missions[Chosen].marker.SetActive(false);
        Chosen = index;
        var mission = missions[index];
        if (mission.marker) mission.marker.SetActive(true);
        if (confirmation)
            confirmation.text = $"Shopping for: <b>{mission.title}</b>. Start with the {mission.bay} space when you are ready.";
        if (logger) logger.Log("mission_chosen", mission.id, mission.bay);
        OnChosen?.Invoke(mission);
    }
}

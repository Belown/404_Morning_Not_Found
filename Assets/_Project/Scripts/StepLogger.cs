using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Writes one CSV file per session to Application.persistentDataPath/logs.
/// Columns: timestamp, t_since_start_s, event, step, object, head_x, head_y, head_z, head_yaw, session, participant, condition.
/// Events come from the scenario (step_start, step_done, ...) and from any Log() call you wire in the Inspector.
/// A head pose row ("head") is written every sampleInterval seconds so Session 5 can trace where the user stood and looked.
/// Pull the files with Meta Quest Developer Hub, File Manager, or adb pull.
/// </summary>
public class StepLogger : MonoBehaviour
{
    [Header("Who and what")]
    [SerializeField] string session = "S01";
    [SerializeField] string participant = "P01";
    [SerializeField] string condition = "A";

    [Header("Head pose sampling")]
    [SerializeField] Transform head;               // the Main Camera under the XR Origin
    [SerializeField] float sampleInterval = 0.5f;  // seconds, 0 disables

    StreamWriter writer;
    float startTime;
    float nextSample;
    string path;

    void Awake()
    {
        var dir = Path.Combine(Application.persistentDataPath, "logs");
        Directory.CreateDirectory(dir);
        path = Path.Combine(dir, $"{session}_{participant}_{condition}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        writer = new StreamWriter(path, false, Encoding.UTF8);
        writer.WriteLine("timestamp,t_since_start_s,event,step,object,head_x,head_y,head_z,head_yaw,session,participant,condition");
        startTime = Time.time;
        if (!head && Camera.main) head = Camera.main.transform;
        Debug.Log($"StepLogger writing to {path}");
    }

    void Update()
    {
        if (sampleInterval <= 0 || !head || Time.time < nextSample) return;
        nextSample = Time.time + sampleInterval;
        Write("head", "", "");
    }

    /// <summary>Public, one or two arguments, so it shows up in Inspector event lists.</summary>
    public void Log(string eventName) => Write(eventName, "", "");
    public void Log(string eventName, string objectName) => Write(eventName, "", objectName);
    public void Log(string eventName, string step, string objectName) => Write(eventName, step, objectName);

    /// <summary>Set before the participant starts, e.g. from a start menu.</summary>
    public void SetParticipant(string id) => participant = id;
    public void SetCondition(string id) => condition = id;

    void Write(string eventName, string step, string objectName)
    {
        if (writer == null) return;
        var c = CultureInfo.InvariantCulture;
        var p = head ? head.position : Vector3.zero;
        var yaw = head ? head.eulerAngles.y : 0f;
        writer.WriteLine(string.Join(",",
            DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff", c),
            (Time.time - startTime).ToString("F3", c),
            Clean(eventName), Clean(step), Clean(objectName),
            p.x.ToString("F3", c), p.y.ToString("F3", c), p.z.ToString("F3", c), yaw.ToString("F1", c),
            Clean(session), Clean(participant), Clean(condition)));
        writer.Flush();
    }

    static string Clean(string s) => string.IsNullOrEmpty(s) ? "" : s.Replace(",", ";").Replace("\n", " ");

    void OnApplicationPause(bool paused) { if (paused) writer?.Flush(); }
    void OnDestroy() { writer?.Flush(); writer?.Dispose(); writer = null; }
}

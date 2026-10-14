using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Load(sceneName) from any event. The scene must be in the Build Profile's scene list.</summary>
public class SceneSwitcher : MonoBehaviour
{
    public void Load(string sceneName) => SceneManager.LoadScene(sceneName);
    public void Reload() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}

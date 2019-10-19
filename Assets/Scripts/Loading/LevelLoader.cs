using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI progressText;

    int i = 0;
    public void Awake()
    {
        progressText.text = "0%";
        slider.value = 0;
    }
    public void Start()
    {
        LoadLevel(PlayerPrefs.GetInt("SceneToLoad"));
    }
    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        while (i <= 5) {
            ++i;
            yield return new WaitForSeconds(0.3f);
        }
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log(progress);
            slider.value = progress;
            progressText.text = progress * 100f + "%";
            yield return null;
        }
    }
}

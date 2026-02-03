using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager singleton;
    public SaveData saveData;

    private void Start()
    {
        DontDestroyOnLoad(this);
        singleton = this;

        LocalizationSettings.InitializationOperation.WaitForCompletion(); //WebGLPlayer does not support synchronous Addressable loading

        LoadScene("MenuScene");
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void LoadData()
    {
        saveData = SaveManager.LoadData();
    }
}

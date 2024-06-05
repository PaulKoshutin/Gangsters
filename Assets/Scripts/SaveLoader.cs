using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoader: MonoBehaviour
{
    public static SaveLoader Instance { get; private set; }
    public bool loading;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        DontDestroyOnLoad(this.gameObject);

        loading = false;
    }
    public void Save()
    {
        CharPool.Instance.Save();
        ActiveEntities.Instance.Save();
    }
    public void Load()
    {
        CharPool.Instance.Load();
        ActiveEntities.Instance.Load();
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene" && loading)
            Load();
    }
}

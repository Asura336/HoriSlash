using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIEvent : MonoBehaviour
{
    [SerializeField] RectTransform onAlive;
    [SerializeField] RectTransform onDead;
    public Text scoreHUD;
    public Text deadScorePad;

    private static UIEvent _instance = null;
    public static UIEvent Instance
    {
        get { return _instance; }
    }

    [HideInInspector] public int kills = 0;
    [HideInInspector] public int continuousKils = 0;
    [HideInInspector] public int maxContinuousKill = 0;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    // Use this for initialization
    void Start()
    {
        OnPlayerAlive();
        OnRefreshHUD();
    }

    public void OnPlayerAlive()
    {
        onAlive.gameObject.SetActive(true);
        onDead.gameObject.SetActive(false);
    }

    public void OnPlayerDead()
    {
        onAlive.gameObject.SetActive(false);
        onDead.gameObject.SetActive(true);

        deadScorePad.text = string.Format("总计消灭数: {0}\n最多连续消灭: {1}",
            kills, maxContinuousKill);
    }

    public void OnRefreshHUD()
    {
        scoreHUD.text = string.Format("消灭: {0}\n连续消灭: {1}", kills, continuousKils);
    }

    public void OnClick_ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClick_ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

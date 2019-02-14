using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 在欢迎界面使用
/// </summary>
public class WelcomeUI : MonoBehaviour
{
    public Button bgmSwitcher;
    public Button soundSwitcher;
    public AudioSource bgmAudio;
    public AudioSource characterAudio;

    private Image bgmSwitcherImage;
    private Image soundSwitcherImage;

    public int mainGameScene = 1;

    private void Start()
    {
        bgmSwitcherImage = bgmSwitcher.GetComponent<Image>();
        soundSwitcherImage = soundSwitcher.GetComponent<Image>();

        bgmSwitcherImage.color = GameMode.Instance.HasMusic ? GameMode.Instance.typeTwo : Color.white;
        soundSwitcherImage.color = GameMode.Instance.HasSound ? GameMode.Instance.typeOne : Color.white;

        bgmAudio.mute = !GameMode.Instance.HasMusic;
    }

    public void OnClick_GameStart()
    {
        StartCoroutine(_SlowStart(0.5f));
    }
    IEnumerator _SlowStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadSceneAsync(mainGameScene);
    }

    public void OnClick_SwitchBgm()
    {
        GameMode.Instance.HasMusic = !GameMode.Instance.HasMusic;
        bgmAudio.mute = !GameMode.Instance.HasMusic;
        bgmSwitcherImage.color = GameMode.Instance.HasMusic ? GameMode.Instance.typeTwo : Color.white;
    }

    public void OnClick_SwitchSound()
    {
        GameMode.Instance.HasSound = !GameMode.Instance.HasSound;
        characterAudio.mute = !GameMode.Instance.HasSound;
        soundSwitcherImage.color = GameMode.Instance.HasSound ? GameMode.Instance.typeOne : Color.white;
    }
}

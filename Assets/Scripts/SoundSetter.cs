using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSetter : MonoBehaviour
{
    public AudioSource playerAudio;
    public AudioSource enemyAudio;
    public AudioSource bgmAudio;

    // 调试用
    [SerializeField] bool _hasSound = true;
    [SerializeField] bool _hasMusic = true;

    private void Start()
    {
        GameMode.Instance.SoundSetter = this;

#if UNITY_EDITOR
        //GameMode.Instance.HasSound = _hasSound;
        //GameMode.Instance.HasMusic = _hasMusic;
#endif

        OnSoundSetting();
        OnBgmSetting();
    }

    public void OnSoundSetting()
    {
        playerAudio.mute = !GameMode.Instance.HasSound;
        enemyAudio.mute = !GameMode.Instance.HasSound;
    }

    public void OnBgmSetting()
    {
        bgmAudio.mute = !GameMode.Instance.HasMusic;
    }
}

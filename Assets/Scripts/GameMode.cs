using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameMode
{
    private static GameMode _instance;
    public static GameMode Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameMode();
            }
            return _instance;
        }
    }

    public const float G = 9.81f;
    public const int EnemyLayerMask = 1 << 10;  // 怪物 Layer 应该设置在第 10 层
    public bool IsPlayerAlive { get; set; }
    public Color typeOne = new Color(1, 0.29f, 0);
    public Color typeTwo = new Color(0, 0.29f, 1);
    public Transform PlayerTrans { get; set; }  // 在别处赋值
    public Character Player { get; set; }  // 在别处赋值

    public SoundSetter SoundSetter { get; set; }
    // 游戏音效开关
    bool _hasSound = true;
    public bool HasSound
    {
        get { return _hasSound; }
        set { _hasSound = value; }
    }
    // 游戏音乐开关
    bool _hasMusic = false;
    public bool HasMusic
    {
        get { return _hasMusic; }
        set { _hasMusic = value; }
    }

    public Dictionary<float, MonsterTimeLine> DicMonsterTLs { get; private set; }

    private const string _csvFile = "Text/_MonsterTimeLine";
    private const string _scriptFile = "timeLineScript.txt";

    private GameMode()
    {
        if (DicMonsterTLs == null) { DicMonsterTLs = new Dictionary<float, MonsterTimeLine>(); }
#if UNITY_ANDROID
        string androidPath = Application.persistentDataPath + _scriptFile;
        BuildToDic(androidPath);
#elif UNITY_EDITOR
        BuildToDic(_scriptFile);
#elif UNITY_STANDALONE_WIN
        BuildToDic(_scriptFile);
#else
        RawBuildToDic();
#endif
    }

    #region 序列化, 反序列化相关

    void _StreamToDic(string[] lines)
    {
        foreach (var line in lines)
        {
            var array = line.Split(',');
            float _time;  // 0
            float _deltaZ;  // 1
            float _speed;  // 4
            char _actType;  // 2
            char _colorType;  // 3
            if (float.TryParse(array[0], out _time) &&
                float.TryParse(array[1], out _deltaZ) &&
                float.TryParse(array[4], out _speed) &&
                char.TryParse(array[2], out _actType) &&
                char.TryParse(array[3], out _colorType))
            {
                DicMonsterTLs[_time] = new MonsterTimeLine(_time, _deltaZ, _actType, _colorType, _speed);
            }
        }
    }

    void BuildToDic(string path)
    {
        string text;
        if (File.Exists(path))
        {
            using (StreamReader sr = File.OpenText(path))
            {
                text = sr.ReadToEnd();
            }
        }
        else
        {
            text = Resources.Load<TextAsset>(_csvFile).text;
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write(text);
            }
        }
        _StreamToDic(text.Split('\r', '\n'));
    }

    void RawBuildToDic()
    {
        string fileStream = Resources.Load<TextAsset>(_csvFile).text;
        _StreamToDic(fileStream.Split('\r', '\n'));
    }

#endregion

    public void OnPlayerDead()
    {
        if (UIEvent.Instance != null) { UIEvent.Instance.OnPlayerDead(); }
    }

    public void OnMonsterDead()
    {
        if (UIEvent.Instance != null)
        {
            UIEvent.Instance.kills++;
            UIEvent.Instance.continuousKils++;
            if (UIEvent.Instance.continuousKils > UIEvent.Instance.maxContinuousKill)
            { UIEvent.Instance.maxContinuousKill = UIEvent.Instance.continuousKils; }
            UIEvent.Instance.OnRefreshHUD();
        }
    }

    public void OnPlayerFaint()
    {
        if (UIEvent.Instance != null)
        {
            UIEvent.Instance.continuousKils = 0;
            UIEvent.Instance.OnRefreshHUD();
        }
    }
}

public class MonsterTimeLine
{
    public float pTime;
    public float deltaZ;
    public char actType;  // F, N, J
    public char colorType;  // O, T
    public float mSpeed;

    public MonsterTimeLine(float pt, float dz, char act, char clt, float spd)
    {
        pTime = pt;
        deltaZ = dz;
        actType = act;
        colorType = clt;
        mSpeed = spd;
    }

    public override string ToString()
    {
        string act = (actType == 'F') ? "fall back" : 
                        ((actType == 'N') ? "near" : 
                            ((actType == 'J') ? "jumpable" : "None"));
        string clr = (colorType == 'O') ? "type one" :
                        (colorType == 'T') ? "type two" : "None";

        return string.Format("time = {0}, dZ = {1}, act = {2}, color = {3}, speed = {4}",
            pTime, deltaZ, act, clr, mSpeed);
    }
}
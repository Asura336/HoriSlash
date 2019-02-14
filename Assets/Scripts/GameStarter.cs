using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏的启动器，从 GameMode 拉取信息
/// </summary>
public class GameStarter : MonoBehaviour
{
    KeyValuePair<float, MonsterTimeLine>[] collection;
    Dictionary<char, MonsterActState> dicActState = new Dictionary<char, MonsterActState>
    {
        { 'F', MonsterActState.FALLBACK },
        { 'J', MonsterActState.JUMPABLE },
        { 'N', MonsterActState.NEAR }
    };
    Dictionary<char, MonsterColor> dicColorType = new Dictionary<char, MonsterColor>
    {
        { 'O', MonsterColor.TYPE_ONE },
        { 'T', MonsterColor.TYPE_TWO }
    };

    Transform playerTrans;
    float pTime = 0;
    int pCollection = 0;

    //public Text consola;

    // Use this for initialization
    void Start()
    {
        playerTrans = GameMode.Instance.PlayerTrans;

        var dic = GameMode.Instance.DicMonsterTLs;
        var col = from single in dic orderby single.Key select single;
        collection = col.ToArray();

        //foreach (var single in collection)
        //{
        //    Debug.Log(single.Value);
        //    consola.text += single.Value.ToString() + '\n';
        //}
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerTrans == null) { return; }

        if (Mathf.Abs(collection[pCollection].Key - pTime) < 0.02f)
        {
            var currentVal = collection[pCollection].Value;
            MonsterActState state = dicActState.ContainsKey(currentVal.actType) ? 
                dicActState[currentVal.actType] : MonsterActState.NONE;
            MonsterColor color = dicColorType.ContainsKey(currentVal.colorType) ? 
                dicColorType[currentVal.colorType] : MonsterColor.NONE;

            EnemyFactory.Instance.SetInstance(
               new Vector3(0, 0, playerTrans.position.z + currentVal.deltaZ),
               state,
               color,
               currentVal.mSpeed);

            pCollection++;
        }
        else if (pTime - collection[pCollection].Key > 1)
        {
            pCollection++;
        }
        if (pCollection == collection.Length)
        {
            pCollection = 0;
            pTime = 0;
        }
        if (EnemyFactory.Instance.ActiveCount < 5 && GameMode.Instance.Player.IsAlive)
        { pTime += Time.fixedDeltaTime; }
    }
}

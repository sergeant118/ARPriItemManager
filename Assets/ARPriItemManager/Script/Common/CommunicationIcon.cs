using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通信中アイコンを管理するクラス
/// </summary>
public class CommunicationIcon : SingletonMonoBehaviour<CommunicationIcon> {

    [SerializeField]
    GameObject Communication = null;

    protected override void Awake()
    {
        base.Awake();

        CommunicationOff();
    }

    /// <summary>
    /// 通信中アイコンを表示
    /// </summary>
    public void CommunicationOn()
    {
        Communication.SetActive(true);
    }

    /// <summary>
    /// 通信中アイコンを非表示
    /// </summary>
    public void CommunicationOff()
    {
        Communication.SetActive(false);
    }
}

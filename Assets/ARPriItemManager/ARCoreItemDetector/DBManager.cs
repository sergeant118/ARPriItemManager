using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

[System.Serializable]
public class ConfigTuple
{
    public string config_name;

    public ARCoreSessionConfig config;
}

public class DBManager : MonoBehaviour
{
    [SerializeField]
    ARCoreSession session = null;

    [SerializeField]
    ConfigTuple[] config_tuples = null;

    public void ChangeDB(Dropdown dd)
    {
        session.SessionConfig = config_tuples[dd.value].config;
    }
}

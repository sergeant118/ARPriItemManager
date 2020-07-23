using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindScreen : SingletonMonoBehaviour<BlindScreen>{

    [SerializeField]
    GameObject blindPanel = null;

    public void blindOn()
    {
        blindPanel.SetActive(true);
    }

    public void blindOff()
    {
        blindPanel.SetActive(false);
    }
}

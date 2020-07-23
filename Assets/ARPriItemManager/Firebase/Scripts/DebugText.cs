using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : SingletonMonoBehaviour<DebugText>
{

    Text debug_text;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        debug_text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ADDText(string text)
    {
        debug_text.text = debug_text.text + text + "\n";
    }

    public void ClearText()
    {
        debug_text.text = "";
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AppEnd : SingletonMonoBehaviour<AppEnd> {

    [SerializeField]
    GameObject end_panel = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Application.platform == RuntimePlatform.Android)
        {
            // エスケープキー取得
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // アプリケーション終了
                opemWindow();
                return;
            }
        }

    }

    public void endApp()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #elif UNITY_ANDROID
        Application.Quit();
        #endif
    }

    public void opemWindow()
    {
        end_panel.SetActive(true);
    }

    public void closeWindow() {
        end_panel.SetActive(false);
    }

    
    public void restrtApp()
    {
        SceneManager.LoadScene("Detector");
    }
    
}

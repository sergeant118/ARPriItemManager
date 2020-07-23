using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.SceneManagement;

public class menu : MonoBehaviour {

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        /*
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("SceneViforiaManager");

            SceneManager.LoadSceneAsync("SceneViforiaManager", LoadSceneMode.Additive);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("VuforiaManager.Instance.startViforiaAR()");

            var db_list = new List<string>();

            db_list.Add("PCH5");
            db_list.Add("RandY");

            VuforiaManager.Instance.setDatabase_List(db_list);
            VuforiaManager.Instance.startViforiaAR();
        }
        */

        /*
        if (Input.GetKeyDown(KeyCode.E))
        {
            opneMenu();
        }*/

    }

    public void ListButtonEvent()
    {
        var scene_name = "SceneCoodinateListPDB";
        Debug.Log("SceneCoodinateListPDB");

        if (!ContainsScene(scene_name))
        {
            Debug.Log("LoadSceneAsync:" + scene_name);
            SceneManager.LoadSceneAsync(scene_name, LoadSceneMode.Additive);
        }
        else
        {
            Debug.Log("ScrollViewManager.Instance.changeDisplayCanvas()");
            ScrollViewManagerPDB.Instance.changeDisplayCanvas();
        }
    }

    public void ARButtonEvent()
    {
        var scene_name = "ARCoreItemDetector";
        Debug.Log(scene_name);

        if (!ContainsScene(scene_name))
        {
            Debug.Log("LoadSceneAsync:" + scene_name);
            SceneManager.LoadSceneAsync(scene_name, LoadSceneMode.Additive);
        }
        else
        {
            /*
            if (VuforiaManager.Instance.isVuforiaCameraEnabled())
                VuforiaManager.Instance.vuforiaCameraOff();
            else
                VuforiaManager.Instance.vuforiaCameraOn();
                */

            //シーン破棄したら二回目のロードでエラー吐くのでいったん中止
            //SceneManager.UnloadSceneAsync(scene_name);

        }
    }

    public void StarButtonEvent()
    {

        //PUFirebaseTwitterLogin.Instance.importItemFromGAS();

        /*
        var scene_name = "SceneCoodinateList";
        Debug.Log("SceneCoodinateList");

        if (!ContainsScene(scene_name))
        {
            Debug.Log("LoadSceneAsync:" + scene_name);
            SceneManager.LoadSceneAsync(scene_name, LoadSceneMode.Additive);
        }
        else
        {
            Debug.Log("ScrollViewManager.Instance.changeDisplayCanvas()");
            ScrollViewManager.Instance.changeDisplayCanvas();
        }
        */
    }

    bool ContainsScene(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
    {
            if (SceneManager.GetSceneAt(i).name == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}

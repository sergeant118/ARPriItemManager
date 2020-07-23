using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionWindowManager : SingletonMonoBehaviour<DecisionWindowManager> {


    [SerializeField]
    GameObject decision_window = null;

    //ItemJSON item;
    PrismdbBindingsJSON item_pdb = null;
    CanvasManager cm = null;

    List<CanvasManager> cm_list = new List<CanvasManager>();
    int canvas_index = 0;

    [SerializeField]
    Text debug_text = null;

    //[SerializeField]
    //RawImage image = null;

    // Use this for initialization
    void Start () {
        decision_window.SetActive(false);
    }

    public void addCanvasManager(CanvasManager cm)
    {
        cm_list.Add(cm);
    }

    public void openWindow()
    {
        cm = cm_list[canvas_index];

        var item_json = JsonUtility.ToJson(cm.getItemPDB());

        item_pdb = JsonUtility.FromJson<PrismdbBindingsJSON>(item_json);

        string name = item_pdb.id.value + "\n" + item_pdb.name.value;

        debug_text.text = name;

        canvas_index++;

        decision_window.SetActive(true);
    }

    public void closeWindow()
    {
        if (canvas_index < cm_list.Count)
            openWindow();
        else
        {
            canvas_index = 0;
            cm_list.Clear();

            decision_window.SetActive(false);
        }
    }

    public void decisionButtonEvent()
    {
        closeWindow();

        string id = item_pdb.id.value;

        int possession_num = PUFirebaseTwitterLogin.Instance.getItemPosessionNum(id) + 1;

        cm.setHaveStatus(true);
        cm.setPosessionNum(possession_num);

        PUFirebaseTwitterLogin.Instance.updateItem(id, possession_num);
    }
}

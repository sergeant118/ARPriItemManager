using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PossessionWindowManagerPDB : SingletonMonoBehaviour<PossessionWindowManagerPDB>
{

    [SerializeField]
    GameObject possession_window = null;

    [SerializeField]
    Text item_name_label = null;

    [SerializeField]
    Text possession_num_label = null;

    private ContentManager cm = null;
    private PrismdbBindingsJSON item = null;

    private int current_possession_num = 0;

    public void OpenWindow(ContentManager cm, PrismdbBindingsJSON item)
    {
        possession_window.SetActive(true);
        this.cm = cm;
        this.item = item;

        int possession_num = PUFirebaseTwitterLogin.Instance.getItemPosessionNum(item.id.value);

        current_possession_num = possession_num;
        possession_num_label.text = possession_num.ToString();
        item_name_label.text = item.name.value;

    }

    public void closeWindow()
    {
        possession_window.SetActive(false);
    }

    public void UpButtonEvent()
    {
        current_possession_num += 1;

        possession_num_label.text = current_possession_num.ToString();
    }

    public void DownButtonEvent()
    {
        current_possession_num -= 1;

        possession_num_label.text = current_possession_num.ToString();
    }

    /// <summary>
    /// 所持数を更新するボタンのイベント
    /// </summary>
    public void possessionUpdateButton()
    {
        //firebaseにも投げる
        PUFirebaseTwitterLogin.Instance.updateItem(item.id.value, current_possession_num);

        //コンテンツも更新
        cm.setItem(item);

        //更新したら閉じる
        closeWindow();
    }
}
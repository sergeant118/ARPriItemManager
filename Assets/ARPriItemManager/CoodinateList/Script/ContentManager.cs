using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentManager : MonoBehaviour
{

    [SerializeField]
    Text coord_label = null;

    [SerializeField]
    RawImage item_raw_image = null;

    [SerializeField]
    Image content_bg = null;

    [SerializeField]
    Text possesion_label = null;

    [SerializeField]
    Color possesion_on = new Color(1, 0, 0);

    [SerializeField]
    Color possesion_off = new Color(1, 0, 0);

    private PrismdbBindingsJSON item_pdb = null;

    private bool textureApplayFlag = false;

    public void Update()
    {
        if (textureApplayFlag)
        {
            if (item_pdb != null)
            {

                if (item_pdb.image_num.value != null)
                {
                    textureApplayFlag = false;
                    return;
                }

                string image_num = item_pdb.image_num.value.ToString();

                item_raw_image.texture = LoadLocalTexture.Instance.getTexture2D(image_num);

                textureApplayFlag = false;
            }
        }
    }

    public void setItem(PrismdbBindingsJSON item)
    {
        this.item_pdb = item;

        //名前を入力
        setName(item.name.value);

        int possession_num = PUFirebaseTwitterLogin.Instance.getItemPosessionNum(item.id.value);

        //所持状態で背景色を変更
        setPossessionNum(possession_num);

        //画像をセット
        if (item.image_num.value != null)
            setImage(item.image_num.value.ToString());
        else
            setImage("0");
    }

    /// <summary>
    /// アイテム名をセットする
    /// </summary>
    /// <param name="coord_name"></param>
    private void setName(string coord_name)
    {
        coord_label.text = coord_name;
    }

    /// <summary>
    /// 所持数をセットする
    /// </summary>
    /// <param name="possession_num"></param>
    private void setPossessionNum(int possession_num)
    {
        if (possession_num > 0)
            content_bg.color = possesion_on;
        else
            content_bg.color = possesion_off;

        if (possession_num < 0)
            possesion_label.text = "0";
        else
            possesion_label.text = possession_num.ToString();
    }

    /// <summary>
    /// 画像をセットする
    /// </summary>
    /// <param name="image_num"></param>
    private void setImage(string image_num)
    {
        LoadLocalTexture.Instance.setTexture2D(this, image_num);
    }

    /// <summary>
    /// 画像をセットする
    /// </summary>
    /// <param name="texture"></param>
    public void setImage(Texture2D texture)
    {
        item_raw_image.texture = texture;
    }

    /// <summary>
    /// テクスチャの更新フラグを立てる
    /// </summary>
    public void refleshImage()
    {
        textureApplayFlag = true;
    }

    /// <summary>
    /// ボタンイベント　個数変更ウィンドウを開く
    /// </summary>
    public void buttonEvent()
    {
        if (PossessionWindowManagerPDB.Instance != null)
            PossessionWindowManagerPDB.Instance.OpenWindow(this, item_pdb);

    }
}

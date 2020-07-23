using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeManager : MonoBehaviour
{

    [SerializeField]
    GameObject contents_root = null;

    [SerializeField]
    Text outfit_text = null;

    public void setContentsActive(bool status)
    {
        contents_root.SetActive(status);
    }

    public void setOutfitName(string outfit_name)
    {

        if (outfit_name != "パシャっとアイテム")
            outfit_text.text = outfit_name + "コーデ";
        else
            outfit_text.text = outfit_name;
    }

    public void addContentToNode(GameObject content)
    {
        content.transform.SetParent(contents_root.transform);

        content.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1f);

    }

    public void activeContents(int index, List<PrismdbBindingsJSON> item_list)
    {
        //数が足りていれば取得してアクティブ化
        var content = contents_root.transform.GetChild(index).gameObject;
        content.SetActive(true);

        //ここで数値を更新すればいい
        content.name = item_list[index].name.value;

        ContentManager cm = content.GetComponent<ContentManager>();
        cm.setItem(item_list[index]);

        //Debug.Log(gameObject.name + "の" + index + "をアクティブ＆更新");
    }

    public void deactivateContents(int index)
    {

        contents_root.transform.GetChild(index).gameObject.SetActive(false);

        //Debug.Log(gameObject.name + "の" + index + "を非アクティブ");
    }

    public int getContentNum()
    {
        return contents_root.transform.childCount;
    }
}

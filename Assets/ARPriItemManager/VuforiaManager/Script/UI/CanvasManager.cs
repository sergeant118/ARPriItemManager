using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {

    [SerializeField]
    Text coordName = null, possesionNum = null;

    [SerializeField]
    Image raw = null;

    [SerializeField]
    Sprite hav_tex = null;

    [SerializeField]
    Sprite not_hav_tex = null;

    [SerializeField]
    private ItemJSON item = null;

    [SerializeField]
    private PrismdbBindingsJSON item_pdb = null;

    public void setItem(ItemJSON _item)
    {
        item = _item;
    }

    public void setItem(PrismdbBindingsJSON _item)
    {
        item_pdb = _item;
    }

    public ItemJSON getItem()
    {
        return item;// = _item;
    }

    public PrismdbBindingsJSON getItemPDB()
    {
        return item_pdb;// = _item;
    }

    public void setCoordName(string name)
    {
        coordName.text = name;
    }

    public void setHaveStatus(bool status)
    {
        if (status)
        {
            raw.sprite = hav_tex;
        }
        else
        {
            raw.sprite = not_hav_tex;
        }
    }

    public void setPosessionNum(int possession_num)
    {

        possesionNum.text = possession_num.ToString();


        if (possession_num > 0)
        {
            raw.sprite = hav_tex;
        }
        else
        {
            raw.sprite = not_hav_tex;
        }

    }

    public void ButtonEvent()
    {
        //DecisionWindowManager.Instance.openWindow(item, this);
    }
}

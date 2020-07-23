using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RarityToggle : MonoBehaviour
{

    [SerializeField]
    //string rarity = "";

    ItemRarity rarity;

    [SerializeField]
    Image image = null;

    [SerializeField]
    Sprite toggle_on = null;

    [SerializeField]
    Sprite toggle_off = null;

    public void toggle_changed(bool status)
    {

        if (status)
        {
            image.sprite = toggle_on;

            if (ScrollViewManagerPDB.Instance != null)
            {

                ScrollViewManagerPDB.Instance.rarityToggleEvent(rarity);
            }
        }
        else
            image.sprite = toggle_off;
    }

}

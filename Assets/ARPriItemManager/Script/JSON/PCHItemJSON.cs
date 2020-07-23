using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// GASから取得した生データのリスト
/// </summary>
[Serializable]
public class ItemListJSON
{
    public List<ItemJSON> item_list;

    public ItemListJSON()
    {
        item_list = new List<ItemJSON>();
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}

/// <summary>
/// アイテム
/// </summary>
[Serializable]
public class ItemJSON
{
    //public bool have;
    public int possession_num;
    public string id;
    public string rarity;
    public string name1;
    public string name2;
    public int image_num;
    public int rating;
    public string database_name;
    public string group_name;

    public ItemJSON()
    {

    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}

/// <summary>
/// 分類したアイテムのリスト
/// </summary>
/// 

[Serializable]
public class AllChannelListPDB
{
    public List<ChannelGroupPDB> channel_list;

    private Dictionary<string, ChannelGroupPDB> dic;

    public void setItem(PrismdbBindingsJSON item)
    {
        var cg = getChannelGroup(item.series_name.value);

        //暫定的にname1で代用
        //string group_name = item.name1;

        var outfit_id = item.outfit_id.value;

        cg.GetRarityGroupStr(item.rarity.value).setItem(item, outfit_id);
    }

    public ChannelGroupPDB getChannelGroup(string series_name)
    {
        //なければ追加
        if (!dic.ContainsKey(series_name))
        {
            ChannelGroupPDB cg = new ChannelGroupPDB();
            cg.series_name = series_name;
            channel_list.Add(cg);
            dic.Add(series_name, cg);

            //Debug.Log("ADD:" + db_name);
        }

        return dic[series_name];
    }

    public AllChannelListPDB()
    {
        channel_list = new List<ChannelGroupPDB>();
        dic = new Dictionary<string, ChannelGroupPDB>();
    }
}

public enum ItemRarity
{
    N,
    R,
    SR,
    PR,
    KR,
    JR,
    HR,
    RR,
    IR,
    ER,
    FR,
    WR
}

/// <summary>
/// チャンネルの分類
/// </summary>
[Serializable]
public class ChannelGroupPDB
{
    public string series_name;

    public Dictionary<ItemRarity, RarityGroupPDB> RarityGroups = new Dictionary<ItemRarity, RarityGroupPDB>();

    public ChannelGroupPDB()
    {
        //各レアリティのDictionaryを生成する
        foreach (ItemRarity Value in Enum.GetValues(typeof(ItemRarity)))
        {
            RarityGroups.Add(Value, new RarityGroupPDB());
        }
    }

    public RarityGroupPDB getRarityGroup(ItemRarity itemRarity)
    {
        return RarityGroups[itemRarity];
    }

    public RarityGroupPDB GetRarityGroupStr(string ir_str)
    {
        ItemRarity itemRarity;
        Enum.TryParse(ir_str, out itemRarity);

        return RarityGroups[itemRarity];
    }
}

/// <summary>
/// レアリティの分類
/// </summary>
[Serializable]
public class RarityGroupPDB
{
    public List<ItemGroupPDB> group_list;

    private Dictionary<string, ItemGroupPDB> group_dic;

    public void setItem(PrismdbBindingsJSON item, string group_name)
    {
        //Debug.Log(group_name);

        //なければ追加
        if (!group_dic.ContainsKey(group_name))
        {
            ItemGroupPDB ig = new ItemGroupPDB();
            ig.group_name = group_name;
            ig.item_group.Add(item);

            group_list.Add(ig);
            group_dic.Add(group_name, ig);

            //Debug.Log(group_name + " : " + item.name);

        }
        else
        {
            group_dic[group_name].item_group.Add(item);
            //Debug.Log(group_name + " : " + item.name);
        }
    }

    public RarityGroupPDB()
    {
        group_list = new List<ItemGroupPDB>();
        group_dic = new Dictionary<string, ItemGroupPDB>();
    }
}

/// <summary>
/// アイテムの分類
/// </summary>
[Serializable]
public class ItemGroup
{
    public string group_name;
    public List<ItemJSON> item_group;

    public ItemGroup()
    {
        item_group = new List<ItemJSON>();
    }
}

[Serializable]
public class ItemGroupPDB
{
    public string group_name;
    public List<PrismdbBindingsJSON> item_group;

    public ItemGroupPDB()
    {
        item_group = new List<PrismdbBindingsJSON>();
    }
}
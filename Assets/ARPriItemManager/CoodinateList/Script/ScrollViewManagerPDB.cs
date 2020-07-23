using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewManagerPDB : SingletonMonoBehaviour<ScrollViewManagerPDB>
{

    [SerializeField]
    GameObject ScrollViewContent = null;

    [SerializeField]
    ScrollRect scrollRect = null;

    /// <summary>
    /// Node用のPrefab
    /// </summary>
    [SerializeField]
    GameObject NodePrefab = null;

    /// <summary>
    /// コンテンツ用のPrefab
    /// </summary>
    [SerializeField]
    GameObject NodeContentPrefab = null;

    [SerializeField]
    Toggle[] rarityToggles = null;

    [SerializeField]
    GameObject menu_canvas = null;

    [SerializeField]
    Dropdown series_dd = null;

    [SerializeField]
    Dropdown volume_dd = null;

    /// <summary>
    /// 整列したアイテムリスト
    /// </summary>
    [SerializeField]
    AllChannelListPDB all_channel = new AllChannelListPDB();

    /// <summary>
    /// 現在のチャンネルグループ
    /// </summary>
    [SerializeField]
    private ChannelGroupPDB current_channel = null;

    /// <summary>
    /// チャンネル名のリスト
    /// </summary>
    private List<string> channel_name_list = new List<string>();

    // Use this for initialization
    IEnumerator Start()
    {

        // #region JSONの読み込み
        //JSONの読み込みを待つ
        while (!PrismDBItemListGetter.Instance.isInit())
            yield return null;

        Debug.Log("PrismDBのJSONの読み込みが終了");

        var volume_names = PrismDBItemListGetter.Instance.GetVolumeNames();


        volume_dd.options.Clear();

        volume_dd.options.Add(new Dropdown.OptionData { text = "現在排出されるアイテム" });

        foreach (string volume_name in volume_names)
        {
            //Debug.Log(volume_name);

            volume_dd.options.Add(new Dropdown.OptionData { text = volume_name });
        }

        volume_dd.RefreshShownValue();

        //スタートは現在排出されるコーデ
        PrismdbJSON pdbj = PrismDBItemListGetter.Instance.GetLatestItems();
        initScrollView(pdbj);

        //先頭にチャンネルをあわせる
        //string first_channel = all_channel.channel_list[0].series_name;
        //channelChangeEvent(first_channel);
    }

    /// <summary>
    /// スクロールViewを初期化する
    /// </summary>
    public void initScrollView(PrismdbJSON pdbj)
    {
        //PrismdbJSON pdbj = PrismDBItemListGetter.Instance.GetLatestItems();

        #region チャンネルドロップダウンの自動生成

        channel_name_list.Clear();

        //チャンネル
        foreach (PrismdbBindingsJSON bindings in pdbj.results.bindings)
        {
            if (!channel_name_list.Contains(bindings.series_name.value))
                channel_name_list.Add(bindings.series_name.value);
        }

        series_dd.options.Clear();

        foreach (string channel_name in channel_name_list)
        {
            Debug.Log(channel_name);

            series_dd.options.Add(new Dropdown.OptionData { text = channel_name });
        }

        series_dd.RefreshShownValue();
        series_dd.value = 0;


        #endregion

        #region アイテムリストの初期化

        initList(pdbj);

        #endregion

        //先頭にチャンネルをあわせる
        string first_channel = all_channel.channel_list[0].series_name;
        channelChangeEvent(first_channel);
    }

    /// <summary>
    /// アイテムリストの初期化
    /// </summary>
    private void initList(PrismdbJSON pdbj)
    {

        all_channel = new AllChannelListPDB();

        foreach (PrismdbBindingsJSON item in pdbj.results.bindings)
        {
            all_channel.setItem(item);
        }
    }

    /// <summary>
    /// チャンネルトグルのイベント
    /// </summary>
    /// <param name="channel"></param>
    public void channelChangeEvent(string channel)
    {
        current_channel = all_channel.getChannelGroup(channel);

        bool first_toggle_on = true;

        //レアリティが高いものから表示
        setToggleStatus(ref rarityToggles[0], ref current_channel, ItemRarity.KR, ref first_toggle_on);
        setToggleStatus(ref rarityToggles[1], ref current_channel, ItemRarity.JR, ref first_toggle_on);
        setToggleStatus(ref rarityToggles[2], ref current_channel, ItemRarity.RR, ref first_toggle_on);
        setToggleStatus(ref rarityToggles[3], ref current_channel, ItemRarity.HR, ref first_toggle_on);
        setToggleStatus(ref rarityToggles[4], ref current_channel, ItemRarity.IR, ref first_toggle_on);
        setToggleStatus(ref rarityToggles[5], ref current_channel, ItemRarity.WR, ref first_toggle_on);
        setToggleStatus(ref rarityToggles[6], ref current_channel, ItemRarity.ER, ref first_toggle_on);
        setToggleStatus(ref rarityToggles[7], ref current_channel, ItemRarity.FR, ref first_toggle_on);
        setToggleStatus(ref rarityToggles[8], ref current_channel, ItemRarity.PR, ref first_toggle_on);
        setToggleStatus(ref rarityToggles[9], ref current_channel, ItemRarity.SR, ref first_toggle_on);
        setToggleStatus(ref rarityToggles[10], ref current_channel, ItemRarity.R, ref first_toggle_on);
        setToggleStatus(ref rarityToggles[11], ref current_channel, ItemRarity.N, ref first_toggle_on);
    }

    private void setToggleStatus(ref Toggle toggle, ref ChannelGroupPDB channel_group/*RarityGroupPDB rarity_group*/, /*string r_name*/ItemRarity rarity, ref bool first_toggle_on)
    {
        Debug.Log(channel_group.series_name);

        RarityGroupPDB rarity_group = channel_group.getRarityGroup(rarity);

        if (rarity_group.group_list.Count != 0)
        {
            toggle.gameObject.SetActive(true);
            //Debug.Log(rarity + ":true");

            if (first_toggle_on)
            {
                if (toggle.isOn)
                    rarityToggleEvent(rarity/*r_name*/);
                else
                    toggle.isOn = true;

                first_toggle_on = false;
            }
        }
        else
        {
            Debug.Log(rarity + " count is 0");
            toggle.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// レアリティトグルのイベント
    /// </summary>
    /// <param name="rarity"></param>
    public void rarityToggleEvent(/*string rarity*/ ItemRarity rarity)
    {
        //Debug.Log(rarity);

        RarityGroupPDB rarity_group = current_channel.getRarityGroup(rarity);

        updateScrollView(rarity_group);

    }

    /// <summary>
    /// レアリティでScrollViewを更新する
    /// </summary>
    /// <param name="rarity"></param>
    public void updateScrollView(RarityGroupPDB rarity)
    {
        //消さないで行きたいね
        //clearScrollView();

        int node_num = ScrollViewContent.transform.childCount;//ScrollViewの子のnode数
        int group_num = rarity.group_list.Count;//更新したいグループ数

        //0の時はすべて非アクティブ
        if (group_num == 0)
        {
            for (int i = 0; i < node_num; i++)
            {
                ScrollViewContent.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }

            return;
        }

        //nodeが多い場合は非アクティブにする
        if (node_num > group_num)
        {
            for (int i = group_num - 1; i < node_num; i++)
                ScrollViewContent.transform.GetChild(i).gameObject.SetActive(false);
        }



        for (int i = 0; i < rarity.group_list.Count; i++)
        {

            if (i < node_num)
            {
                //数が足りていれば取得
                var node = ScrollViewContent.transform.GetChild(i).gameObject;

                //アクティブ化
                node.SetActive(true);

                //ここで数値を更新すればいい
                node.name = rarity.group_list[i].group_name;
                node.GetComponent<NodeManager>().setOutfitName(rarity.group_list[i].group_name);

                //ノードをアップデート
                updateNode(node, rarity.group_list[i].item_group);
            }
            else
            {
                //数が足りなければ追加
                var node = createNode(rarity.group_list[i].item_group);
                node.name = rarity.group_list[i].group_name;
                node.GetComponent<NodeManager>().setOutfitName(rarity.group_list[i].group_name);

                //Debug.Log(node.name);

                addNodeToScrollView(node);
                node.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1f);
            }

        }

        scrollRect.verticalNormalizedPosition = 1;
    }

    #region ScrollViewの表示管理

    /// <summary>
    /// Nodeを生成する
    /// </summary>
    private GameObject createNode(List<PrismdbBindingsJSON> item_list)
    {
        //ノードを生成
        GameObject new_node = Instantiate(NodePrefab);
        NodeManager nm = new_node.GetComponent<NodeManager>();

        //ノード内に同じグループのアイテムを追加
        foreach (PrismdbBindingsJSON item in item_list)
        {
            var new_content = createContent(item);
            nm.addContentToNode(new_content);
        }

        return new_node;
    }

    /// <summary>
    /// Nodeを更新する
    /// </summary>
    private void updateNode(GameObject node, List<PrismdbBindingsJSON> item_list)
    {
        var nm = node.GetComponent<NodeManager>();
        int content_num = nm.getContentNum();
        int item_num = item_list.Count;

        //contentが多い場合は非アクティブにする
        if (content_num > item_num)
        {
            //コンテンツ数の差を計算
            int sub_content_num = content_num - item_num;

            for (int i = 0; i < sub_content_num; i++)
            {
                nm.deactivateContents(content_num - 1 - i);
            }

        }

        for (int i = 0; i < item_list.Count; i++)
        {
            if (i < content_num)
            {
                nm.activeContents(i, item_list);
            }
            else
            {
                //数が足りなければ追加
                var new_content = createContent(item_list[i]);

                nm.addContentToNode(new_content);
            }

        }

        //高さを調整
        //setNodeHeight(node);
    }

    /// <summary>
    /// コンテンツを生成する
    /// </summary>
    /// <returns></returns>
    private GameObject createContent(PrismdbBindingsJSON item)
    {
        GameObject new_content = Instantiate(NodeContentPrefab);
        new_content.name = item.name.value;

        //Debug.Log("create : " + new_content.name);

        ContentManager cm = new_content.GetComponent<ContentManager>();
        cm.setItem(item);

        return new_content;
    }

    /// <summary>
    /// ScrollViewにNodeを追加する
    /// </summary>
    /// <param name="node"></param>
    private void addNodeToScrollView(GameObject node)
    {
        node.transform.SetParent(ScrollViewContent.transform);
    }

    /// <summary>
    /// Nodeにコンテンツを追加する
    /// </summary>
    /// <param name="node"></param>
    private void addContentToNode(GameObject node, GameObject content)
    {
        content.transform.SetParent(node.transform);

        content.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1f);

    }

    #endregion

    /// <summary>
    /// 一覧メニューの表示を切り替える
    /// </summary>
    public void changeDisplayCanvas()
    {
        menu_canvas.SetActive(!menu_canvas.activeSelf);
    }

    /// <summary>
    /// 子要素を削除する
    /// </summary>
    /// <param name="obj"></param>
    private void deleteChildren(GameObject obj)
    {
        foreach (Transform n in obj.transform)
        {
            GameObject.Destroy(n.gameObject);
        }
    }

    //Ouput the new value of the Dropdown into Text
    public void ChannelDropdownValueChanged(Dropdown change)
    {
        var series_name = change.options[change.value].text;
        Debug.Log(change.value + " : " + series_name);
        channelChangeEvent(series_name);
    }

    //Ouput the new value of the Dropdown into Text
    public void VolumeDropdownValueChanged(Dropdown change)
    {
        var volume_name = change.options[change.value].text;
        Debug.Log(change.value + " : " + volume_name);

        if (volume_name == "現在排出されるアイテム")
        {
            PrismdbJSON pdbj = PrismDBItemListGetter.Instance.GetLatestItems();
            initScrollView(pdbj);
        }
        else
        {
            PrismdbJSON pdbj = PrismDBItemListGetter.Instance.GetVolumeItems(volume_name);
            initScrollView(pdbj);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class PrismDBItemListGetter : SingletonMonoBehaviour<PrismDBItemListGetter>
{
    [SerializeField]
    private PrismdbJSON latest_items = null;

    [SerializeField]
    private PrismdbJSON all_items = null;

    /// <summary>
    /// 最新アイテムリスト初期化フラグ
    /// </summary>
    [SerializeField]
    private bool latest_init_flag = false;

    /// <summary>
    /// 全アイテムリスト初期化フラグ
    /// </summary>
    [SerializeField]
    private bool all_init_flag = false;

    //各シリーズごとのアイテムリスト
    private Dictionary<string, PrismdbJSON> volume_items_dictionary = new Dictionary<string, PrismdbJSON>();


    /// <summary>
    /// 画像番号からアイテムを取得するDictionary
    /// </summary>
    private Dictionary<int, PrismdbBindingsJSON> itemDictionaryImageNum = new Dictionary<int, PrismdbBindingsJSON>();

    /// <summary>
    /// アイテムリストの初期化フラグ
    /// </summary>
    /// <returns></returns>
    public bool isInit()
    {
        return latest_init_flag && all_init_flag;
    }

    /// <summary>
    /// アイテムデータを返す
    /// </summary>
    /// <param name="image_num">画像番号</param>
    /// <returns></returns>
    public PrismdbBindingsJSON getItem(int image_num)
    {
        if (itemDictionaryImageNum.ContainsKey(image_num))
            return itemDictionaryImageNum[image_num];
        else
            return null;
    }

    /// <summary>
    /// 最新アイテムリストを返す
    /// </summary>
    /// <returns></returns>
    public PrismdbJSON GetLatestItems()
    {
        return latest_items;
    }

    /// <summary>
    /// 存在するVolumeを返す
    /// </summary>
    /// <returns></returns>
    public List<string> GetVolumeNames()
    {
        List<string> keysList = new List<string>(volume_items_dictionary.Keys);
        return keysList;
    }

    /// <summary>
    /// 指定したvolumeのアイテムリストを返す
    /// </summary>
    /// <param name="volume"></param>
    /// <returns></returns>
    public PrismdbJSON GetVolumeItems(string volume)
    {
        if (volume_items_dictionary.ContainsKey(volume))
            return volume_items_dictionary[volume];
        else
            return null;
    }


    void Start()
    {
        //現在排出されるコーデ一覧を取ってくる
        StartCoroutine(GetLatestItemsFromPDB());

        //すべてのコーデ一覧を取ってくる
        StartCoroutine(GetAllItemsFromPDB());
    }





    /// <summary>
    /// PrismDBから最新アイテムを取得する
    /// </summary>
    /// <returns></returns>
    IEnumerator GetLatestItemsFromPDB()
    {
        //const string latest_url = "https://prismdb.takanakahiko.me/sparql/?default-graph-uri=&query=DEFINE+sql%3Aselect-option+%22order%22%0D%0APREFIX+prism%3A+%3Chttps%3A%2F%2Fprismdb.takanakahiko.me%2Fprism-schema.ttl%23%3E%0D%0APREFIX+xsd%3A+%3Chttp%3A%2F%2Fwww.w3.org%2F2001%2FXMLSchema%23%3E%0D%0ASELECT+%3Fid+%3Frarity+%3Fname+%3Fcategory+%3Fimage_num+%3Fcolor+%3Ftype+%3Flike+%3Foutfit_id+%3Fvolume+%3Fseries_name+%3Fstart+%3Fend%0D%0AWHERE+%7B%0D%0A%3Fitem+a+prism%3AItem%3B%0D%0Aprism%3Aterm_start+%3Fstart%3B%0D%0Aprism%3Aterm_end+%3Fend.%0D%0AOPTIONAL%7B+%3Fitem+prism%3Aimage_num+%3Fimage_num.%7D%0D%0AOPTIONAL%7B+%3Fitem+prism%3Avolume+%3Fvolume.%7D%0D%0AFILTER%28xsd%3AdateTime%28%3Fstart%29+%3C%3D+NOW%28%29+%26%26+%28xsd%3AdateTime%28%3Fend%29+%2B+xsd%3Aduration%28%22P1D%22%29%29+%3E%3D+NOW%28%29%29%0D%0A%3Fitem+prism%3Aitem_id+%3Fid%3B%0D%0Aprism%3Ararity+%3Frarity%3B%0D%0Aprism%3Acategory+%3Fcategory%3B%0D%0Aprism%3Aname+%3Fname%3B%0D%0Aprism%3Acolor+%3Fcolor%3B%0D%0Aprism%3Atype+%3Ftype%3B%0D%0Aprism%3Alike+%3Flike%3B%0D%0Aprism%3Aoutfit_id+%3Foutfit_id%3B%0D%0Aprism%3Aseries_name+%3Fseries_name.%0D%0A%7D&format=application%2Fsparql-results%2Bjson&timeout=0&debug=on";

        const string latest_url = "https://prismdb.takanakahiko.me/sparql/?default-graph-uri=&query=DEFINE+sql%3Aselect-option+%22order%22%0D%0APREFIX+prism%3A+%3Chttps%3A%2F%2Fprismdb.takanakahiko.me%2Fprism-schema.ttl%23%3E%0D%0APREFIX+xsd%3A+%3Chttp%3A%2F%2Fwww.w3.org%2F2001%2FXMLSchema%23%3E%0D%0ASELECT+%3Fid+%3Frarity+%3Fname+%3Fcategory+%3Fimage_num+%3Fcolor+%3Flike+%3Foutfit_id+%3Fvolume+%3Fseries_name+%3Fstart+%3Fend%0D%0AWHERE+%7B%0D%0A%3Fitem+a+prism%3AItem%3B%0D%0Aprism%3Aterm_start+%3Fstart%3B%0D%0Aprism%3Aterm_end+%3Fend.%0D%0AOPTIONAL%7B+%3Fitem+prism%3Aimage_num+%3Fimage_num.%7D%0D%0AOPTIONAL%7B+%3Fitem+prism%3Avolume+%3Fvolume.%7D%0D%0AFILTER%28xsd%3AdateTime%28%3Fstart%29+%3C%3D+NOW%28%29+%26%26+%28xsd%3AdateTime%28%3Fend%29+%2B+xsd%3Aduration%28%22P1D%22%29%29+%3E%3D+NOW%28%29%29%0D%0A%3Fitem+prism%3Aitem_id+%3Fid%3B%0D%0Aprism%3Ararity+%3Frarity%3B%0D%0Aprism%3Acategory+%3Fcategory%3B%0D%0Aprism%3Aname+%3Fname%3B%0D%0Aprism%3Acolor+%3Fcolor%3B%0D%0Aprism%3Alike+%3Flike%3B%0D%0Aprism%3Aoutfit_id+%3Foutfit_id%3B%0D%0Aprism%3Aseries_name+%3Fseries_name.%0D%0A%7D&format=application%2Fsparql-results%2Bjson&timeout=0&debug=on";

        using (UnityWebRequest www = UnityWebRequest.Get(latest_url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);

                loadLatestItemsJSONFromLocal();
            }
            else
            {
                string json = Regex.Unescape(www.downloadHandler.text);

                deserializeLatestItemsJSON(json);

                saveLatestItemsJSONToLocal();
            }
        }

    }

    /// <summary>
    /// 最新アイテムリストをデシリアライズする
    /// </summary>
    /// <param name="json"></param>
    private void deserializeLatestItemsJSON(string json)
    {
        latest_items = JsonUtility.FromJson<PrismdbJSON>(json);

        latest_init_flag = true;
    }

    /// <summary>
    /// PrismDBから期間指定のあるアイテムをすべて取得する
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetAllItemsFromPDB()
    {
        //const string all_url = "https://prismdb.takanakahiko.me/sparql/?default-graph-uri=&query=DEFINE+sql%3Aselect-option+%22order%22%0D%0APREFIX+prism%3A+%3Chttps%3A%2F%2Fprismdb.takanakahiko.me%2Fprism-schema.ttl%23%3E%0D%0APREFIX+xsd%3A+%3Chttp%3A%2F%2Fwww.w3.org%2F2001%2FXMLSchema%23%3E%0D%0ASELECT+%3Fid+%3Frarity+%3Fname+%3Fcategory+%3Fimage_num+%3Fcolor+%3Fbrand+%3Ftype+%3Flike+%3Foutfit_id+%3Fvolume+%3Fseries_name+%3Fstart+%3Fend%0D%0AWHERE+%7B%0D%0A%3Fitem+a+prism%3AItem%3B%0D%0Aprism%3Aterm_start+%3Fstart%3B%0D%0Aprism%3Aterm_end+%3Fend.%0D%0AOPTIONAL%7B+%3Fitem+prism%3Aimage_num+%3Fimage_num.%7D%0D%0AOPTIONAL%7B+%3Fitem+prism%3Avolume+%3Fvolume.%7D%0D%0A%0D%0A%3Fitem+prism%3Aitem_id+%3Fid%3B%0D%0Aprism%3Ararity+%3Frarity%3B%0D%0Aprism%3Acategory+%3Fcategory%3B%0D%0Aprism%3Aname+%3Fname%3B%0D%0Aprism%3Acolor+%3Fcolor%3B%0D%0Aprism%3Abrand+%3Fbrand%3B%0D%0Aprism%3Atype+%3Ftype%3B%0D%0Aprism%3Alike+%3Flike%3B%0D%0Aprism%3Aoutfit_id+%3Foutfit_id%3B%0D%0Aprism%3Aseries_name+%3Fseries_name.%0D%0A%7D&format=application%2Fsparql-results%2Bjson&timeout=0&debug=on";

        const string all_url = "https://prismdb.takanakahiko.me/sparql/?default-graph-uri=&query=DEFINE+sql%3Aselect-option+%22order%22%0D%0APREFIX+prism%3A+%3Chttps%3A%2F%2Fprismdb.takanakahiko.me%2Fprism-schema.ttl%23%3E%0D%0APREFIX+xsd%3A+%3Chttp%3A%2F%2Fwww.w3.org%2F2001%2FXMLSchema%23%3E%0D%0ASELECT+%3Fid+%3Frarity+%3Fname+%3Fcategory+%3Fimage_num+%3Fcolor+%3Ftype+%3Flike+%3Foutfit_id+%3Fvolume+%3Fseries_name+%3Fstart+%3Fend%0D%0AWHERE+%7B%0D%0A%3Fitem+a+prism%3AItem%3B%0D%0Aprism%3Aterm_start+%3Fstart%3B%0D%0Aprism%3Aterm_end+%3Fend.%0D%0AOPTIONAL%7B+%3Fitem+prism%3Aimage_num+%3Fimage_num.%7D%0D%0AOPTIONAL%7B+%3Fitem+prism%3Avolume+%3Fvolume.%7D%0D%0A%0D%0A%3Fitem+prism%3Aitem_id+%3Fid%3B%0D%0Aprism%3Ararity+%3Frarity%3B%0D%0Aprism%3Acategory+%3Fcategory%3B%0D%0Aprism%3Aname+%3Fname%3B%0D%0Aprism%3Acolor+%3Fcolor%3B%0D%0Aprism%3Atype+%3Ftype%3B%0D%0Aprism%3Alike+%3Flike%3B%0D%0Aprism%3Aoutfit_id+%3Foutfit_id%3B%0D%0Aprism%3Aseries_name+%3Fseries_name.%0D%0A%7D&format=application%2Fsparql-results%2Bjson&timeout=0&debug=on";

        using (UnityWebRequest www = UnityWebRequest.Get(all_url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);

                loadAllItemsJSONFromLocal();
            }
            else
            {
                string json = Regex.Unescape(www.downloadHandler.text);

                deserializeAllItemsJSON(json);

                //ローカルにも保存しておく
                saveAllItemsJSONToLocal();
            }
        }

    }

    /// <summary>
    /// 全アイテムリストをデシリアライズする
    /// </summary>
    /// <param name="json"></param>
    private void deserializeAllItemsJSON(string json)
    {
        all_items = JsonUtility.FromJson<PrismdbJSON>(json);

        itemDictionaryImageNum.Clear();

        volume_items_dictionary.Clear();

        foreach (PrismdbBindingsJSON binding in all_items.results.bindings)
        {
            try
            {
                //itemDictonaryImageNumを生成
                var image_num = int.Parse(binding.image_num.value);

                if (!itemDictionaryImageNum.ContainsKey(image_num))
                {
                    itemDictionaryImageNum.Add(image_num, binding);
                }
                else
                {
                    itemDictionaryImageNum[image_num] = binding;
                }

                //volume_items_dictonaryを生成
                var volume = binding.volume.value;

                if (!volume_items_dictionary.ContainsKey(volume))
                {
                    volume_items_dictionary.Add(volume, new PrismdbJSON());
                }

                volume_items_dictionary[volume].results.bindings.Add(binding);

            }
            catch
            {
                ; //Debug.Log("int.Parse Error");
            }
        }

        //値を確認
        /*
        foreach (var elem in volume_items_dictonary)
        {
            Debug.Log(elem.Key + " , " + elem.Value.results.bindings.Count);
        }
        */

        all_init_flag = true;
    }







    /// <summary>
    /// 最新アイテムリストをローカルに保存する
    /// </summary>
    private void saveLatestItemsJSONToLocal()
    {
        string folder_path = Application.persistentDataPath + "/Data";
        string path = folder_path + "/LatestItems.json";

        //pathがなければ作成
        DirectoryUtils.SafeCreateDirectory(folder_path);

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                string json = latest_items.ToString();

                sw.Write(json);
                sw.Flush();
            }
        }
    }

    /// <summary>
    /// 最新アイテムリストをローカルから読み込む
    /// </summary>
    private void loadLatestItemsJSONFromLocal()
    {
        string path = Application.persistentDataPath + "/Data/LatestItems.json";

        if (File.Exists(path))
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string json = sr.ReadToEnd();
                    deserializeLatestItemsJSON(json);
                }
            }
        }
    }

    /// <summary>
    /// 全アイテムリストをローカルに保存する
    /// </summary>
    private void saveAllItemsJSONToLocal()
    {
        string folder_path = Application.persistentDataPath + "/Data";
        string path = folder_path + "/AllItems.json";

        //pathがなければ作成
        DirectoryUtils.SafeCreateDirectory(folder_path);

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                string json = all_items.ToString();

                sw.Write(json);
                sw.Flush();
            }
        }
    }

    /// <summary>
    /// 全アイテムリストをローカルから読み込む
    /// </summary>
    private void loadAllItemsJSONFromLocal()
    {
        string path = Application.persistentDataPath + "/Data/AllItems.json";

        if (File.Exists(path))
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string json = sr.ReadToEnd();
                    deserializeAllItemsJSON(json);
                }
            }
        }
    }
}

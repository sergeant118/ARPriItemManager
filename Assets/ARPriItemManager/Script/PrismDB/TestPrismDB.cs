using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class TestPrismDB : SingletonMonoBehaviour<TestPrismDB>
{

    //const string sparql_head = "https://prismdb.takanakahiko.me/sparql?default-graph-uri=&query=";

    //const string sparql_end = "&format=application%2Fsparql-results%2Bjson";

    
    [SerializeField]
    PrismdbJSON pj;

    [SerializeField]
    PrismdbJSON latest_items;

    [SerializeField]
    PrismdbJSON all_items;

    [SerializeField]
    private bool latest_init_flag = false;

    [SerializeField]
    private bool all_init_flag = false;

    /// <summary>
    /// IDからアイテムを取得するDictionary
    /// </summary>
    //private Dictionary<string, PrismdbBindingsJSON> itemDictonaryID = new Dictionary<string, PrismdbBindingsJSON>();

    /// <summary>
    /// 画像番号からアイテムを取得するDictionary
    /// </summary>
    private Dictionary<int, PrismdbBindingsJSON> itemDictonaryImageNum = new Dictionary<int, PrismdbBindingsJSON>();

    /// <summary>
    /// 初期化しているかどうか
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
        if(itemDictonaryImageNum.ContainsKey(image_num))
            return itemDictonaryImageNum[image_num];
        else
            return null;
    }

    /*
    string sparql_create(List<string> series_name_list)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("DEFINE sql:select-option \"order\" ");
        sb.Append("PREFIX prism: <https://prismdb.takanakahiko.me/prism-schema.ttl#>");
        sb.Append("SELECT ?id ?rarity ?category ?name ?image_num ?color ?brand ?type ?like ?outfit_id ?volume ?series_name ?collection_term ?term_start ?term_end");
        sb.Append(" WHERE{");

        sb.Append("?s prism:series_name ?o.");

        if (series_name_list.Count > 0)
        {
            sb.Append("?s prism:series_name ?o.");
            sb.Append("FILTER(?o = '");

            for(int i = 0; i < series_name_list.Count; i++) {

                sb.Append(series_name_list[i]);

                if(i != series_name_list.Count - 1)
                    sb.Append("' || ?o = '");
            }

            sb.Append("')");
        }

        sb.Append("?s prism:item_id ?id;");
        sb.Append("prism:rarity ?rarity;");
        sb.Append("prism:category ?category;");
        sb.Append("prism:name ?name;");
        sb.Append("prism:image_num ?image_num;");
        sb.Append("prism:color ?color;");
        sb.Append("prism:brand ?brand;");
        sb.Append("prism:type ?type;");
        sb.Append("prism:like ?like;");
        sb.Append("prism:outfit_id ?outfit_id;");
        sb.Append("prism:volume ?volume;");
        sb.Append("prism:series_name ?series_name;");
        sb.Append("prism:term_start ?term_start;");
        sb.Append("prism:term_end ?term_end.");
        sb.Append("}");

        return sb.ToString();
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        //現在排出されるコーデ一覧を取ってくる
        StartCoroutine(GetLatestItemsFromPDB());

        //すべてのコーデ一覧を取ってくる
        StartCoroutine(GetAllItemsFromPDB());


        /*
        List<string> series_name_list = new List<string>();
        series_name_list.Add("リングマリィコレクション");
        series_name_list.Add("すずコレクション リングマリィコレクション共通");


        string sparql = sparql_create(series_name_list);
        string sparql_enc = System.Web.HttpUtility.UrlEncode(sparql);

        string sparql_url = sparql_head + sparql_enc + sparql_end;

        Debug.Log(sparql_url);

        string url = System.Web.HttpUtility.UrlDecode(sparql_url);

        Debug.Log(url);

        StartCoroutine(GetText(sparql_url));
        */
    }



    IEnumerator GetText(string url)
    {

        Debug.Log("GET TEXT!");


        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                string json = Regex.Unescape(www.downloadHandler.text);

                pj = JsonUtility.FromJson<PrismdbJSON>(json);

                Debug.Log(pj);
            }
        }

        Debug.Log("GET TEXT END");
    }

    /// <summary>
    /// PrismDBから最新アイテムを取得する
    /// </summary>
    /// <returns></returns>
    IEnumerator GetLatestItemsFromPDB()
    {

        Debug.Log("GetLatestItems!");

        const string latest_url = "https://prismdb.takanakahiko.me/sparql/?default-graph-uri=&query=DEFINE+sql%3Aselect-option+%22order%22%0D%0APREFIX+prism%3A+%3Chttps%3A%2F%2Fprismdb.takanakahiko.me%2Fprism-schema.ttl%23%3E%0D%0APREFIX+xsd%3A+++%3Chttp%3A%2F%2Fwww.w3.org%2F2001%2FXMLSchema%23%3E%0D%0ASELECT+%3Fid+%3Frarity+%3Fname+%3Fcategory+%3Fimage_num+%3Fcolor+%3Fbrand+%3Ftype+%3Flike+%3Foutfit_id+%3Fvolume+%3Fseries_name+%3Fstart+%3Fend%0D%0AWHERE+%7B%0D%0A%3Fitem+a+prism%3AItem%3B%0D%0Aprism%3Aterm_start+%3Fstart%3B%0D%0Aprism%3Aterm_end+%3Fend.%0D%0AOPTIONAL%7B%0D%0A%3Fitem+prism%3Aimage_num+%3Fimage_num%3B%0D%0A++++++prism%3Avolume+%3Fvolume.%0D%0A%7D%0D%0AFILTER%28xsd%3AdateTime%28%3Fstart%29+%3C%3D+NOW%28%29+%26%26%0D%0A++++++xsd%3AdateTime%28%3Fend%29+%3E%3D+NOW%28%29%29%0D%0A%3Fitem+prism%3Aitem_id+%3Fid%3B%0D%0Aprism%3Ararity+%3Frarity%3B%0D%0Aprism%3Acategory+%3Fcategory%3B%0D%0Aprism%3Aname+%3Fname%3B%0D%0Aprism%3Acolor+%3Fcolor%3B%0D%0Aprism%3Abrand+%3Fbrand%3B%0D%0Aprism%3Atype+%3Ftype%3B%0D%0Aprism%3Alike+%3Flike%3B%0D%0Aprism%3Aoutfit_id+%3Foutfit_id%3B%0D%0Aprism%3Aseries_name+%3Fseries_name.%0D%0A%7D&format=application%2Fsparql-results%2Bjson&timeout=0&debug=on";


        using (UnityWebRequest www = UnityWebRequest.Get(latest_url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                string json = Regex.Unescape(www.downloadHandler.text);

                latest_items = JsonUtility.FromJson<PrismdbJSON>(json);

                latest_init_flag = true;

                Debug.Log(latest_items);
            }
        }

        Debug.Log("GetLatestItems END");
    }

    /// <summary>
    /// 最新アイテムのリストを返す
    /// </summary>
    /// <returns></returns>
    public PrismdbJSON GetLatestItems()
    {
        return latest_items;
    }

    /// <summary>
    /// PrismDBから期間指定のあるアイテムをすべて取得する
    /// </summary>
    /// <returns></returns>
    IEnumerator GetAllItemsFromPDB()
    {
        Debug.Log("GetALLItems!");

        const string all_url = "https://prismdb.takanakahiko.me/sparql/?default-graph-uri=&query=DEFINE+sql%3Aselect-option+%22order%22%0D%0APREFIX+prism%3A+%3Chttps%3A%2F%2Fprismdb.takanakahiko.me%2Fprism-schema.ttl%23%3E%0D%0APREFIX+xsd%3A+%3Chttp%3A%2F%2Fwww.w3.org%2F2001%2FXMLSchema%23%3E%0D%0ASELECT+%3Fid+%3Frarity+%3Fname+%3Fcategory+%3Fimage_num+%3Fcolor+%3Fbrand+%3Ftype+%3Flike+%3Foutfit_id+%3Fvolume+%3Fseries_name+%3Fstart+%3Fend%0D%0AWHERE+%7B%0D%0A%3Fitem+a+prism%3AItem%3B%0D%0Aprism%3Aterm_start+%3Fstart%3B%0D%0Aprism%3Aterm_end+%3Fend.%0D%0AOPTIONAL%7B+%3Fitem+prism%3Aimage_num+%3Fimage_num.%7D%0D%0AOPTIONAL%7B+%3Fitem+prism%3Avolume+%3Fvolume.%7D%0D%0A%0D%0A%3Fitem+prism%3Aitem_id+%3Fid%3B%0D%0Aprism%3Ararity+%3Frarity%3B%0D%0Aprism%3Acategory+%3Fcategory%3B%0D%0Aprism%3Aname+%3Fname%3B%0D%0Aprism%3Acolor+%3Fcolor%3B%0D%0Aprism%3Abrand+%3Fbrand%3B%0D%0Aprism%3Atype+%3Ftype%3B%0D%0Aprism%3Alike+%3Flike%3B%0D%0Aprism%3Aoutfit_id+%3Foutfit_id%3B%0D%0Aprism%3Aseries_name+%3Fseries_name.%0D%0A%7D&format=application%2Fsparql-results%2Bjson&timeout=0&debug=on";
        using (UnityWebRequest www = UnityWebRequest.Get(all_url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                string json = Regex.Unescape(www.downloadHandler.text);

                all_items = JsonUtility.FromJson<PrismdbJSON>(json);

                itemDictonaryImageNum.Clear();

                foreach (PrismdbBindingsJSON binding in all_items.results.bindings)
                {
                    try
                    {
                        var image_num = int.Parse(binding.image_num.value);

                        if (!itemDictonaryImageNum.ContainsKey(image_num))
                        {
                            itemDictonaryImageNum.Add(image_num, binding);
                        }
                        else
                        {
                            itemDictonaryImageNum[image_num] = binding;
                        }
                    }
                    catch
                    {
                        ; //Debug.Log("int.Parse Error");
                    }
                }

                Debug.Log("Dictionary Count : " + itemDictonaryImageNum.Count);

                //itemDictonaryImageNum = all_items.results.bindings.ToDictionary(binding => int.Parse(binding.image_num.value));

                all_init_flag = true;

                Debug.Log(all_items);
            }
        }

        Debug.Log("GetALLItems END");
    }


}

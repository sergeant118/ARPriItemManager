using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// GASから取得した生データのリスト
/// </summary>
[Serializable]
public class PrismdbJSON
{
    public PrismdbResultsJSON results;

    public PrismdbJSON()
    {
        results = new PrismdbResultsJSON();
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}

[Serializable]
public class PrismdbResultsJSON
{
    public List<PrismdbBindingsJSON> bindings;

    public PrismdbResultsJSON()
    {
        bindings = new List<PrismdbBindingsJSON>();
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}

[Serializable]
public class PrismdbBindingsJSON
{
    public PrismdbValueJSON id;
    public PrismdbValueJSON rarity;
    public PrismdbValueJSON category;
    public PrismdbValueJSON name;
    public PrismdbValueJSON image_num;
    public PrismdbValueJSON color;
    public PrismdbValueJSON brand;
    public PrismdbValueJSON type;
    public PrismdbValueJSON like;
    public PrismdbValueJSON outfit_id;
    public PrismdbValueJSON volume;
    public PrismdbValueJSON series_name;
    public PrismdbValueJSON start;
    public PrismdbValueJSON end;


    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}

[Serializable]
public class PrismdbValueJSON
{
    public string type;
    public string value;

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}


//{ "head": { "link": [], "vars": ["id", "rarity", "category", "name", "image_num", "color", "brand", "type", "like", "outfit_id", "series_name", "collection_term"] },
// "results": { "distinct": false, "ordered": true, "bindings": [

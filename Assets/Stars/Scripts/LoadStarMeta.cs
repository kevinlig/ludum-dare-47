using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadStarMeta : MonoBehaviour
{
    public static List<string> GetStarNames() {
        TextAsset rawJson = Resources.Load<TextAsset>("stars");
        JSONStarList listParent = JsonUtility.FromJson<JSONStarList>(rawJson.text);
        return listParent.Stars.ToList();
    }
}

[System.Serializable]
class JSONStarList {
    public string[] Stars = new string[0];
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class ModificationJSONInfo
{
    public int id;
    public int type;

    public static string ToJSON(ModificationJSONInfo info)
    {
        return JsonUtility.ToJson(info);
    }

}




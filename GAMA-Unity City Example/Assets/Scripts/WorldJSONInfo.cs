using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WorldJSONInfo
{
    public List<int> building;
    public List<PeopleInfo> people;
    // public ;

    public static WorldJSONInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<WorldJSONInfo>(jsonString);
    }

}

[System.Serializable]
public class PeopleInfo
{
    public List<int> d;

}


using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int level_number;
    public int move_count;
    public List<string> grid = new List<string>();
    public int grid_width;
    public int grid_height;

    public static Level CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Level>(jsonString);
    }
}
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Achievement
{
    public string imagePath;
    public string title;
    public string description;
    public bool collected;

    public Achievement(string ip, string t, string d, bool c)
    {
        imagePath = ip;
        title = t;
        description = d;
        collected = c;
    }
}

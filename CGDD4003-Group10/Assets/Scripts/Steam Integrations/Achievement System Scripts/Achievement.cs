[System.Serializable]
public class Achievement
{
    public string imagePath;
    public string title;
    public string api_name;
    public string description;
    public bool collected;

    public Achievement(string ip, string t, string a, string d, bool c)
    {
        imagePath = ip;
        title = t;
        api_name = a;
        description = d;
        collected = c;
    }
}

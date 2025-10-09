using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Localizer : MonoBehaviour
{
    public enum TextIdentifier
    {
        None,
        UI_MainMenu_New_Game,
        UI_MainMenu_Continue,
        UI_MainMenu_Options,
        UI_MainMenu_Achievements,
        UI_MainMenu_Exit,
        UI_MainMenu_Credits,
        UI_Paused_Resume,
        UI_Paused_Main_Menu,
        UI_Paused_Options,
        UI_Paused_Quit,
        UI_Difficulty_Select_Baby,
        UI_Difficulty_Select_Normal,
        UI_Difficulty_Select_Nightmare,
        Game_Level,
        Game_Objective,
        Game_Score,
        Game_Pellets,
        Game_Stuns,
        Game_Shields,
        Game_Lives,
        Game_Weapon_Message,
        Game_Speed_Boost_Active,
        Game_Speed_Boost_Inactive,
        Game_Invisibility_Active,
        Game_Invisibility_Inactive,
        Game_Enhanced_Radar_Active,
        Game_Enhanced_Radar_Inactive,
        Game_Shield_Obtained,
        UI_Options_Resolution,
        UI_Options_Fullscreen,
        UI_Options_Language,
        UI_Options_FOV,
        UI_Options_View_Bobbing,
        UI_Options_Sensitivity,
        UI_Options_Master_Volume,
        UI_Options_Music_Volume,
        UI_Options_Weapon_Volume,
        UI_Options_Player_Volume,
        UI_Options_Enemy_Volume,
        UI_Options_Pickup_Volume,
        UI_Options_UI_Volume,
        UI_Options_Misc_Volume,
        UI_Back,
        UI_Return,
        UI_Select,
        UI_Continue,
        UI_Intro_Skip,
        UI_Yes,
        UI_No,
        UI_WeaponSelect_Range,
        UI_WeaponSelect_Damage,
        UI_WeaponSelect_Speed,
        Weapon_Railgun_Name,
        Weapon_Railgun_Description,
        Weapon_Rifle_Name,
        Weapon_Rifle_Description,
        Weapon_Shotgun_Name,
        Weapon_Shotgun_Description,
        UI_Credits_Role_Team_Leader,
        UI_Credits_Role_Lead_Programmer,
        UI_Credits_Role_Lead_UI_Programmer,
        UI_Credits_Role_Programmers,
        UI_Credits_Role_Lead_SFX_Artist,
        UI_Credits_Role_SFX_Artists,
        UI_Credits_Role_Lead_Music_Composer,
        UI_Credits_Role_Lead_Artist,
        UI_Credits_Role_2D3D_Artists,
        UI_Credits_Role_Incantations,
        UI_Credits_Role_Playtesters,
        UI_Credits_Role_Sounds,
        UI_Credits_Cover_Art,
        UI_Credits_Credits_Music,
        UI_Credits_Fonts,
        UI_Credits_Thanks,
        UI_End_Retry,
        UI_End_Save_Score,
        UI_End_Main_Menu,
        UI_End_Statistics,
        UI_End_Quit,
        UI_End_Score,
        UI_End_Stuns,
        UI_End_Shields,
        UI_End_Deaths,
        UI_End_Click_Continue,
        UI_End_Pellets,
        UI_End_Time,
        UI_HighScore_Main_Menu,
        UI_HighScore_Score,
        UI_HighScore_Enter_Initials,
        UI_HighScore_High_Score,
        Beginning_Paragraph_0,
        Beginning_Paragraph_1,
        Beginning_Paragraph_2,
        Ending_Good_Paragraph_0,
        Ending_Good_Paragraph_1,
        Ending_Good_Paragraph_2,
        Ending_Bad_Paragraph_0,
        Ending_Bad_Paragraph_1,
        Ending_Bad_Paragraph_2,
        Ending_Insanity_Paragraph_0,
        Ending_Insanity_Paragraph_1,
        Ending_Insanity_Paragraph_2,
        DemoEnd_Paragraph_0,
        DemoEnd_Paragraph_1,
        DemoEnd_Paragraph_2,
        UI_WeaponSelect_Classified,
        Game_Corrupted_Objective,
        Game_Fruit_Offering,
        Game_Corrupted_Final_Offering,
        Game_Level_Cleared,
        UI_End_Shots_Fired,
        UI_End_Kills,
        UI_Highscore_Baby_Mode_Insult,
        Achievement_Description_triple_threat,
        Achievement_Description_victory,
        Achievement_Description_corrupted,
        Achievement_Description_slow,
        Achievement_Description_baby,
        Achievement_Description_oof,
        Achievement_Description_dead_baby,
        Achievement_Description_massacre,
        Achievement_Description_nom,
        Achievement_Description_speakers,
        Achievement_Description_speed,
        Achievement_Description_completed,
        Weapon_Launcher_Name,
        Weapon_Launcher_Description,
        Weapon_Sword_Name,
        Weapon_Sword_Description,
        UI_Credits_Guitar_Solo,
        Game_Boss_Clear,
        Game_End_Baby_Mode_Insult,
    }

    [System.Serializable]
    public struct FontLink
    {
        public string languageCode;
        public TMPro.TMP_FontAsset font;
    }
    public FontLink[] fontLinks;
    public TMPro.TMP_FontAsset foreignLanguageFont;

    public static Localizer instance;

    public string currentLanguageCode { get; private set; }

    public string[] availableLanguageCodes { get; private set; }

    public Dictionary<string, string> languageData { get; private set; }

    public Dictionary<string, string> languageCodeToName { get; private set; }
    public Dictionary<string, string> nameToLanguageCode { get; private set; }

    public Dictionary<string, TMPro.TMP_FontAsset> fontDictionary { get; private set; }

    public event Action OnLanguageChanged;

    public void Awake()
    {
        //print(Application.streamingAssetsPath);
        if (instance == null)
        {
            instance = this;

            InitializeLanguageCodeAndNameDictionaries();
            InitializeFontDictionaries();

            availableLanguageCodes = GetAvailableLanguages();
            availableLanguageCodes = VerifyAvailableLanguages(availableLanguageCodes);

            try
            {
                if(!PlayerPrefs.HasKey("Language"))
                {
                    PlayerPrefs.SetString("Language", "en");
                }
                LoadDictionary(PlayerPrefs.GetString("Language", "en"));
            }
            catch
            {
                LoadDictionary("en");
            }

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public string GetLanguageNameFromCode(string languageCode)
    {
        if (languageCodeToName.ContainsKey(languageCode))
        {
            return languageCodeToName[languageCode];
        }
        return languageCode;
    }
    public string GetLanguageCodeFromName(string languageName)
    {
        if (nameToLanguageCode.ContainsKey(languageName))
        {
            return nameToLanguageCode[languageName];
        }
        return languageName;
    }

    public TMPro.TMP_FontAsset GetCurrentFont()
    {
        if(fontDictionary.ContainsKey(currentLanguageCode))
        {
            return fontDictionary[currentLanguageCode];
        }
        return foreignLanguageFont;
    }

    void InitializeLanguageCodeAndNameDictionaries()
    {
        languageCodeToName = new Dictionary<string, string>();
        nameToLanguageCode = new Dictionary<string, string>();

        string languagePath = Path.Combine(Application.streamingAssetsPath, "Languages");
        string filePath = languagePath + "\\language_code_to_language.json";
        if(!File.Exists(filePath))
        {
            Debug.LogWarning("language_code_to_language json file does not exist. Make sure to add this file for localizer engine to function.");
        }
        string json = File.ReadAllText(filePath);
        json = json.Replace("{", "");
        json = json.Replace("}", "");
        //json = json.Replace("\"", "");
        //string[] pairs = json.Split(',');
        string[] pairs = json.Split('\"');
        for (int i = 0; i < pairs.Length; )
        {
            if (pairs[i].Trim() == "," || i == 0 || i == pairs.Length - 1)
            {
                i++;
                continue;
            }
            else
            {
                //print($"{pairs[i + 0]}{pairs[i + 1]}{pairs[i + 2]}");
                //string[] pair = pairs[i].Split(':');
                //if (pair.Length >= 3)
                //{
                languageCodeToName.Add(pairs[i + 0].Trim(), pairs[i + 2].Trim());
                nameToLanguageCode.Add(pairs[i + 2].Trim(), pairs[i + 0].Trim());
                //}
                i += 3;
            }
        }
    }
    void InitializeFontDictionaries()
    {
        fontDictionary = new Dictionary<string, TMPro.TMP_FontAsset>();
        foreach(FontLink fontLink in fontLinks)
        {
            fontDictionary.Add(fontLink.languageCode, fontLink.font);
        }
    }

    public string[] VerifyAvailableLanguages(string[] languageCodes)
    {
        List<string> languageCodesList = languageCodes.ToList();
        for (int i = 0; i < languageCodesList.Count; i++)
        {
            if (!languageCodeToName.ContainsKey(languageCodesList[i])) 
            {
                Debug.LogWarning($"lang_{languageCodesList[i]} is an unknown language or named incorrectly.");

                languageCodesList.RemoveAt(i);
                i--;
            }
        }
        return languageCodesList.ToArray();
    }

    public string[] GetAvailableLanguages()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Languages");
        if (!Directory.Exists(path))
        {
            return null;
        }

        return Directory.GetFiles(path, "lang_*.json").Select(file => Path.GetFileNameWithoutExtension(file).Replace("lang_", "")).ToArray();
    }

    public void LoadDictionary(string languageCode)
    {
        if (currentLanguageCode != languageCode)
        {
            languageData = new Dictionary<string, string>();
            string json = File.ReadAllText(Application.streamingAssetsPath + $"\\Languages\\lang_{languageCode}.json");
            json = json.Replace("{", "");
            json = json.Replace("}", "");
           // json = json.Replace("\"", "");
            //string[] pairs = json.Split(',');
            string[] pairs = json.Split('\"');
            /*foreach(string c in pairs)
            {
                print(c);
            }*/
            for (int i = 0; i < pairs.Length; )
            {
                if (pairs[i].Trim() == "," || i == 0 || i == pairs.Length - 1)
                {
                    i++;
                    continue;
                }
                else
                {
                    //print($"{pairs[i + 0]}{pairs[i + 1]}{pairs[i + 2]}");
                    languageData.Add(pairs[i + 0].Trim(), pairs[i + 2].Trim());
                    i += 3;
                }
/*                    string[] pair = pairs[i].Split(':');
                if (pair.Length >= 2)
                {
                    //print($"{pair[0].Trim()}:{pair[1].Trim()}");
                    languageData.Add(pair[0].Trim(), pair[1].Trim());
                }*/
            }

            Debug.Log($"Language is now {languageCode}");

            currentLanguageCode = languageCode;
            OnLanguageChanged?.Invoke();
        }
    }

    public string GetLanguageText(TextIdentifier identifier)
    {
        if (languageData.ContainsKey(identifier.ToString()))
        {
            return languageData[identifier.ToString()];
        }
        return identifier.ToString();
    }
}

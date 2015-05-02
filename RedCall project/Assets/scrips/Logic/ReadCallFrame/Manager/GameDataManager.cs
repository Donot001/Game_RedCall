using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using System.Xml;
using System.Security.Cryptography;

public class GameDataManager
{
    private const string dataFileName = "GameData.dat";

    private static GameDataManager instance;
    private XmlSaver xmlSaver;
    private GameData gameData;

    public GameDataManager()
    {
        gameData = new GameData();
        xmlSaver = new XmlSaver();

        //		gameData.Key = SystemInfo.deviceUniqueIdentifier;
        Load();
    }

    public void Save()
    {
        SaveToPlayerPrefs();
    }

    private void SaveToFile()
    {
        string gameDataFile = GetDataPath() + "/" + dataFileName;
        string dataString = xmlSaver.SerializeObject(gameData, typeof(GameData));
        xmlSaver.CreateXML(gameDataFile, dataString);
    }

    private void SaveToPlayerPrefs()
    {
        if (Application.isEditor)
        {
            PlayerPrefs.SetString("Account_Editor", gameData.Account);
            PlayerPrefs.SetString("Password_Editor", gameData.Password);
            PlayerPrefs.SetInt("HasVideoPlayedBefore_Editor", gameData.HasVideoPlayedBefore ? 1 : 0);
            PlayerPrefs.SetString("Key_Editor", gameData.Key);
            PlayerPrefs.SetInt("IsClientVersionInitialed_Editor", gameData.IsClientVersionInitialed ? 1 : 0);
            PlayerPrefs.SetString("ClientVersion_Editor", gameData.ClientVersion);
        }
        else
        {
            PlayerPrefs.SetString("Account", gameData.Account);
            PlayerPrefs.SetString("Password", gameData.Password);
            PlayerPrefs.SetInt("HasVideoPlayedBefore", gameData.HasVideoPlayedBefore ? 1 : 0);
            PlayerPrefs.SetString("Key", gameData.Key);
            PlayerPrefs.SetInt("IsClientVersionInitialed", gameData.IsClientVersionInitialed ? 1 : 0);
            PlayerPrefs.SetString("ClientVersion", gameData.ClientVersion);
        }

    }

    private void LoadFromPlayerPrefs()
    {
        if (Application.isEditor)
        {
            gameData.Account = PlayerPrefs.GetString("Account_Editor");
            gameData.Password = PlayerPrefs.GetString("Password_Editor");
            gameData.HasVideoPlayedBefore = PlayerPrefs.GetInt("HasVideoPlayedBefore_Editor") == 1 ? true : false;
            gameData.Key = PlayerPrefs.GetString("Key_Editor");
            gameData.IsClientVersionInitialed = PlayerPrefs.GetInt("IsClientVersionInitialed_Editor") == 1 ? true : false;
            gameData.ClientVersion = PlayerPrefs.GetString("ClientVersion_Editor");
        }
        else
        {
            gameData.Account = PlayerPrefs.GetString("Account");
            gameData.Password = PlayerPrefs.GetString("Password");
            gameData.HasVideoPlayedBefore = PlayerPrefs.GetInt("HasVideoPlayedBefore") == 1 ? true : false;
            gameData.Key = PlayerPrefs.GetString("Key");
            gameData.IsClientVersionInitialed = PlayerPrefs.GetInt("IsClientVersionInitialed") == 1 ? true : false;
            gameData.ClientVersion = PlayerPrefs.GetString("ClientVersion");
        }
    }

    public void Load()
    {
        LoadFromPlayerPrefs();
    }

    private void LoadFromFile()
    {
        string gameDataFile = GetDataPath() + "/" + dataFileName;
        if (xmlSaver.hasFile(gameDataFile))
        {
            string dataString = xmlSaver.LoadXML(gameDataFile);
            GameData gameDataFromXML = xmlSaver.DeserializeObject(dataString, typeof(GameData)) as GameData;

            if (gameDataFromXML.Key == gameData.Key)
            {
                gameData = gameDataFromXML;
            }
            else
            {

            }
        }
        else
        {
            if (gameData != null)
            {
                Save();
            }
        }
    }

    public static string GetDataPath()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            return path + "/Documents" + "GameData";
        }
        else if (Application.isEditor)
        {
            return Application.dataPath + "/GameDataEditor";
        }
        else
        {
            return Application.dataPath;
        }
    }

    public static GameDataManager GetInstance()
    {
        if (instance == null)
        {
            instance = new GameDataManager();
        }
        return instance;
    }

    public GameData GetGameData()
    {
        return gameData;
    }
}

﻿using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace ScenesTeamManagement
{
  public class UserSettings
  {
    public static UserSettings Instance
    {
      get
      {
        if (instance == null)
        {
          instance = new UserSettings ();
        }
        return instance;
      }
    }

    public string TrelloApiKey
    {
      get
      {
        return trelloApiKey;
      }

      set
      {
        trelloApiKey = value;
      }
    }

    public string TrelloApiToken
    {
      get
      {
        return trelloApiToken;
      }

      set
      {
        trelloApiToken = value;
      }
    }

    public int CurrentBranchIndex
    {
      get
      {
        return currentBranchIndex;
      }
      set
      {
        currentBranchIndex = value;
        Save();
      }
    }

    public void Save ()
    {
      deserializedInfo["TrelloApiKey"] = trelloApiKey;
      deserializedInfo["TrelloApiToken"] = trelloApiToken;
      deserializedInfo["CurrentBranchIndex"] = currentBranchIndex.ToString();
      string path = Application.persistentDataPath + "/BlockedScenesUserSettings.data";
      string serializedInfo = MiniJSON.Json.Serialize (deserializedInfo);
      if (File.Exists (path))
      {
        File.Delete (path);
      }

      using (StreamWriter sw = new StreamWriter (path))
      {
        sw.WriteLine (serializedInfo);
        sw.Close ();
      }
    }

    private UserSettings ()
    {
      deserializedInfo = new Dictionary<string, object> ();
      string path = Application.persistentDataPath + "/BlockedScenesUserSettings.data";
      if (File.Exists (path))
      {
        using (StreamReader sr = new StreamReader (path))
        {
          string serializedInfo = sr.ReadToEnd ();
          deserializedInfo = MiniJSON.Json.Deserialize (serializedInfo) as Dictionary<string, object>;
          sr.Close ();
        }
        trelloApiKey = deserializedInfo["TrelloApiKey"] as string;
        trelloApiToken = deserializedInfo["TrelloApiToken"] as string;
        currentBranchIndex = deserializedInfo.ContainsKey("CurrentBranchIndex") ? int.Parse(deserializedInfo["CurrentBranchIndex"] as string) : 0;
      }
    }

    private static UserSettings instance;

    private string trelloApiKey;
    private string trelloApiToken;
    private int currentBranchIndex;
    private Dictionary<string, object> deserializedInfo;
  }
}

using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;

namespace ScenesTeamManagement
{
  public class ScenesManager
  {
    public static ScenesManager Instance
    {
      get
      {
        if (instance == null)
        {
          instance = new ScenesManager ();
        }
        return instance;
      }
    }

    public ReadOnlyCollection<Scene> Scenes
    {
      get
      {
        return scenes.AsReadOnly ();
      }
    }

    public ReadOnlyCollection<Scene> LoadScenes ()
    {
      scenes.Clear ();
      TrelloSettings trelloSettings = ProjectSettings.Instance.TrelloSettings;

      if (trelloSettings.Initialized)
      {
        loadScenesFromTrello ();
        string[] scenesIds = AssetDatabase.FindAssets ("t:scene", new string[] { ProjectSettings.Instance.ScenesFolderPath });
        foreach (string sceneId in scenesIds)
        {
          AddScene (AssetDatabase.GUIDToAssetPath (sceneId));
        }
      }

      return Scenes;
    }

    public void AddScene (string scenePath)
    {
      string sceneName = GetSceneNameFrom (scenePath);
      if (scenes.Find (s => s.Name == sceneName) == null)
      {
        string checkItemId = TrelloAPI.Instance.CreateCheckItem (sceneName);
        scenes.Add (new Scene (sceneName, checkItemId, false, null));
      }
    }

    public void RemoveScene (string scenePath)
    {
      string sceneName = GetSceneNameFrom (scenePath);
      Scene scene = scenes.Find (s => s.Name == sceneName);
      if (scene != null)
      {
        TrelloAPI.Instance.DeleteCheckItem (scene.CheckItemId);
        scenes.Remove (scene);
      }
    }

    public Scene GetSceneWithName (string name)
    {
      return scenes.Find (s => s.Name == name);
    }

    public Scene GetSceneAtPath (string path)
    {
      return GetSceneWithName (GetSceneNameFrom (path));
    }

    public bool IsSceneBlockedByOtherMember (Scene scene)
    {
      return scene != null && scene.Blocked && scene.Owner != TrelloAPI.Instance.UserName;
    }

    public bool IsSceneBlockedByOtherMember (UnityEngine.SceneManagement.Scene scene)
    {
      return IsSceneBlockedByOtherMember(scene.name);
    }

    public bool IsSceneBlockedByOtherMember (string sceneName)
    {
      return IsSceneBlockedByOtherMember(GetSceneWithName(sceneName));
    }

    public string GetOwnerOf (UnityEngine.SceneManagement.Scene scene)
    {
      Scene sceneInfo = scenes.Find (s => s.Name == scene.name);
      return (sceneInfo == null) ? string.Empty : sceneInfo.Owner;
    }

    public string GetSceneNameFrom (string assetPath)
    {
      int startSubstring = assetPath.LastIndexOf ('/') + 1;
      return assetPath.Substring (startSubstring, assetPath.LastIndexOf (".unity") - startSubstring);
    }

    public bool IsAssetPathASceneInFolderPath (string assetPath)
    {
      return assetPath.Contains (".unity") && assetPath.Contains (ProjectSettings.Instance.ScenesFolderPath);
    }

    private ScenesManager ()
    {
      scenes = new List<Scene> ();
    }

    private void loadScenesFromTrello ()
    {
      TrelloSettings trelloSettings = ProjectSettings.Instance.TrelloSettings;
      List<object> checkItems = TrelloAPI.Instance.GetCheckItemsFrom (trelloSettings.CardId, trelloSettings.CheckListId);
      foreach (object checkItem in checkItems)
      {
        Dictionary<string, object> checkItemInfo = checkItem as Dictionary<string, object>;
        bool sceneBlocked = checkItemInfo["state"].Equals ("complete");
        string sceneName = checkItemInfo["name"] as string;
        string owner = string.Empty;
        if (sceneBlocked)
        {
          string[] parsedCheckItemName = sceneName.Split (new string[] { " - " }, System.StringSplitOptions.RemoveEmptyEntries);
          owner = parsedCheckItemName[1];
          sceneName = parsedCheckItemName[0];
        }
        scenes.Add (new Scene (sceneName, checkItemInfo["id"] as string, sceneBlocked, owner));
      }
    }

    private List<Scene> scenes;

    private static ScenesManager instance;
  }
}

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
        return scenesPerBranch[currentBranch].Scenes;
      }
    }

    public string CurrentBranch
    {
      get
      {
        return currentBranch;
      }
      set
      {
        currentBranch = value;
      }
    }

    public string CurrentBranchId
    {
      get
      {
        return scenesPerBranch[currentBranch].ChecklistId;
      }
    }

    public ReadOnlyCollection<Scene> LoadScenes ()
    {
      scenesPerBranch.Clear ();
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
      if (scenesPerBranch[currentBranch].GetSceneWithName (sceneName) == null)
      {
        string checkItemId = TrelloAPI.Instance.CreateCheckItem (sceneName);
        scenesPerBranch[currentBranch].AddScene (new Scene (sceneName, currentBranch, checkItemId, false, null));
      }
    }

    public void RemoveScene (string scenePath)
    {
      string sceneName = GetSceneNameFrom (scenePath);
      Scene scene = scenesPerBranch[currentBranch].GetSceneWithName (sceneName);
      if (scene != null)
      {
        TrelloAPI.Instance.DeleteCheckItem (scene.CheckItemId);
        scenesPerBranch[currentBranch].Remove (scene);
      }
    }

    public Scene GetSceneWithName (string name)
    {
      return scenesPerBranch[currentBranch].GetSceneWithName(name);
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
      Scene sceneInfo = scenesPerBranch[currentBranch].GetSceneWithName(scene.name);
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
      scenesPerBranch = new Dictionary<string, BranchScenes> ();
      currentBranch = ProjectSettings.Instance.Branches[0];
    }

    private void loadScenesFromTrello ()
    {
      TrelloSettings trelloSettings = ProjectSettings.Instance.TrelloSettings;
      foreach (string branchName in ProjectSettings.Instance.Branches)
      {
        List<object> collection = TrelloAPI.Instance.GetChecklistsFrom (ProjectSettings.Instance.TrelloSettings.CardId);
        string id = TrelloAPI.Instance.GetIdFromElement (branchName, collection);
        if (string.IsNullOrEmpty (id))
        {
          id = TrelloAPI.Instance.CreateChecklist (branchName);
        }
        BranchScenes branchScene = new BranchScenes (id);
        scenesPerBranch.Add (branchName, branchScene);


        List<object> checkItems = TrelloAPI.Instance.GetCheckItemsFrom (id);
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
          branchScene.AddScene (new Scene (sceneName, currentBranch, checkItemInfo["id"] as string, sceneBlocked, owner));
        }
      }
      
    }

    private string currentBranch;
    private Dictionary<string, BranchScenes> scenesPerBranch;

    private static ScenesManager instance;
  }
}

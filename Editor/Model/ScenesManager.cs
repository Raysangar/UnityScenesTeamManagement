using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;

namespace ScenesTeamManagement
{
  public class ScenesManager
  {
    public class SceneBlockedAtOtherBranchResponse
    {
      public bool IsBlocked;
      public Scene Scene;
      public string BranchName;

      public static SceneBlockedAtOtherBranchResponse GetNotBlockedResponse()
      {
        return new SceneBlockedAtOtherBranchResponse(false, null, null);
      }

      public static SceneBlockedAtOtherBranchResponse GetBlockedResponse(Scene scene, string branchName)
      {
        return new SceneBlockedAtOtherBranchResponse(true, scene, branchName);
      }

      private SceneBlockedAtOtherBranchResponse(bool blocked, Scene scene, string branchName)
      {
        IsBlocked = blocked;
        Scene = scene;
        BranchName = branchName;
      }
    }

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
        return scenesPerBranch[CurrentBranch].Scenes;
      }
    }

    public string CurrentBranch
    {
      get
      {
        if (UserSettings.Instance.CurrentBranchIndex >= ProjectSettings.Instance.Branches.Count)
        {
          UserSettings.Instance.CurrentBranchIndex = 0;
        }
        return ProjectSettings.Instance.Branches[UserSettings.Instance.CurrentBranchIndex];
      }
    }

    public string CurrentBranchId
    {
      get
      {
        return scenesPerBranch[CurrentBranch].ChecklistId;
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
      if (scenesPerBranch[CurrentBranch].GetSceneWithName (sceneName) == null)
      {
        string checkItemId = TrelloAPI.Instance.CreateCheckItem (sceneName);
        scenesPerBranch[CurrentBranch].AddScene (new Scene (sceneName, CurrentBranch, checkItemId, false, null));
      }
    }

    public void RemoveScene (string scenePath)
    {
      string sceneName = GetSceneNameFrom (scenePath);
      Scene scene = scenesPerBranch[CurrentBranch].GetSceneWithName (sceneName);
      if (scene != null)
      {
        TrelloAPI.Instance.DeleteCheckItem (scene.CheckItemId);
        scenesPerBranch[CurrentBranch].Remove (scene);
      }
    }

    public Scene GetSceneWithName (string name)
    {
      return scenesPerBranch[CurrentBranch].GetSceneWithName(name);
    }

    public Scene GetSceneAtPath (string path)
    {
      return GetSceneWithName (GetSceneNameFrom (path));
    }

    public SceneBlockedAtOtherBranchResponse IsSceneBlockedInOtherBranch(Scene scene)
    {
      return IsSceneBlockedInOtherBranch(scene.Name);
    }

    public SceneBlockedAtOtherBranchResponse IsSceneBlockedInOtherBranch(UnityEngine.SceneManagement.Scene scene)
    {
      return IsSceneBlockedInOtherBranch(GetSceneNameFrom(scene.path));
    }

    public SceneBlockedAtOtherBranchResponse IsSceneBlockedInOtherBranch(string sceneName)
    {
      foreach (KeyValuePair<string, BranchScenes> scenesInBranch in scenesPerBranch)
      {
        if (scenesInBranch.Key != CurrentBranch)
        {
          Scene searchScene = scenesInBranch.Value.GetSceneWithName(sceneName);
          if (searchScene != null && searchScene.Blocked)
          {
            return SceneBlockedAtOtherBranchResponse.GetBlockedResponse(searchScene, scenesInBranch.Key);
          }
        }
      }
      return SceneBlockedAtOtherBranchResponse.GetNotBlockedResponse();
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
      Scene sceneInfo = scenesPerBranch[CurrentBranch].GetSceneWithName(scene.name);
      return (sceneInfo == null) ? string.Empty : sceneInfo.Owner;
    }

    public string GetSceneNameFrom (string assetPath)
    {
      return assetPath.Replace (ProjectSettings.Instance.ScenesFolderPath, "").TrimStart(new char[] { '/' }).Replace(".unity", "");
    }

    public bool IsAssetPathASceneInFolderPath (string assetPath)
    {
      return assetPath.Contains (".unity") && assetPath.Contains (ProjectSettings.Instance.ScenesFolderPath);
    }

    public void TryOpenScene (Scene scene)
    {
      string scenePath = ProjectSettings.Instance.ScenesFolderPath + "/" + scene.Name + ".unity";
      scenePath = scenePath.Replace ("//", "/");
      try
      {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene (scenePath);
      }
      catch(System.ArgumentException)
      {
        UnityEngine.Debug.Log ("Scene " + scenePath + " not found in project");
      }
    }

    public SceneBlockedAtOtherBranchResponse HasSceneBeenBlockedRecently(Scene scene)
    {
      foreach(KeyValuePair<string, BranchScenes> branch in scenesPerBranch)
      {
        Scene branchScene = branch.Value.GetSceneWithName(scene.Name);
        if (branchScene != null)
        {
          List<object> checkItems = TrelloAPI.Instance.GetCheckItemsFrom(branch.Value.ChecklistId);
          Dictionary<string, object> checkItemInfo = checkItems.Find(c => ((c as Dictionary<string, object>)["id"] as string) == branchScene.CheckItemId) as Dictionary<string, object>;
          bool sceneBlocked = checkItemInfo["state"].Equals("complete");
          if (sceneBlocked)
          {
            string sceneName = checkItemInfo["name"] as string;
            string[] parsedCheckItemName = sceneName.Split(new string[] { " - " }, System.StringSplitOptions.RemoveEmptyEntries);
            branchScene.OnSceneBlockedBy(parsedCheckItemName[1]);
            return SceneBlockedAtOtherBranchResponse.GetBlockedResponse(branchScene, branch.Key);
          }
        }
      }
      return SceneBlockedAtOtherBranchResponse.GetNotBlockedResponse();
    }

    private ScenesManager ()
    {
      scenesPerBranch = new Dictionary<string, BranchScenes> ();
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
          branchScene.AddScene (new Scene (sceneName, CurrentBranch, checkItemInfo["id"] as string, sceneBlocked, owner));
        }
      }
      
    }

    private Dictionary<string, BranchScenes> scenesPerBranch;

    private static ScenesManager instance;
  }
}

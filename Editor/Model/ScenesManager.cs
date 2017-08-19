using System.Collections.Generic;
using System.Collections.ObjectModel;

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
      scenes.Clear();
      TrelloSettings trelloSettings = ProjectSettings.Instance.TrelloSettings;

      if (trelloSettings.Initialized)
      {
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

      return Scenes;
    }

    public bool IsSceneBlockedByOtherMember (UnityEngine.SceneManagement.Scene scene)
    {
      Scene sceneInfo = scenes.Find (s => s.Name == scene.name);
      return sceneInfo != null && sceneInfo.Blocked && sceneInfo.Owner != TrelloAPI.Instance.UserName;
    }

    public string GetOwnerOf (UnityEngine.SceneManagement.Scene scene)
    {
      Scene sceneInfo = scenes.Find (s => s.Name == scene.name);
      return (sceneInfo == null) ? string.Empty : sceneInfo.Owner;
    }

    private ScenesManager ()
    {
      scenes = new List<Scene> ();
    }

    private List<Scene> scenes;

    private static ScenesManager instance;
  }
}

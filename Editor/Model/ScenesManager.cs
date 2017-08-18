using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScenesTeamManagement
{
  public class ScenesManager
  {

    public static ReadOnlyCollection<Scene> Scenes
    {
      get
      {
        return scenes.AsReadOnly ();
      }
    }

    public static ReadOnlyCollection<Scene> LoadScenes()
    {
      scenes = new List<Scene> ();
      ProjectSettings projectSettings = ProjectSettings.Instance;
      List<object> checkItems = TrelloAPI.Instance.GetCheckItemsFrom (projectSettings.TrelloSettings.CardId, projectSettings.TrelloSettings.CheckListId);
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
      return Scenes;
    }

    public static bool IsSceneBlockedByOtherMember (UnityEngine.SceneManagement.Scene scene)
    {
      Scene sceneInfo = scenes.Find (s => s.Name == scene.name);
      return sceneInfo != null && sceneInfo.Blocked && sceneInfo.Owner != TrelloAPI.Instance.UserName;
    }

    public static string GetOwnerOf (UnityEngine.SceneManagement.Scene scene)
    {
      Scene sceneInfo = scenes.Find (s => s.Name == scene.name);
      return (sceneInfo == null) ? string.Empty : sceneInfo.Owner;
    }

    private static List<Scene> scenes;
  }
}

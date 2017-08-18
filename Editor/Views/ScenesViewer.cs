using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ScenesTeamManagement
{
  public class ScenesViewer : EditorWindow
  {
    [MenuItem ("Scenes Team Management/Scenes Viewer")]
    public static void ShowWindow ()
    {
      GetWindow (typeof (ScenesViewer));
    }

    private void OnEnable ()
    {
      initInfo ();
    }

    void OnGUI ()
    {
      EditorGUILayout.BeginVertical ();
      foreach (Scene scene in scenes)
      {
        EditorGUILayout.BeginHorizontal ();
        EditorGUILayout.LabelField (scene.Name);

        if (scene.Blocked && !scene.Owner.Equals(TrelloAPI.Instance.UserName))
        {
          GUI.enabled = false;
        }
        bool blocked = EditorGUILayout.Toggle (scene.Blocked);
        GUI.enabled = true;

        if (blocked != scene.Blocked)
        {
          if (blocked)
          {
            scene.BlockScene ();
          }
          else
          {
            scene.FreeScene ();
          }
        }

        EditorGUILayout.LabelField (scene.Blocked ? ("Blocked by " + scene.Owner) : string.Empty);

        EditorGUILayout.EndHorizontal ();
      }

      if (GUILayout.Button ("Refresh Info")) {
        initInfo ();
      }

      EditorGUILayout.EndVertical ();
    }

    private void initInfo ()
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
    }

    private TrelloAPI api;
    private List<Scene> scenes;
  }
}

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
      foreach (Scene scene in ScenesManager.Scenes)
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
      ScenesManager.LoadScenes ();
    }
  }
}

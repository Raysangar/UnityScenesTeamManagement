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
      int branchIndex = getIndexOfCurrentBranch ();
      branchIndex = EditorGUILayout.Popup ("Current Branch", branchIndex, ProjectSettings.Instance.Branches);
      ScenesManager.Instance.CurrentBranch = ProjectSettings.Instance.Branches[branchIndex];

      scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition);
      foreach (Scene scene in ScenesManager.Instance.Scenes)
      {
        EditorGUILayout.BeginHorizontal ();
        EditorGUILayout.LabelField (scene.Name);

        if (scene.Blocked && !scene.Owner.Equals (TrelloAPI.Instance.UserName))
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

      EditorGUILayout.EndScrollView ();

      if (GUILayout.Button ("Refresh Info")) {
        initInfo ();
      }
    }

    private int getIndexOfCurrentBranch ()
    {
      int index = 0;
      while (ProjectSettings.Instance.Branches[index] != ScenesManager.Instance.CurrentBranch)
      {
        ++index;
      }
      return index;
    }

    private void initInfo ()
    {
      ScenesManager.Instance.LoadScenes ();
    }

    private Vector2 scrollPosition;
  }
}

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

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
      UserSettings.Instance.CurrentBranchIndex = EditorGUILayout.Popup ("Current Branch", getIndexOfCurrentBranch (), ProjectSettings.Instance.Branches.ToArray());

      scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition);
      foreach (Scene scene in ScenesManager.Instance.Scenes)
      {
        EditorGUILayout.BeginHorizontal ();

        if (GUILayout.Button("GO"))
        {
          ScenesManager.Instance.TryOpenScene (scene);
        }

        EditorGUILayout.LabelField (scene.Name);

        ScenesManager.SceneBlockedAtOtherBranchResponse sceneBlockedInOtherBranch = ScenesManager.Instance.IsSceneBlockedInOtherBranch(scene);
        if (sceneBlockedInOtherBranch.IsBlocked || (scene.Blocked && !scene.Owner.Equals (TrelloAPI.Instance.UserName)))
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

        if (sceneBlockedInOtherBranch.IsBlocked)
        {
          EditorGUILayout.LabelField("Blocked by " + sceneBlockedInOtherBranch.Scene.Owner + " at branch " + sceneBlockedInOtherBranch.BranchName);
        }
        else
        {
          EditorGUILayout.LabelField (scene.Blocked ? ("Blocked by " + scene.Owner) : string.Empty);
        }


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

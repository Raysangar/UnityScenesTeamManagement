using UnityEditor;
using UnityEditor.SceneManagement;

namespace ScenesTeamManagement
{
  [InitializeOnLoad]
  public static class ScenesBlockedReminder
  {
    static ScenesBlockedReminder ()
    {
      EditorSceneManager.sceneOpened += onSceneOpened;
    }

    private static void onSceneOpened (UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
    {
      if (ScenesManager.Instance.IsSceneBlockedByOtherMember(scene))
      {
        EditorUtility.DisplayDialog ("Warning!", "This scene is blocked by " + ScenesManager.Instance.GetOwnerOf (scene), "Ok");
      }
    }
  }
}
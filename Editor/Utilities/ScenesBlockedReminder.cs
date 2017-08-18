﻿using UnityEditor;
using UnityEditor.SceneManagement;

namespace ScenesTeamManagement
{
  [InitializeOnLoad]
  public static class ScenesBlockedReminder
  {
    private static string currentScene;
    static ScenesBlockedReminder ()
    {
      EditorSceneManager.sceneOpened += onSceneOpened;
    }

    private static void onSceneOpened (UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
    {
      if (ScenesManager.IsSceneBlockedByOtherMember(scene))
      {
        EditorUtility.DisplayDialog ("Warning!", "This scene is blocked by " + ScenesManager.GetOwnerOf (scene), "Ok");
      }
    }
  }
}
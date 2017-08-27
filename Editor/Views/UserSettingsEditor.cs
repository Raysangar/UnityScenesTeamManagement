using UnityEngine;
using UnityEditor;

namespace ScenesTeamManagement
{
  public class UserSettingsEditor : EditorWindow
  {
    [MenuItem ("Scenes Team Management/User Settings")]
    public static void ShowWindow ()
    {
      GetWindow (typeof (UserSettingsEditor));
    }

    private void OnEnable ()
    {
      userSettings = UserSettings.Instance;
    }

    void OnGUI ()
    {
      EditorGUILayout.BeginVertical ();

      EditorGUILayout.BeginHorizontal ();
      userSettings.TrelloApiKey = EditorGUILayout.TextField ("Trello API Key", userSettings.TrelloApiKey);
      if (GUILayout.Button ("Check It Here"))
      {
        Application.OpenURL ("https://trello.com/app-key");
      }
      EditorGUILayout.EndHorizontal ();

      EditorGUILayout.BeginHorizontal ();
      userSettings.TrelloApiToken = EditorGUILayout.TextField ("Trello API Token", userSettings.TrelloApiToken);
      if (!string.IsNullOrEmpty (userSettings.TrelloApiKey) && GUILayout.Button ("Check It Here"))
      {
        Application.OpenURL ("https://trello.com/1/authorize?expiration=never&scope=read,write,account&response_type=token&name=Server%20Token&key=" + userSettings.TrelloApiKey);
      }
      EditorGUILayout.EndHorizontal ();

      EditorGUILayout.EndVertical ();

      if (GUI.changed)
      {
        userSettings.Save ();
      }
    }

    private UserSettings userSettings;
  }
}
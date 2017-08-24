using UnityEngine;
using UnityEditor;

namespace ScenesTeamManagement
{
  [CustomEditor (typeof (ProjectSettings))]
  public class ProjectSettingsCustomInspector : Editor
  {
    [MenuItem ("Scenes Team Management/Project Settings")]
    public static void ShowProjectSettings ()
    {
      Selection.activeObject = ProjectSettings.Instance;
    }

    public override void OnInspectorGUI ()
    {
      ProjectSettings projectSettings = (ProjectSettings) target;

      if (string.IsNullOrEmpty(scenesPath))
      {
        scenesPath = projectSettings.ScenesFolderPath;
      }

      scenesPath = EditorGUILayout.TextField ("Scenes Folder Path", scenesPath);

      EditorGUILayout.Space ();

      EditorGUILayout.LabelField ("Slack Parameters");

      projectSettings.SlackIntegrationEnabled = EditorGUILayout.Toggle ("Slack Integration", projectSettings.SlackIntegrationEnabled);

      if (!projectSettings.SlackIntegrationEnabled)
      {
        GUI.enabled = false;
      }
      projectSettings.SlackSettings.WebhookUrl = EditorGUILayout.TextField("Webhook URL", projectSettings.SlackSettings.WebhookUrl);
      projectSettings.SlackSettings.ChannelName = EditorGUILayout.TextField ("Channel Name", projectSettings.SlackSettings.ChannelName);

      GUI.enabled = true;

      EditorGUILayout.Space ();

      EditorGUILayout.LabelField ("Trello Parameters");

      EditorGUI.BeginDisabledGroup (true);
      EditorGUILayout.Toggle ("API Initialized", projectSettings.TrelloSettings.Initialized);
      EditorGUI.EndDisabledGroup ();

      EditorGUILayout.Space ();

      if (GUILayout.Button ("Configure API"))
      {
        TrelloApiInitializationWindow.ShowWindow ();
      }

      EditorGUILayout.Space ();

      if (GUILayout.Button ("Save Settings"))
      {
        if (!scenesPath.Equals(projectSettings.ScenesFolderPath))
        {
          projectSettings.ScenesFolderPath = scenesPath;
          ScenesManager.Instance.LoadScenes ();
        }

        EditorUtility.SetDirty (projectSettings);
        AssetDatabase.SaveAssets ();
      }
    }

    private string scenesPath;
  }
}

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

      EditorGUILayout.LabelField ("Slack Parameters");

      projectSettings.SlackIntegrationEnabled = EditorGUILayout.Toggle ("Slack Integration", projectSettings.SlackIntegrationEnabled);

      if (!projectSettings.SlackIntegrationEnabled)
      {
        GUI.enabled = false;
      }
      projectSettings.SlackSettings.WebhookUrl = EditorGUILayout.TextField ("Webhook URL", projectSettings.SlackSettings.WebhookUrl);

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
        EditorUtility.SetDirty (projectSettings);
        AssetDatabase.SaveAssets ();
      }
    }
  }
}

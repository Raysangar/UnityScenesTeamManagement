using UnityEngine;
using UnityEditor;
using System.IO;

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

      EditorGUILayout.LabelField ("Common Parameters");

      if (string.IsNullOrEmpty(scenesPath))
      {
        scenesPath = projectSettings.ScenesFolderPath;
      }

      EditorGUILayout.BeginHorizontal ();

      EditorGUILayout.LabelField ("Scenes Forlder Path:", "\"" + scenesPath + "\"");
      if (GUILayout.Button("Select"))
      {
        scenesPath = EditorUtility.OpenFolderPanel ("Select Scenes Folder Directory", "", "");
        int startSubstringIndex = scenesPath.IndexOf ("/Assets") + 1;
        scenesPath = scenesPath.Substring (startSubstringIndex, scenesPath.Length - startSubstringIndex);
      }

      EditorGUILayout.EndHorizontal ();

      drawBranchesGui (projectSettings);

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

    private void drawBranchesGui(ProjectSettings projectSettings)
    {
      branchesFoldout = EditorGUILayout.Foldout (branchesFoldout, "Branches");

      if (branchesFoldout)
      {
        for (int i = 0; i < projectSettings.Branches.Length; ++i)
        {
          EditorGUILayout.BeginHorizontal ();

          projectSettings.Branches[i] = EditorGUILayout.TextField ("Branch " + i, projectSettings.Branches[i]);
          if (GUILayout.Button ("x"))
          {
            string[] branches = projectSettings.Branches;
            projectSettings.Branches = new string[branches.Length - 1];
            for (int j = 0; j < branches.Length; ++j)
            {
              if (j != i)
              {
                projectSettings.Branches[(j < i) ? j : (j - 1)] = branches[j];
              }
            }
          }

          EditorGUILayout.EndHorizontal ();
        }

        EditorGUILayout.BeginHorizontal ();

        EditorGUILayout.LabelField (string.Empty);
        if (GUILayout.Button ("+"))
        {
          string[] branches = projectSettings.Branches;
          projectSettings.Branches = new string[branches.Length + 1];
          branches.CopyTo (projectSettings.Branches, 0);
          projectSettings.Branches[branches.Length - 1] = string.Empty;
        }

        EditorGUILayout.EndHorizontal ();
      }
    }

    private string scenesPath;
    private bool branchesFoldout;
  }
}

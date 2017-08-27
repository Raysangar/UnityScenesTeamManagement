using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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

      if (branches == null)
      {
        branches = projectSettings.Branches;
      }

      EditorGUILayout.LabelField ("Common Parameters");

      if (string.IsNullOrEmpty (scenesPath))
      {
        scenesPath = projectSettings.ScenesFolderPath;
      }

      EditorGUILayout.BeginHorizontal ();

      EditorGUILayout.LabelField ("Scenes Forlder Path:", "\"" + scenesPath + "\"");
      if (GUILayout.Button ("Select"))
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
      projectSettings.SlackSettings.WebhookUrl = EditorGUILayout.TextField ("Webhook URL", projectSettings.SlackSettings.WebhookUrl);
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
        if (!scenesPath.Equals (projectSettings.ScenesFolderPath))
        {
          projectSettings.ScenesFolderPath = scenesPath;
        }
        if (projectSettings.Branches.Count > branches.Count)
        {
          UserSettings.Instance.CurrentBranchIndex = 0;
        }
        projectSettings.Branches = branches;
        ScenesManager.Instance.LoadScenes ();

        EditorUtility.SetDirty (projectSettings);
        AssetDatabase.SaveAssets ();
      }
    }

    private void drawBranchesGui (ProjectSettings projectSettings)
    {
      branchesFoldout = EditorGUILayout.Foldout (branchesFoldout, "Branches");

      if (branchesFoldout)
      {
        branchesToRemove.Clear ();
        for (int i = 0; i < branches.Count; ++i)
        {
          EditorGUILayout.BeginHorizontal ();

          branches[i] = EditorGUILayout.TextField ("Branch " + i, branches[i]);
          if (branches.Count > 1 && GUILayout.Button ("x"))
          {
            branchesToRemove.Add (i);
          }

          EditorGUILayout.EndHorizontal ();
        }

        foreach (int iBranch in branchesToRemove)
        {
          branches.RemoveAt (iBranch);
        }

        EditorGUILayout.BeginHorizontal ();

        EditorGUILayout.LabelField (string.Empty);
        if (GUILayout.Button ("+"))
        {
          branches.Add ("");
        }

        EditorGUILayout.EndHorizontal ();
      }
    }

    private string scenesPath;
    private List<string> branches;
    private List<int> branchesToRemove = new List<int> ();
    private bool branchesFoldout;
  }
}

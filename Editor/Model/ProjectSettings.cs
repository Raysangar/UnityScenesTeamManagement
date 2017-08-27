using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ScenesTeamManagement
{
  [CreateAssetMenu (fileName = "Scenes Team Management Settings")]
  public class ProjectSettings : ScriptableObject
  {
    public static ProjectSettings Instance
    {
      get
      {
        if (instance == null)
        {
          string[] guids = AssetDatabase.FindAssets ("t:ScenesTeamManagement.ProjectSettings");
          if (guids.Length == 0)
          {
            instance = CreateInstance<ProjectSettings> ();
            AssetDatabase.CreateAsset (instance, InstanceDefaultPath);
            AssetDatabase.SaveAssets ();
          }
          else
          {
            instance = AssetDatabase.LoadAssetAtPath<ProjectSettings> (AssetDatabase.GUIDToAssetPath(guids[0]));
          }
        }
        return instance;
      }
    }

    public string ScenesFolderPath
    {
      get
      {
        return scenesFolderPath;
      }

      set
      {
        scenesFolderPath = value;
      }
    }

    public List<string> Branches
    {
      get
      {
        return branches;
      }
      set
      {
        branches = value;
      }
    }

    public TrelloSettings TrelloSettings
    {
      get
      {
        return trelloSettings;
      }
    }

    public bool SlackIntegrationEnabled
    {
      get
      {
        return slackIntegrationEnabled;
      }
      set
      {
        slackIntegrationEnabled = value;
      }
    }

    public SlackSettings SlackSettings
    {
      get
      {
        return slackSettings;
      }
    }

    [SerializeField]
    private string scenesFolderPath;

    [SerializeField]
    private List<string> branches = new List<string> { "master" };

    [SerializeField]
    private TrelloSettings trelloSettings;

    [SerializeField]
    private bool slackIntegrationEnabled;

    [SerializeField]
    private SlackSettings slackSettings;

    private static ProjectSettings instance;

    private const string InstanceDefaultPath = "Assets/Plugins/STMProjectSettings.asset";
  }
}
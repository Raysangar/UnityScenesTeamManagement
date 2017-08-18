using UnityEngine;
using UnityEditor;

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

    public TrelloSettings TrelloSettings
    {
      get
      {
        return trelloSettings;
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
    private TrelloSettings trelloSettings;

    [SerializeField]
    private SlackSettings slackSettings;

    private static ProjectSettings instance;

    private const string InstanceDefaultPath = "Assets/Plugins/STMProjectSettings.asset";
  }
}
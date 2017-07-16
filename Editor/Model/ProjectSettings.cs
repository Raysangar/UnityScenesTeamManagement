using UnityEngine;

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
          instance = Resources.Load<ProjectSettings> ("Scenes Team Management Settings");
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
  }
}
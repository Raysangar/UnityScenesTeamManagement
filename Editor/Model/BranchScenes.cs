using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScenesTeamManagement
{
  public class BranchScenes
  {
    public BranchScenes (string checklistId)
    {
      this.checklistId = checklistId;
      scenes = new List<Scene> ();
    }

    public ReadOnlyCollection<Scene> Scenes
    {
      get
      {
        return scenes.AsReadOnly ();
      }
    }

    public string ChecklistId
    {
      get
      {
        return checklistId;
      }
    }

    public void AddScene (Scene scene)
    {
      scenes.Add (scene);
    }

    public void Remove (Scene scene)
    {
      scenes.Remove (scene);
    }

    public Scene GetSceneWithName(string sceneName)
    {
      return scenes.Find (s => s.Name == sceneName);
    }

    private string checklistId;
    private List<Scene> scenes;
  }
}

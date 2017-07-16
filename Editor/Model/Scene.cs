namespace ScenesTeamManagement
{
  public class Scene
  {

    public string Name
    {
      get
      {
        return name;
      }
    }

    public string CheckItemId
    {
      get
      {
        return checkItemId;
      }
    }

    public bool Blocked
    {
      get
      {
        return blocked;
      }
    }

    public string Owner
    {
      get
      {
        return owner;
      }
    }

    public bool UserOwnsScene
    {
      get { return owner.Equals (TrelloAPI.Instance.UserName); }
    }

    public Scene (string name, string checkItemId, bool blocked, string owner)
    {
      this.name = name;
      this.checkItemId = checkItemId;
      this.blocked = blocked;
      this.owner = owner;
    }

    public void FreeScene ()
    {
      TrelloAPI.Instance.CheckItemOn (checkItemId, Name, false);
      SlackAPI.SendMessage ("Scene " + Name + " freed by " + owner);
      OnSceneFreed ();
    }

    public void BlockScene ()
    {
      OnSceneBlockedBy (TrelloAPI.Instance.UserName);
      TrelloAPI.Instance.CheckItemOn (checkItemId, Name + " - " + owner, true);
      SlackAPI.SendMessage ("Scene " + Name + " blocked by" + owner);
    }

    public void OnSceneFreed ()
    {
      blocked = false;
      owner = null;
    }

    public void OnSceneBlockedBy (string owner)
    {
      blocked = true;
      this.owner = owner;
    }

    private string name;
    private string checkItemId;
    private bool blocked;
    private string owner;
  }
}
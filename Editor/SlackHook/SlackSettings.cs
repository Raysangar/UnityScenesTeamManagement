using UnityEngine;

namespace ScenesTeamManagement
{
  [System.Serializable]
  public class SlackSettings
  {

    public string WebhookUrl
    {
      get
      {
        return webhookUrl;
      }

      set
      {
        webhookUrl = value;
      }
    }

    [SerializeField]
    private string webhookUrl;
  }
}

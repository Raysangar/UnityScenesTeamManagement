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

    public string ChannelName
    {
      get
      {
        return channelName;
      }

      set
      {
        channelName = value;
      }
    }

    [SerializeField]
    private string webhookUrl;

    [SerializeField]
    private string channelName;
  }
}

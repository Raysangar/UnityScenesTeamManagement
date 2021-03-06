﻿using UnityEngine;
using System.Collections.Generic;

namespace ScenesTeamManagement
{
  public class SlackAPI
  {
    public static void SendMessage(string message)
    {
      if (!ProjectSettings.Instance.SlackIntegrationEnabled)
      {
        return;
      }

      var form = new WWWForm();

      form.AddField("payload", MiniJSON.Json.Serialize(new Dictionary<string, string>() { { "text", message }, { "channel", ProjectSettings.Instance.SlackSettings.ChannelName } }));
      form.headers["Content-Type"] = "application/x-www-form-urlencoded";

      var www = new WWW(ProjectSettings.Instance.SlackSettings.WebhookUrl, form);
      while (!www.isDone)
      { }

      if (!string.IsNullOrEmpty(www.error))
      {
        Debug.Log("Slack Error: " + www.error + "\n" + www.text);
      }
    }
  }
}

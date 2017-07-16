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

      EditorGUILayout.LabelField ("Slack Parameters");

      projectSettings.SlackSettings.WebhookUrl = EditorGUILayout.TextField ("Webhook URL", projectSettings.SlackSettings.WebhookUrl);

      EditorGUILayout.Space ();

      EditorGUILayout.LabelField ("Trello Parameters");

      boardName = EditorGUILayout.TextField ("Board Name", boardName);
      listName = EditorGUILayout.TextField ("List Name", listName);
      cardName = EditorGUILayout.TextField ("Card Name", cardName);
      checklistName = EditorGUILayout.TextField ("Checklist Name", checklistName);

      EditorGUILayout.Space ();

      if (GUILayout.Button ("Save Settings"))
      {
        List<object> boards = TrelloAPI.Instance.GetUserBoards ();
        foreach (object board in boards)
        {
          Dictionary<string, object> boardInfo = board as Dictionary<string, object>;
          if (boardInfo["name"].Equals (boardName))
          {
            projectSettings.TrelloSettings.BoardId = boardInfo["id"] as string;
          }
        }

        List<object> lists = TrelloAPI.Instance.GetListsFrom (projectSettings.TrelloSettings.BoardId);
        foreach (object list in lists)
        {
          Dictionary<string, object> listInfo = list as Dictionary<string, object>;
          if (listInfo["name"].Equals (listName))
          {
            projectSettings.TrelloSettings.ListId = listInfo["id"] as string;
          }
        }

        List<object> cards = TrelloAPI.Instance.GetCardsFrom (projectSettings.TrelloSettings.ListId);
        foreach (object card in cards)
        {
          Dictionary<string, object> cardInfo = card as Dictionary<string, object>;
          if (cardInfo["name"].Equals (cardName))
          {
            projectSettings.TrelloSettings.CardId = cardInfo["id"] as string;
          }
        }

        List<object> checklists = TrelloAPI.Instance.GetChecklistsFrom (projectSettings.TrelloSettings.CardId);
        foreach (object checklist in checklists)
        {
          Dictionary<string, object> checklistInfo = checklist as Dictionary<string, object>;
          if (checklistInfo["name"].Equals (checklistName))
          {
            projectSettings.TrelloSettings.CheckListId = checklistInfo["id"] as string;
          }
        }
        EditorUtility.SetDirty (projectSettings);
        AssetDatabase.SaveAssets ();
      }

    }

    private string boardName;
    private string listName;
    private string cardName;
    private string checklistName;
  }
}

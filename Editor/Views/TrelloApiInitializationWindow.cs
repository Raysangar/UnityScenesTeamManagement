using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ScenesTeamManagement
{
  public class TrelloApiInitializationWindow : EditorWindow
  {
    public static void ShowWindow ()
    {
      GetWindow (typeof (TrelloApiInitializationWindow));
    }

    private void OnEnable ()
    {
      projectSettings = ProjectSettings.Instance;
    }

    void OnGUI ()
    {
      EditorGUILayout.LabelField ("Trello Parameters");

      boardName = EditorGUILayout.TextField ("Board Name", boardName);
      listName = EditorGUILayout.TextField ("List Name", listName);
      cardName = EditorGUILayout.TextField ("Card Name", cardName);

      EditorGUILayout.Space ();

      if (GUILayout.Button ("Init API"))
      {
        initTrelloSettings (projectSettings);
        EditorUtility.SetDirty (projectSettings);
        AssetDatabase.SaveAssets ();
      }

      if (!string.IsNullOrEmpty (initializationResultMessage))
      {
        EditorGUILayout.LabelField (initializationResultMessage);
      }
    }

    private void initTrelloSettings (ProjectSettings projectSettings)
    {
      initializationResultMessage = string.Empty;
      projectSettings.TrelloSettings.Initialized = false;

      List<object> collection = TrelloAPI.Instance.GetUserBoards ();
      string id = TrelloAPI.Instance.GetIdFromElement (boardName, collection);
      initializationResultMessage = string.IsNullOrEmpty (id) ? BoardNotFoundMessage : initializationResultMessage;
      if (!string.IsNullOrEmpty (id))
      {
        projectSettings.TrelloSettings.BoardId = id;

        collection = TrelloAPI.Instance.GetListsFrom (projectSettings.TrelloSettings.BoardId);
        id = TrelloAPI.Instance.GetIdFromElement (listName, collection);
        initializationResultMessage = string.IsNullOrEmpty (id) ? ListNotFoundMessage : initializationResultMessage;
        if (!string.IsNullOrEmpty (id))
        {
          projectSettings.TrelloSettings.ListId = id;

          collection = TrelloAPI.Instance.GetCardsFrom (projectSettings.TrelloSettings.ListId);
          id = TrelloAPI.Instance.GetIdFromElement (cardName, collection);
          initializationResultMessage = string.IsNullOrEmpty (id) ? CardNotFoundMessage : initializationResultMessage;
          if (!string.IsNullOrEmpty (id))
          {
            projectSettings.TrelloSettings.CardId = id;
            projectSettings.TrelloSettings.Initialized = true;
            initializationResultMessage = TrelloApiInitializationSucceed;
          }
        }
      }
    }

    ProjectSettings projectSettings;

    private string boardName;
    private string listName;
    private string cardName;
    private string checklistName;
    private string initializationResultMessage;

    private const string BoardNotFoundMessage = "Board not found";
    private const string ListNotFoundMessage = "List not found";
    private const string CardNotFoundMessage = "Card not found";
    private const string TrelloApiInitializationSucceed = "Trello API initialized";
  }
}

﻿using UnityEngine;
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
      checklistName = EditorGUILayout.TextField ("Checklist Name", checklistName);

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
      string id = getIdFromElement (boardName, collection, BoardNotFoundMessage);
      if (!string.IsNullOrEmpty (id))
      {
        projectSettings.TrelloSettings.BoardId = id;

        collection = TrelloAPI.Instance.GetListsFrom (projectSettings.TrelloSettings.BoardId);
        id = getIdFromElement (listName, collection, ListNotFoundMessage);
        if (!string.IsNullOrEmpty (id))
        {
          projectSettings.TrelloSettings.ListId = id;

          collection = TrelloAPI.Instance.GetCardsFrom (projectSettings.TrelloSettings.ListId);
          id = getIdFromElement (cardName, collection, CardNotFoundMessage);
          if (!string.IsNullOrEmpty (id))
          {
            projectSettings.TrelloSettings.CardId = id;

            collection = TrelloAPI.Instance.GetChecklistsFrom (projectSettings.TrelloSettings.CardId);
            id = getIdFromElement (checklistName, collection, ChecklistNotFoundMessage);
            if (!string.IsNullOrEmpty (id))
            {
              projectSettings.TrelloSettings.CheckListId = id;
              projectSettings.TrelloSettings.Initialized = true;
              initializationResultMessage = TrelloApiInitializationSucceed;
            }
          }
        }
      }
    }

    private string getIdFromElement (string elementName, List<object> collection, string errorMessage)
    {
      if (collection == null)
      {
        initializationResultMessage = errorMessage;
        return null;
      }

      int collectionCount = collection.Count;
      int iElement = 0;
      while (iElement < collectionCount && !(collection[iElement] as Dictionary<string, object>)[TrelloNameField].Equals (elementName))
      {
        ++iElement;
      }

      if (iElement == collectionCount)
      {
        initializationResultMessage = errorMessage;
        return null;
      }

      return (collection[iElement] as Dictionary<string, object>)[TrelloIdField] as string;
    }

    ProjectSettings projectSettings;

    private string boardName;
    private string listName;
    private string cardName;
    private string checklistName;
    private string initializationResultMessage;

    private const string TrelloNameField = "name";
    private const string TrelloIdField = "id";
    private const string BoardNotFoundMessage = "Board not found";
    private const string ListNotFoundMessage = "List not found";
    private const string CardNotFoundMessage = "Card not found";
    private const string ChecklistNotFoundMessage = "Checklst not found";
    private const string TrelloApiInitializationSucceed = "Trello API initialized";
  }
}

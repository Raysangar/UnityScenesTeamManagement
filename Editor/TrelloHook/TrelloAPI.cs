using UnityEngine;
using System.Collections.Generic;
using MiniJSON;

namespace ScenesTeamManagement
{
  public class TrelloAPI
  {

    public TrelloAPI (string apiKey, string apiToken)
    {
      this.apiKey = apiKey;
      this.apiToken = apiToken;
    }

    public static TrelloAPI Instance
    {
      get
      {
        if (instance == null)
        {
          UserSettings userSettings = UserSettings.Instance;
          instance = new TrelloAPI (userSettings.TrelloApiKey, userSettings.TrelloApiToken);
        }
        return instance;
      }
    }

    public List<object> GetUserBoards ()
    {
      Dictionary<string, object> respond = sendRequest (string.Format (UserBoardsRequestFormat, apiKey, apiToken));
      return (respond == null) ? new List<object>() : respond["boards"] as List<object>;
    }

    public List<object> GetListsFrom (string boardId)
    {
      Dictionary<string, object> respond = sendRequest (string.Format (BoardListsRequestFormat, boardId, apiKey, apiToken));
      return (respond == null) ? new List<object> () : respond["lists"] as List<object>;
    }

    public List<object> GetCardsFrom (string listId)
    {
      Dictionary<string, object> respond = sendRequest (string.Format (ListCardsRequestFormat, listId, apiKey, apiToken));
      return (respond == null) ? new List<object> () : respond["cards"] as List<object>;
    }

    public List<object> GetChecklistsFrom (string cardId)
    {
      Dictionary<string, object> respond = sendRequest (string.Format (CardChecklistsRequestFormat, cardId, apiKey, apiToken));
      return (respond == null) ? new List<object> () : respond["checklists"] as List<object>;
    }

    public string CreateChecklist(string checklistName)
    {
      string uri = string.Format (CreateChecklistFormat, ProjectSettings.Instance.TrelloSettings.CardId, checklistName, apiKey, apiToken);
      string checklistId = string.Empty;
      UnityHTTP.Request request = new UnityHTTP.Request ("POST", uri);
      request.synchronous = true;
      request.Send (c => {
        UnityEngine.Debug.Log (c.response.Text);
        Dictionary<string, object> dictionary = Json.Deserialize (c.response.Text) as Dictionary<string, object>;
        checklistId = dictionary["id"] as string;
      });
      return checklistId;
    }

    public List<object> GetCheckItemsFrom (string checklistId)
    {
      Dictionary<string, object> respond = sendRequest (string.Format (CheckItemsRequestFormat, checklistId, apiKey, apiToken));
      return (respond == null) ? new List<object> () : respond["checkItems"] as List<object>;
    }

    public void CheckItemOn (string checkItemId, string checkItemName, bool itemChecked)
    {
      string uri = string.Format (CardChecklistsUpdateFormat, ProjectSettings.Instance.TrelloSettings.CardId, ScenesManager.Instance.CurrentBranchId, checkItemId, apiKey, apiToken, itemChecked.ToString ().ToLower (), checkItemName);
      UnityHTTP.Request request = new UnityHTTP.Request ("PUT", uri);
      request.synchronous = true;
      request.Send ();
    }

    public string CreateCheckItem (string checkItemName)
    {
      string uri = string.Format (CreateCheckItemFormat, ScenesManager.Instance.CurrentBranchId, checkItemName, apiKey, apiToken);
      string checkItemId = string.Empty;
      UnityHTTP.Request request = new UnityHTTP.Request ("POST", uri);
      request.synchronous = true;
      request.Send (c => {
        Dictionary<string, object> dictionary = Json.Deserialize (c.response.Text) as Dictionary<string, object>;
        checkItemId = dictionary["id"] as string;
      });
      return checkItemId;
    }

    public void DeleteCheckItem (string checkItemId)
    {
      string uri = string.Format (DeleteCheckItemFormat, ScenesManager.Instance.CurrentBranchId, checkItemId, apiKey, apiToken);
      UnityHTTP.Request request = new UnityHTTP.Request ("DELETE", uri);
      request.synchronous = true;
      request.Send ();
    }

    public string GetIdFromElement (string elementName, List<object> collection)
    {
      if (collection == null)
      {
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
        return null;
      }

      return (collection[iElement] as Dictionary<string, object>)[TrelloIdField] as string;
    }

    public string UserName
    {
      get
      {
        if (string.IsNullOrEmpty (userName))
        {
          Dictionary<string, object> respond = sendRequest (string.Format (UserInfoRequestFormat, apiKey, apiToken));
          userName = respond["fullName"] as string;
        }
        return userName;
      }
    }

    private Dictionary<string, object> sendRequest (string request)
    {
      WWW www = new WWW (request);
      while (!www.isDone)
      { }
      if (!string.IsNullOrEmpty (www.error))
      {
        Debug.Log (www.error + " -> " + www.text);
      }
      return string.IsNullOrEmpty (www.error) ? Json.Deserialize (www.text) as Dictionary<string, object> : null;
    }

    private static TrelloAPI instance;

    private string apiKey;
    private string apiToken;
    private string userName;

    private const string TrelloNameField = "name";
    private const string TrelloIdField = "id";

    private const string UserInfoRequestFormat = "https://api.trello.com/1/members/me?fields=fullName&key={0}&token={1}";
    private const string UserBoardsRequestFormat = "https://api.trello.com/1/members/me?key={0}&token={1}&boards=all";
    private const string BoardListsRequestFormat = "https://api.trello.com/1/boards/{0}?key={1}&token={2}&lists=all";
    private const string ListCardsRequestFormat = "https://api.trello.com/1/lists/{0}?key={1}&token={2}&cards=all";
    private const string CardChecklistsRequestFormat = "https://api.trello.com/1/cards/{0}?key={1}&token={2}&checklists=all";
    private const string CheckItemsRequestFormat = "https://api.trello.com/1/checklists/{0}?key={1}&token={2}";
    private const string ChecklistItemsRequestFormat = "https://api.trello.com/1/checklists/{0}/checkItems?key={1}&token={2}";
    private const string CardChecklistsUpdateFormat = "https://api.trello.com/1/cards/{0}/checklist/{1}/checkItem/{2}/?key={3}&token={4}&state={5}&name={6}";
    private const string CreateChecklistFormat = "https://api.trello.com/1/cards/{0}/checklists?name={1}&key={2}&token={3}";
    private const string CreateCheckItemFormat = "https://api.trello.com/1/checklists/{0}/checkItems?name={1}&key={2}&token={3}";
    private const string DeleteCheckItemFormat = "https://api.trello.com/1/checklists/{0}/checkItems/{1}?key={2}&token={3}";
  }
}
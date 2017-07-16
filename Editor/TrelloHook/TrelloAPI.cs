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
          UserSettings userSettings = UserSettings.Instane;
          instance = new TrelloAPI (userSettings.TrelloApiKey, userSettings.TrelloApiToken);
        }
        return instance;
      }
    }

    public List<object> GetUserBoards ()
    {
      Dictionary<string, object> respond = sendRequest (string.Format (UserBoardsRequestFormat, apiKey, apiToken));
      return (respond == null) ? null : respond["boards"] as List<object>;
    }

    public List<object> GetListsFrom (string boardId)
    {
      Dictionary<string, object> respond = sendRequest (string.Format (BoardListsRequestFormat, boardId, apiKey, apiToken));
      return (respond == null) ? null : respond["lists"] as List<object>;
    }

    public List<object> GetCardsFrom (string listId)
    {
      Dictionary<string, object> respond = sendRequest (string.Format (ListCardsRequestFormat, listId, apiKey, apiToken));
      return (respond == null) ? null : respond["cards"] as List<object>;
    }

    public List<object> GetChecklistsFrom (string cardId)
    {
      Dictionary<string, object> respond = sendRequest (string.Format (CardChecklistsRequestFormat, cardId, apiKey, apiToken));
      return (respond == null) ? null : respond["checklists"] as List<object>;
    }

    public List<object> GetCheckItemsFrom (string cardId, string checklistId)
    {
      List<object> checklists = GetChecklistsFrom (cardId);
      int i = 0;
      int checklistCount = checklists.Count;
      while (i < checklistCount && !(checklists[0] as Dictionary<string, object>)["id"].Equals (checklistId))
      {
        ++i;
      }
      return (i < checklistCount) ? (checklists[i] as Dictionary<string, object>)["checkItems"] as List<object> : null;
    }

    public void CheckItemOn (string checkItemId, string checkItemName, bool itemChecked)
    {
      string uri = string.Format (CardChecklistsUpdateFormat, ProjectSettings.Instance.TrelloSettings.CardId, ProjectSettings.Instance.TrelloSettings.CheckListId, checkItemId, apiKey, apiToken, itemChecked.ToString ().ToLower (), checkItemName);
      UnityHTTP.Request request = new UnityHTTP.Request ("PUT", uri);
      request.synchronous = true;
      request.Send ();
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
        Debug.Log (www.error);
      }
      return string.IsNullOrEmpty (www.error) ? Json.Deserialize (www.text) as Dictionary<string, object> : null;
    }

    private static TrelloAPI instance;

    private string apiKey;
    private string apiToken;
    private string userName;

    private const string UserInfoRequestFormat = "https://api.trello.com/1/members/me?fields=fullName&key={0}&token={1}";
    private const string UserBoardsRequestFormat = "https://api.trello.com/1/members/me?key={0}&token={1}&boards=all";
    private const string BoardListsRequestFormat = "https://api.trello.com/1/boards/{0}?key={1}&token={2}&lists=all";
    private const string ListCardsRequestFormat = "https://api.trello.com/1/lists/{0}?key={1}&token={2}&cards=all";
    private const string CardChecklistsRequestFormat = "https://api.trello.com/1/cards/{0}?key={1}&token={2}&checklists=all";
    private const string ChecklistItemsRequestFormat = "https://api.trello.com/1/checklists/{0}/checkItems?key={1}&token={2}";
    private const string CardChecklistsUpdateFormat = "https://api.trello.com/1/cards/{0}/checklist/{1}/checkItem/{2}/?key={3}&token={4}&state={5}&name={6}";
  }
}
using UnityEngine;

namespace ScenesTeamManagement
{
  [System.Serializable]
  public class TrelloSettings
  {
    public string BoardId
    {
      get
      {
        return boardId;
      }

      set
      {
        boardId = value;
      }
    }

    public string ListId
    {
      get
      {
        return listId;
      }

      set
      {
        listId = value;
      }
    }

    public string CardId
    {
      get
      {
        return cardId;
      }

      set
      {
        cardId = value;
      }
    }

    public bool Initialized
    {
      get
      {
        return initialized;
      }

      set
      {
        initialized = value;
      }
    }

    [SerializeField]
    private string boardId;

    [SerializeField]
    private string listId;

    [SerializeField]
    private string cardId;

    [SerializeField]
    private bool initialized;
  }
}
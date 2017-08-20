using UnityEditor;

namespace ScenesTeamManagement
{
  public class SceneDeleterController : UnityEditor.AssetModificationProcessor
  {
    static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions removeAssetOptions)
    {
      if (ScenesManager.Instance.IsAssetPathASceneInFolderPath(assetPath))
      {
        ScenesManager.Instance.LoadScenes ();
        Scene scene = ScenesManager.Instance.GetSceneAtPath (assetPath);
        if (ScenesManager.Instance.IsSceneBlockedByOtherMember (scene))
        {
          bool result = EditorUtility.DisplayDialog (DialogTitle, string.Format(DialogMessageFormat, scene.Owner), DialogAccept, DialogCancel);
          return result ? AssetDeleteResult.DidNotDelete : AssetDeleteResult.FailedDelete;
        }
      }
      return AssetDeleteResult.DidNotDelete;
    }

    private const string DialogTitle = "Warning!";
    private const string DialogMessageFormat = "This scene is blocked by {0}, Are you sure you want to delete it?";
    private const string DialogAccept = "Yes";
    private const string DialogCancel = "No";
  }
}

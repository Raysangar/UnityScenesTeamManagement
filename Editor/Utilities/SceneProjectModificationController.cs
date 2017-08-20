using UnityEditor;

namespace ScenesTeamManagement
{
  public class SceneProjectModificationController : AssetPostprocessor
  {
    static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
      foreach (string assetPath in importedAssets)
      {
        if (ScenesManager.Instance.IsAssetPathASceneInFolderPath (assetPath))
        {
          ScenesManager.Instance.AddScene (assetPath);
        }
      }

      foreach (string assetPath in deletedAssets)
      {
        if (ScenesManager.Instance.IsAssetPathASceneInFolderPath (assetPath))
        {
          ScenesManager.Instance.LoadScenes ();
          ScenesManager.Instance.RemoveScene (assetPath);
        }
      }

      for (int i = 0; i < movedAssets.Length; i++)
      {
        if (ScenesManager.Instance.IsAssetPathASceneInFolderPath(movedAssets[i]) && 
          !ScenesManager.Instance.IsAssetPathASceneInFolderPath (movedFromAssetPaths[i]))
        {
          ScenesManager.Instance.AddScene (movedAssets[i]);
        }
        else if (!ScenesManager.Instance.IsAssetPathASceneInFolderPath (movedAssets[i]) &&
          ScenesManager.Instance.IsAssetPathASceneInFolderPath (movedFromAssetPaths[i]))
        {
          ScenesManager.Instance.LoadScenes ();
          ScenesManager.Instance.RemoveScene (movedFromAssetPaths[i]);
        }
      }
    }

    
  }
}

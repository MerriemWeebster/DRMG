using CardGame.Core;
using UnityEngine;

namespace DRMG.Core
{
    /// <summary>
    /// Component for loading and unloading a CoreScene.
    /// </summary>
    public class CoreSceneRequestController : MonoBehaviour
    {
        [Tooltip("Specified CoreScene to load.")]
        public CoreScene coreScene;

        /// <summary>
        /// Sends request to CoreSceneManager Instance to load specified CoreScene.
        /// </summary>
        public void SendLoadRequest() => CoreSceneManager.Instance?.AddCoreScene(coreScene);

        /// <summary>
        /// Sends request to CoreSceneManager Instance to unload specified CoreScene.
        /// </summary>
        public void SendUnloadRequest() => CoreSceneManager.Instance?.RemoveCoreScene(coreScene);
    }
}
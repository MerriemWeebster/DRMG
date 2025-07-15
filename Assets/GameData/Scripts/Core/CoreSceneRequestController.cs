using CardGame.Core;
using UnityEngine;

namespace DRMG.Core
{
    public class CoreSceneRequestController : MonoBehaviour
    {
        [Tooltip("Specified CoreScene to load.")]
        public CoreScene coreScene;

        /// <summary>
        /// Sends request to CoreScenesManager Instance to load specified CoreScene.
        /// </summary>
        public void SendLoadRequest() => CoreSceneManager.Instance?.AddCoreScene(coreScene);

        /// <summary>
        /// Sends request to CoreScenesManager Instance to unload specified CoreScene.
        /// </summary>
        public void SendUnoadRequest() => CoreSceneManager.Instance?.RemoveCoreScene(coreScene);
    }
}
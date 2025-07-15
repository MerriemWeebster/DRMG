using CardGame.Core;
using UnityEngine;

namespace DRMG.Core
{
    public class CoreScenesRequestController : MonoBehaviour
    {
        [Tooltip("Specified CoreScene to load.")]
        public CoreScenes coreScene;

        /// <summary>
        /// Sends request to CoreScenesManager Instance to load specified CoreScene.
        /// </summary>
        public void SendRequest() => CoreScenesManager.Instance?.AddCoreScene(coreScene);
    }
}
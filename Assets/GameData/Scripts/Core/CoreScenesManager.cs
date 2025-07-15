using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CardGame.Core
{
    public class CoreScenesManager : MonoBehaviour
    {
        [Tooltip("Sequence of scenes to load. Each scene is added additively.")]
        public CoreScenes[] LoadingSequence;

        /// <summary>
        /// This MonoBehaviour will execute a Start Coroutine that sequentially itterates through
        /// each CoreScenes enum assigned in the LoadingSequence above. Each scene will
        /// be loaded additively via the routine as an async operation.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Start()
        {
            foreach (CoreScenes coreScene in LoadingSequence)
            {
                int sceneIndex = (int)coreScene;
                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
                operation.allowSceneActivation = true;
                yield return operation;
                Debug.Log($"[{nameof(CoreScenesManager)}] Activated scene {coreScene} (sceneIndex)");
            }
        }
    }
}
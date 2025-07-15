using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CardGame.Core
{
    public class CoreScenesManager : MonoBehaviour
    {
        public static CoreScenesManager Instance { get; private set; } // Singleton to handle requests from RequestControllers.

        [Tooltip("Sequence of scenes to load. Each scene is added additively.")]
        public CoreScenes[] LoadingSequence;

        /// <summary>
        /// Begins a Coroutine which additively loads a CoreScene.
        /// Requests are handled parallelly.
        /// </summary>
        /// <param name="coreScene"></param>
        public void AddCoreScene(CoreScenes coreScene) => StartCoroutine(LoadCoreScene(coreScene));

        /// <summary>
        /// This MonoBehaviour will execute a Start Coroutine that sequentially itterates through
        /// each CoreScenes enum assigned in the LoadingSequence above. Each scene will
        /// be loaded additively via the routine as an async operation.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Start()
        {
            Instance = this;
            foreach (CoreScenes coreScene in LoadingSequence)
                yield return LoadCoreScene(coreScene);
        }

        /// <summary>
        /// LoadCoreScene routine which creates the async operation to load the
        /// specified CoreScene additively.
        /// </summary>
        /// <param name="coreScene"></param>
        /// <returns></returns>
        private IEnumerator LoadCoreScene(CoreScenes coreScene)
        {
            int sceneIndex = (int)coreScene;
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            operation.allowSceneActivation = true;
            yield return operation;
            Debug.Log($"[{nameof(CoreScenesManager)}] Activated scene {coreScene} (sceneIndex)");
        }
    }
}
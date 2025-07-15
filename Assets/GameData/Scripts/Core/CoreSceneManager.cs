using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CardGame.Core
{
    /// <summary>
    /// Singleton that handles loading and unloading of scenes.
    /// </summary>
    public class CoreSceneManager : MonoBehaviour
    {
        public static CoreSceneManager Instance { get; private set; } // Singleton to handle requests from RequestControllers.

        [Tooltip("Sequence of scenes to load. Each scene is added additively.")]
        public CoreScene[] LoadingSequence;

        /// <summary>
        /// Begins a Coroutine which additively loads a CoreScene.
        /// Requests are handled parallelly.
        /// </summary>
        /// <param name="coreScene"></param>
        public void AddCoreScene(CoreScene coreScene) => StartCoroutine(LoadCoreScene(coreScene));

        /// <summary>
        /// Begins a Couritine which unloads a CoreScene.
        /// Requests are handled parallelly.
        /// </summary>
        /// <param name="coreScene"></param>
        public void RemoveCoreScene(CoreScene coreScene) => StartCoroutine(UnloadCoreScene(coreScene));

        /// <summary>
        /// This MonoBehaviour will execute a Start Coroutine that sequentially itterates through
        /// each CoreScene enum assigned in the LoadingSequence above. Each scene will
        /// be loaded additively via the routine as an async operation.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Start()
        {
            Instance = this;
            foreach (CoreScene coreScene in LoadingSequence)
                yield return StartCoroutine(LoadCoreScene(coreScene));
        }

        /// <summary>
        /// LoadCoreScene routine which creates the async operation to load the
        /// specified CoreScene additively. Avoids duplicates.
        /// </summary>
        /// <param name="coreScene"></param>
        /// <returns></returns>
        private IEnumerator LoadCoreScene(CoreScene coreScene)
        {
            int sceneIndex = (int)coreScene;
            if (SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded)
            {
                Debug.Log($"[{nameof(CoreSceneManager)}] Already loaded scene {coreScene} ({sceneIndex})");
                yield return null;
            }
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            operation.allowSceneActivation = true;
            yield return operation;
            Debug.Log($"[{nameof(CoreSceneManager)}] Activated scene {coreScene} ({sceneIndex})");
        }

        /// <summary>
        /// UnloadCoreScene routine which creates the async operation to unload the
        /// specified CoreScene.
        /// </summary>
        /// <param name="coreScene"></param>
        /// <returns></returns>
        private IEnumerator UnloadCoreScene(CoreScene coreScene)
        {
            int sceneIndex = (int)coreScene;
            if (!SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded)
            {
                Debug.Log($"[{nameof(CoreSceneManager)}] Ignoring request to unload scene {coreScene} ({sceneIndex})");
                yield return null;
            }
            AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneIndex);
            yield return operation;
            Debug.Log($"[{nameof(CoreSceneManager)}] Unloaded scene {coreScene} ({sceneIndex})");
        }
    }
}
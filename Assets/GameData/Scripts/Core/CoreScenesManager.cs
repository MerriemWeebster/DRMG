using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CardGame.Core
{
    public class CoreScenesManager : MonoBehaviour
    {
        public CoreScenes[] LoadingSequence;

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
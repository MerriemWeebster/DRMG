using UnityEngine;
using UnityEngine.UI;

namespace DRMG.Core
{
    [RequireComponent(typeof(Button))]
    public class ApplicationQuitButton : MonoBehaviour
    {
        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(Application.Quit);
        }
    }
}
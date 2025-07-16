using UnityEngine;
using UnityEngine.EventSystems;

namespace DRMG.Gameplay
{
    [RequireComponent(typeof(Card))]
    public class CardStateController : MonoBehaviour, IPointerClickHandler
    {
        private Card card;

        public void OnPointerClick(PointerEventData eventData)
        {
            card ??= GetComponent<Card>();
            card.FlipCard();
        }
    }
}
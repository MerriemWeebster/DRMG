using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DRMG.Gameplay
{
    [RequireComponent(typeof(Image))]
    public class Card : MonoBehaviour, ICardDataCollectionObserver
    {
        public float animationSpeed = 0.1f;
        public Sprite cardBack;
        private int cardGridIndex = -1;
        private Image image;
        private CardData cardData;
        private CardState previousState;
        private Coroutine updateStateRoutine;

        public void OnCardDataCollectionModified(CardData[] cardDataCollection)
        {
            if (cardDataCollection == null || cardGridIndex == -1) return;
            cardData = cardDataCollection[cardGridIndex];
            if (updateStateRoutine != null) StopCoroutine(updateStateRoutine);
            updateStateRoutine = StartCoroutine(UpdateCardState());
        }

        public void SetCardGridIndex(int index)
        {
            if (!gameObject.activeSelf) return;
            if (cardGridIndex == index) return;
            cardGridIndex = index;
            OnDisable();
            OnEnable();
        }

        public void FlipCard()
        {
            if (cardGridIndex == -1 || cardData.cardState != CardState.FaceDown) return;
            MatchDataManager.MatchDataSubject.UpdateCardState(cardGridIndex, CardState.FaceUp);
        }

        private IEnumerator UpdateCardState()
        {
            while (previousState != cardData.cardState)
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Clamp(currentScale.x - Time.deltaTime * animationSpeed, 0f, currentScale.x);
                if (currentScale.x == 0f)
                    previousState = cardData.cardState;
                transform.localScale = currentScale;
                yield return null;
            }

            image ??= GetComponent<Image>();
            image.sprite = cardData.cardState == CardState.FaceDown ? cardBack : cardData.GetCardFaceSprite();
            image.color = cardData.cardState == CardState.Matched ? Color.gray : Color.white;

            while (transform.localScale.x != 1f)
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Clamp(currentScale.x + Time.deltaTime * animationSpeed, currentScale.x, 1f);
                transform.localScale = currentScale;
                yield return null;
            }

            updateStateRoutine = null;
        }

        private void OnEnable() => MatchDataManager.MatchDataSubject.Subscribe(this);
        private void OnDisable() => MatchDataManager.MatchDataSubject.Unsubscribe(this);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DRMG.Gameplay
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(AudioSource))]
    public class Card : MonoBehaviour, ICardDataCollectionObserver
    {
        public float animationSpeed = 0.1f;
        public Sprite cardBack;
        public AudioClip flipSound, matchSound, mismatchSound;
        private int cardGridIndex = -1;
        private AudioSource audioSource;
        private Image image;
        private CardData cardData;
        private CardState previousState;
        private Coroutine updateStateRoutine;

        public void OnCardDataCollectionModified(CardData[] cardDataCollection)
        {
            if (cardDataCollection == null || cardGridIndex == -1) return;
            if (updateStateRoutine != null) StopCoroutine(updateStateRoutine);
            cardData = cardDataCollection[cardGridIndex];
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
            if (cardGridIndex == -1 || cardData.cardState == CardState.FaceUp || cardData.cardState == CardState.Matched) return;
            MatchDataManager.MatchDataSubject.UpdateCardState(cardGridIndex, CardState.FaceUp);
        }

        private IEnumerator UpdateCardState()
        {
            audioSource ??= GetComponent<AudioSource>();
            image ??= GetComponent<Image>();
            Sprite targetSprite = cardData.cardState == CardState.FaceDown ? cardBack : cardData.GetCardFaceSprite();

            if (previousState == CardState.FaceDown && cardData.cardState == CardState.FaceUp
                || previousState == CardState.FaceUp && cardData.cardState == CardState.FaceDown)
                audioSource.PlayOneShot(flipSound);

            if (previousState != CardState.Matched && cardData.cardState == CardState.Matched)
                audioSource.PlayOneShot(matchSound);


            if (previousState != CardState.FailedMatch && cardData.cardState == CardState.FailedMatch)
                audioSource.PlayOneShot(mismatchSound);

            while (previousState != cardData.cardState)
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Clamp(currentScale.x - Time.deltaTime * animationSpeed, 0f, currentScale.x);
                if (currentScale.x == 0f || targetSprite == image.sprite)
                    previousState = cardData.cardState;
                transform.localScale = currentScale;
                yield return null;
            }

            image.sprite = targetSprite;
            image.color = cardData.cardState == CardState.Matched ? Color.gray : (cardData.cardState == CardState.FailedMatch ? Color.red : Color.white);

            while (transform.localScale.x != 1f)
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Clamp(currentScale.x + Time.deltaTime * animationSpeed, currentScale.x, 1f);
                transform.localScale = currentScale;
                yield return null;
            }

            if (cardData.cardState == CardState.FailedMatch)
            {
                yield return new WaitForSeconds(0.25f);
                MatchDataManager.MatchDataSubject.UpdateCardState(cardGridIndex, CardState.FaceDown);
            }

            updateStateRoutine = null;
        }

        private void OnEnable() => MatchDataManager.MatchDataSubject.Subscribe(this);
        private void OnDisable() => MatchDataManager.MatchDataSubject.Unsubscribe(this);
    }
}
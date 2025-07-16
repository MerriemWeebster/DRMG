using TMPro;
using UnityEngine;

namespace DRMG.Gameplay
{
    [RequireComponent(typeof(AudioSource))]
    public class ScoreManager : MonoBehaviour, ICardDataCollectionObserver
    {
        public float comboThresholdSeconds = 0.5f;
        public TextMeshProUGUI scoreText;
        public AudioClip gameOverSound;
        private int combo = 1;
        private float lastScore = float.MinValue;
        private AudioSource audioSource;

        public void OnCardDataCollectionModified(CardData[] cardDataCollection)
        {
            for (int i = 0; i < cardDataCollection.Length; i++)
            {
                if (cardDataCollection[i].cardState == CardState.FaceUp)
                {
                    for (int j = 0; j < cardDataCollection.Length; j++)
                    {
                        if (i == j) continue;
                        if (cardDataCollection[j].cardState == CardState.FaceUp)
                        {
                            if (cardDataCollection[j].cardFaceId == cardDataCollection[i].cardFaceId)
                            {
                                if (Time.time <= lastScore + comboThresholdSeconds)
                                    combo++;
                                MatchDataManager.MatchDataSubject.UpdateCardState(i, CardState.Matched);
                                MatchDataManager.MatchDataSubject.UpdateCardState(j, CardState.Matched);
                                lastScore = Time.time;
                                MatchDataManager.MatchDataSubject.SetScore(MatchDataManager.MatchDataSubject.GetScore() + combo);
                            }
                            else
                            {
                                combo = 1;
                                MatchDataManager.MatchDataSubject.UpdateCardState(i, CardState.FailedMatch);
                                MatchDataManager.MatchDataSubject.UpdateCardState(j, CardState.FailedMatch);
                            }

                            break;
                        }
                    }
                }
            }

            bool gameOver = true;
            foreach (CardData cardData in cardDataCollection)
            {
                if (cardData.cardState != CardState.Matched)
                {
                    gameOver = false;
                    break;
                }
            }

            if (gameOver)
            {
                audioSource ??= GetComponent<AudioSource>();
                audioSource.PlayOneShot(gameOverSound);
            }
        }

        private void Update()
        {
            if (Time.time > lastScore + comboThresholdSeconds)
                combo = 1;
            scoreText.text = $"Score: {MatchDataManager.MatchDataSubject.GetScore()}\nCombo: {combo}x";
        }

        private void SetScore(int score)
        {
            MatchDataManager.MatchDataSubject.SetScore(MatchDataManager.MatchDataSubject.GetScore() + combo);
        }

        private void OnEnable() => MatchDataManager.MatchDataSubject.Subscribe(this);
        private void OnDisable() => MatchDataManager.MatchDataSubject.Unsubscribe(this);
    }
}
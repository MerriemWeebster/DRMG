using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace DRMG.Gameplay
{
    [Serializable]
    public class MatchDataSubject
    {
        [SerializeField] private bool createdCards;
        public List<Sprite> CardFaces { get; private set; }
        [SerializeField, HideInInspector] private bool allowRepition;
        [SerializeField, HideInInspector] private int score;
        [Min(2), SerializeField, HideInInspector] private int gridWidth = 3;
        [Min(2), SerializeField, HideInInspector] private int gridHeight = 2;
        [SerializeField, HideInInspector] private CardData[] cardDataCollection = new CardData[0];
        private List<ICardDataCollectionObserver> observers = new List<ICardDataCollectionObserver>();

        public MatchDataSubject(int gridWidth = 3, int gridHeight = 2, bool allowRepition = false)
        {
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            this.allowRepition = allowRepition;
            cardDataCollection = new CardData[gridWidth * gridHeight];
            CardFaces = new List<Sprite>();
            ResizeCardDataCollection();
        }

        public bool CreatedCards() => createdCards;
        public bool AllowRepitition() => allowRepition;
        public bool ValidGridSize(int gridWidth, int gridHeight)
        {
            int gridSize = gridWidth * gridHeight;
            if (gridSize % 2 != 0 || (!AllowRepitition() && gridSize > CardFaces.Count))
                return false;
            return true;
        }

        public int GetScore() => score;
        public int GetGridSize() => gridWidth * gridHeight;
        public int GetGridWidth() => gridWidth;
        public int GetGridHeight() => gridHeight;
        public void SetScore(int score) => this.score = score;
        public void SetCardFaces(List<Sprite> cardFaces) => CardFaces = cardFaces;

        public void SetAllowRepititions(bool value)
        {
            allowRepition = value;
            if (!allowRepition && GetGridSize() > CardFaces.Count)
                SetGridSize(2, 2);
            ResetMatch();
        }

        public void SetGridSize(int gridWidth, int gridHeight)
        {
            if (!ValidGridSize(gridWidth, gridHeight))
                return;
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            ResizeCardDataCollection();
        }

        public void UpdateCardState(int gridIndex, CardState cardState)
        {
            cardDataCollection[gridIndex].cardState = cardState;
            NotifyObservers();
        }

        public void UpdateCardDataCollection(CardData[] cardDataCollection)
        {
            if (cardDataCollection.Length < this.cardDataCollection.Length)
                throw new Exception($"[{nameof(MatchDataSubject)}] UpdateCardDataCollection requires collection to be the length of GridSize ({gridWidth * gridHeight})");
            for (int i = 0; i < this.cardDataCollection.Length; i++)
                this.cardDataCollection[i] = cardDataCollection[i];
            NotifyObservers();
        }

        public void Subscribe(ICardDataCollectionObserver observer)
        {
            observers ??= new List<ICardDataCollectionObserver>();
            if (!observers.Contains(observer))
                observers.Add(observer);
            observer.OnCardDataCollectionModified(cardDataCollection);
        }

        public void Unsubscribe(ICardDataCollectionObserver observer) => observers.Remove(observer);

        public void ResetMatch()
        {
            score = 0;
            createdCards = true;
            int gridSize = MatchDataManager.MatchDataSubject.GetGridSize();
            int pairSize = gridSize / 2;
            HashSet<int> cardIdsSet = new HashSet<int>();
            while (cardIdsSet.Count < pairSize)
            {
                int randomId = UnityEngine.Random.Range(0, MatchDataManager.MatchDataSubject.CardFaces.Count);
                cardIdsSet.Add(randomId);
            }

            int cardIdx = 0;
            int[] cardIds = cardIdsSet.ToArray();
            CardData[] cardDataCollection = new CardData[gridSize];
            HashSet<int> gridSlotsSet = new HashSet<int>();
            while (gridSlotsSet.Count < gridSize)
            {
                int slot1 = -1;
                int slot2 = -1;

                while (slot1 == slot2)
                {
                    slot1 = UnityEngine.Random.Range(0, gridSize);
                    slot2 = UnityEngine.Random.Range(0, gridSize);
                }

                if (!gridSlotsSet.Contains(slot1) && !gridSlotsSet.Contains(slot2))
                {
                    int cardId = cardIds[cardIdx++];
                    cardDataCollection[slot1].cardFaceId = cardId;
                    cardDataCollection[slot2].cardFaceId = cardId;
                    gridSlotsSet.Add(slot1);
                    gridSlotsSet.Add(slot2);
                }

                UpdateCardDataCollection(cardDataCollection);
            }
        }

        private void ResizeCardDataCollection()
        {
            int gridSize = gridWidth * gridHeight;
            int copySize = math.min(gridSize, cardDataCollection.Length);
            CardData[] resizedCollection = new CardData[gridSize];
            Array.Copy(cardDataCollection, resizedCollection, copySize);
            cardDataCollection = resizedCollection;
            ResetMatch();
            NotifyObservers();
        }

        private void NotifyObservers()
        {
            observers ??= new List<ICardDataCollectionObserver>();
            foreach (ICardDataCollectionObserver observer in observers.ToArray())
                observer?.OnCardDataCollectionModified(cardDataCollection);
        }
    }
}
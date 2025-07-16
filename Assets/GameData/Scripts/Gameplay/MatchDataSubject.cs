using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace DRMG.Gameplay
{
    [Serializable]
    public class MatchDataSubject
    {
        public List<Sprite> CardFaces { get; private set; }
        [SerializeField, HideInInspector] private CardData[] cardDataCollection;
        [SerializeField, HideInInspector] private bool allowRepition;
        [Min(2), SerializeField, HideInInspector] private int gridWidth = 2;
        [Min(2), SerializeField, HideInInspector] private int gridHeight = 2;
        private List<ICardDataCollectionObserver> observers = new List<ICardDataCollectionObserver>();

        public MatchDataSubject(int gridWidth = 2, int gridHeight = 2, bool allowRepition = false)
        {
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            this.allowRepition = allowRepition;
            cardDataCollection = new CardData[gridWidth * gridHeight];
            CardFaces = new List<Sprite>();
            ResizeCardDataCollection();
        }

        public bool DoesAllowRepitition() => allowRepition;

        public bool SetGridSize(int gridWidth, int gridHeight)
        {
            int gridSize = gridWidth * gridHeight;
            if (gridSize % 2 != 0 || (!DoesAllowRepitition() && gridSize > CardFaces.Count))
                return false;
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            ResizeCardDataCollection();
            return true;
        }

        public int GetGridSize() => gridWidth * gridHeight;
        public int GetGridWidth() => gridWidth;
        public int GetGridHeight() => gridHeight;
        public void SetCardFaces(List<Sprite> cardFaces) => this.CardFaces = cardFaces;

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
            if (!observers.Contains(observer))
                observers.Add(observer);
            observer.OnCardDataCollectionModified(cardDataCollection);
        }

        public void Unsubscribe(ICardDataCollectionObserver observer) => observers.Remove(observer);

        private void ResizeCardDataCollection()
        {
            int gridSize = gridWidth * gridHeight;
            int copySize = math.min(gridSize, cardDataCollection.Length);
            CardData[] resizedCollection = new CardData[gridSize];
            Array.Copy(cardDataCollection, resizedCollection, copySize);
            cardDataCollection = resizedCollection;
        }

        private void NotifyObservers()
        {
            foreach (ICardDataCollectionObserver observer in observers)
                observer.OnCardDataCollectionModified(cardDataCollection);
        }
    }
}
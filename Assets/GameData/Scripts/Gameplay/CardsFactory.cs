using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DRMG.Gameplay
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class CardsFactory : MonoBehaviour, ICardDataCollectionObserver
    {
        public Vector2 maxCellSize = new Vector2(300, 400);
        public Vector2 spacing = new Vector2(25, 25);
        public float aspectRatio = 3f / 4f;
        public Transform cardsContainer;
        public Card cardPrefab;
        private bool createdCards;
        private GridLayoutGroup grid;
        private RectTransform rectTransform;
        private List<Card> activeCards = new List<Card>();
        private Vector2 currentResolution;

        public void OnCardDataCollectionModified(CardData[] cardDataCollection)
        {
            if (cardDataCollection == null || createdCards) return;
            for (int i = 0; i < cardDataCollection.Length; i++)
            {
                if (activeCards.Count >= i)
                    activeCards.Add(Instantiate(cardPrefab, cardsContainer));
                activeCards[i].SetCardGridIndex(i);
            }

            createdCards = true;
            UpdateGridLayoutGroup();
        }

        public void SetupGrid()
        {
            int gridSize = MatchDataManager.MatchDataSubject.GetGridSize();
            int pairSize = gridSize / 2;
            HashSet<int> cardIdsSet = new HashSet<int>();
            while (cardIdsSet.Count < pairSize)
            {
                int randomId = Random.Range(0, MatchDataManager.MatchDataSubject.CardFaces.Count);
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
                    slot1 = Random.Range(0, gridSize);
                    slot2 = Random.Range(0, gridSize);
                }

                if (!gridSlotsSet.Contains(slot1) && !gridSlotsSet.Contains(slot2))
                {
                    int cardId = cardIds[cardIdx++];
                    cardDataCollection[slot1].cardFaceId = cardId;
                    cardDataCollection[slot2].cardFaceId = cardId;
                    gridSlotsSet.Add(slot1);
                    gridSlotsSet.Add(slot2);
                }

                MatchDataManager.MatchDataSubject.UpdateCardDataCollection(cardDataCollection);
            }
        }

        public void ResetGrid()
        {
            createdCards = false;
            SetupGrid();
        }

        private void UpdateGridLayoutGroup()
        {
            grid ??= GetComponent<GridLayoutGroup>();
            rectTransform ??= GetComponent<RectTransform>();
            int childCount = transform.childCount;
            if (childCount == 0)
                return;

            Vector2 bestCellSize = maxCellSize;
            float containerWidth = rectTransform.rect.width - grid.padding.left - grid.padding.right;
            float containerHeight = rectTransform.rect.height - grid.padding.top - grid.padding.bottom;
            int columns = MatchDataManager.MatchDataSubject.GetGridWidth();
            int rows = MatchDataManager.MatchDataSubject.GetGridHeight();
            float totalSpacingX = spacing.x * (columns - 1);
            float totalSpacingY = spacing.y * (rows - 1);
            float availableWidth = containerWidth - totalSpacingX;
            float availableHeight = containerHeight - totalSpacingY;
            float cellWidth = availableWidth / columns;
            float cellHeight = availableHeight / rows;
            cellWidth = cellWidth / cellHeight > aspectRatio ? cellHeight * aspectRatio : cellWidth / aspectRatio;
            if (cellWidth <= maxCellSize.x && cellHeight <= maxCellSize.y)
                bestCellSize = new Vector2(cellWidth, cellHeight);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.cellSize = bestCellSize;
            grid.spacing = spacing;
        }

        private void Awake() => ResetGrid();
        private void Update()
        {
            if (currentResolution.x == Screen.width && currentResolution.y == Screen.height)
                return;
            currentResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            UpdateGridLayoutGroup();
        }
        private void OnEnable() => MatchDataManager.MatchDataSubject.Subscribe(this);
        private void OnDisable() => MatchDataManager.MatchDataSubject.Unsubscribe(this);
    }
}
using System.Collections.Generic;
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
        private bool gridSetupComplete = false;
        private GridLayoutGroup grid;
        private RectTransform rectTransform;
        private List<Card> activeCards = new List<Card>();
        private Vector2 currentResolution;

        public void ResetGrid() => MatchDataManager.MatchDataSubject.ResetMatch();

        public void OnCardDataCollectionModified(CardData[] cardDataCollection)
        {
            if (cardDataCollection == null || gridSetupComplete) return;
            if (!MatchDataManager.MatchDataSubject.CreatedCards())
            {
                ResetGrid();
                return;
            }
            
            for (int i = 0; i < cardDataCollection.Length; i++)
            {
                if (activeCards.Count >= i)
                    activeCards.Add(Instantiate(cardPrefab, cardsContainer));
                activeCards[i].SetCardGridIndex(i);
            }

            gridSetupComplete = true;
            UpdateGridLayoutGroup();
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
            if (cellWidth / cellHeight > aspectRatio)
                cellWidth = cellHeight * aspectRatio;
            else
                cellHeight = cellWidth / aspectRatio;
            if (cellWidth <= maxCellSize.x && cellHeight <= maxCellSize.y)
                bestCellSize = new Vector2(cellWidth, cellHeight);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.cellSize = bestCellSize;
            grid.spacing = spacing;
        }

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
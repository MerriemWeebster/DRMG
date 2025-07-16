using System;
using UnityEngine;

namespace DRMG.Gameplay
{
    [Serializable]
    public struct CardData
    {
        public int cardFaceId;
        [NonSerialized]
        public CardState cardState;

        public CardData(int cardFaceId, CardState cardState = CardState.FaceDown)
        {
            this.cardFaceId = cardFaceId;
            this.cardState = cardState;
        }

        public Sprite GetCardFaceSprite() => MatchDataManager.MatchDataSubject.CardFaces[cardFaceId];
    }
}
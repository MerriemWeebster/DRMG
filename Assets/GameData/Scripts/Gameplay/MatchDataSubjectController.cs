using UnityEngine;

namespace DRMG.Gameplay
{
    public class MatchDataSubjectController : MonoBehaviour
    {
        public CardFacesObject cardFacesObject;
        private void Awake() => MatchDataManager.LoadMatchData().SetCardFaces(cardFacesObject.cardFaces);
    }
}

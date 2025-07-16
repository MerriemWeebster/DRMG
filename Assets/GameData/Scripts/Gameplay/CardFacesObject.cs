using System.Collections.Generic;
using UnityEngine;

namespace DRMG.Gameplay
{
    [CreateAssetMenu(fileName = "CardFacesObject", menuName = "DRMG/Create Card Faces Object")]
    public class CardFacesObject : ScriptableObject
    {
        public List<Sprite> cardFaces;
    }
}
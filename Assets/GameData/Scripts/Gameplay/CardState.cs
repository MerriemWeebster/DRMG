using System;

namespace DRMG.Gameplay
{
    [Serializable]
    public enum CardState
    {
        FaceDown,
        FaceUp,
        Matched,
        FailedMatch
    }
}
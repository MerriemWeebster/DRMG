namespace DRMG.Gameplay
{
    public interface ICardDataCollectionObserver
    {
        public void OnCardDataCollectionModified(CardData[] cardDataCollection);
    }
}
using PriceTracker.Models;

namespace PriceTracker.Intefaces
{
    public interface IPriceHistoryRepository
    {
        void Insert(PriceHistory priceHistory);
        PriceHistory Get(long id);
    }
}

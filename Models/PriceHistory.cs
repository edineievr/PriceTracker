namespace PriceTracker.Models
{
    public class PriceHistory
    {
        public long Id { get; set; }
        public string ProductName { get; set; }
        public string Platform { get; set; }
        public string Link { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal? OldPrice { get; set; }
        public bool Changed => PriceChanged();

        public PriceHistory()
        {
            ProductName = string.Empty;
            Platform = string.Empty;
            Link = string.Empty;
        }

        public bool PriceChanged()
        {
            return CurrentPrice != OldPrice;

        }
    }
}

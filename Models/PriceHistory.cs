namespace PriceTracker.Models
{
    public class PriceHistory
    {
        public long Id { get; set; }
        public string ProductName { get; set; }
        public string Platform { get; set; }
        public string Link { get; set; }
        public string CurrentPrice { get; set; }
        public string OldPrice { get; set; }
        //public bool Changed => PriceChanged();

        public PriceHistory()
        {
            ProductName = string.Empty;
            Platform = string.Empty;
            Link = string.Empty;
            CurrentPrice = string.Empty;
            OldPrice = string.Empty;
        }

        public bool PriceChanged()
        {
            return CurrentPrice != OldPrice;
        }
    }
}

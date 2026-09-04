namespace kakto
{
    public class Product
    {
        public string Article { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string ManufacturerName { get; set; }
        public string Supplier { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public int StockQuantity { get; set; }
        public string Description { get; set; }
        public string PhotoPath { get; set; }

        public decimal FinalPrice
        {
            get
            {
                return Price - (Price * Discount / 100m);
            }
        }

        public string DiscountText
        {
            get
            {
                return Discount + "%";
            }
        }

        public bool HasDiscount
        {
            get
            {
                return Discount > 0;
            }
        }

        public bool IsOutOfStock
        {
            get
            {
                return StockQuantity == 0;
            }
        }
        public int CategoryID { get; set; }

        public int ManufacturerID { get; set; }
        public bool IsBigDiscount
        {
            get
            {
                return Discount > 25;
            }
        }
    }
}
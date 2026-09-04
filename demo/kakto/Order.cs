namespace kakto
{
    public class Order
    {
        public int OrderID { get; set; }

        public string ProductArticle { get; set; }

        public string StatusName { get; set; }

        public string PickupAddress { get; set; }

        public System.DateTime OrderDate { get; set; }

        public System.DateTime DeliveryDate { get; set; }
    }
}
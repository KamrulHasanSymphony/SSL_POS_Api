namespace ShampanPOS.ViewModel
{
    public class ProductSaleModel
    {
        public int ProductId { get; set; }
        public int BranchId { get; set; }
        public string ProductName { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal AverageUnitRate { get; set; }
        public decimal TotalSaleValue { get; set; }
    }
}

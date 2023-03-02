namespace PointOfSaleTerminal.Models
{
    public class PricingModel
    {
        public string ProductCode { get; init; }

        public decimal UnitPrice { get; init; }

        public bool HasVolumePricing => VolumePricing != null;

        public int? VolumePricingQuantity { get; init; } = null;

        public decimal? VolumePricing { get; init; } = null;

        public PricingModel(string productCode, decimal unitPrice, int volumePricingQuantity, decimal volumePricing) {
            ProductCode = productCode;
            UnitPrice = unitPrice;
            VolumePricingQuantity = volumePricingQuantity;
            VolumePricing = volumePricing;
        }

        public PricingModel(string productCode, decimal unitPrice) {
            ProductCode = productCode;
            UnitPrice = unitPrice;
        }
    }
}

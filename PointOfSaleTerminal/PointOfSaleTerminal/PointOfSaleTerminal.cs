using PointOfSaleTerminal.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PointOfSaleTerminal
{
    public class PointOfSaleTerminal
    {
        List<PricingModel> allPrices = new List<PricingModel>();

        Dictionary<string, int> basket = new Dictionary<string, int>();

        public PointOfSaleTerminal() {
        }

        public void SetPricing(IEnumerable<PricingModel> pricingModels) {
            allPrices.Clear();
            HashSet<string> productCodes = new HashSet<string>();
            foreach (PricingModel model in pricingModels) {
                if (productCodes.Contains(model.ProductCode)) {
                    throw new InvalidOperationException($"Cannot have multiple pricing models for the same product code: {model.ProductCode}");
                }
                productCodes.Add(model.ProductCode);
                allPrices.Add(model);
            }
        }

        public bool TryScanProduct(string productCode) {
            if (string.IsNullOrEmpty(productCode)) return false;

            if (basket.ContainsKey(productCode)) {
                basket[productCode] = basket[productCode] + 1;
                return true;
            }

            if (allPrices.Any(p => p.ProductCode == productCode)) {
                basket.Add(productCode, 1);
                return true;
            }

            return false; // if we don't have pricing for the product then the scan should not be successful
        }

        public decimal CalculateTotal() {
            decimal total = 0;

            foreach (KeyValuePair<string, int> item in basket) {
                PricingModel model = allPrices.FirstOrDefault(p => p.ProductCode == item.Key);
                if (model.HasVolumePricing) {
                    int volumeQuantities = item.Value / (model.VolumePricingQuantity ?? 1);
                    int additionals = item.Value % (model.VolumePricingQuantity ?? 1);
                    total += model.VolumePricing * volumeQuantities ?? 0;
                    total += model.UnitPrice * additionals;
                } else {
                    total += model.UnitPrice * item.Value;
                }
            }

            return total;
        }
    }
}

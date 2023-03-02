using PointOfSaleTerminal.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace PointOfSaleTerminal.UnitTests
{
    public class Tests
    {

        [InlineData("A", 1.25)]
        [InlineData("B", 4.25)]
        [InlineData("C", 1.0)]
        [InlineData("D", 0.75)]
        [Theory]
        public void ScanSingleItem_ShouldCalculateTotalCorrectly(string productCode, decimal unitPrice) {
            // Arrange
            PointOfSaleTerminal terminal = new PointOfSaleTerminal();
            _SetPricing(terminal);

            // Act
            bool success = terminal.TryScanProduct(productCode);
            decimal total = terminal.CalculateTotal();

            // Assert
            Assert.True(success, $"Not successfull scanning product {productCode}");
            Assert.Equal(unitPrice, total);
        }

        [InlineData("A", 3, 3.0)]
        [InlineData("C", 6, 5.0)]
        [Theory]
        public void ScanMultipleOfSameItem_ShouldCalculateTotalCorrectly(string productCode, int quantity, decimal volumePrice) {
            // Arrange
            PointOfSaleTerminal terminal = new PointOfSaleTerminal();
            _SetPricing(terminal);

            // Act
            bool success = true;
            for (int i = 0; i < quantity; i++) {
                success &= terminal.TryScanProduct(productCode);
            }
            decimal total = terminal.CalculateTotal();

            // Assert
            Assert.True(success, $"Not successfull scanning product {productCode}");
            Assert.Equal(volumePrice, total);
        }

        [InlineData("Z")]
        [InlineData("")]
        [Theory]
        public void ScanInvalidProductCode_ShouldFailToScan(string productCode) {
            // Arrange
            PointOfSaleTerminal terminal = new PointOfSaleTerminal();
            _SetPricing(terminal);

            // Act
            bool success = terminal.TryScanProduct(productCode);

            // Assert
            Assert.False(success, $"Successfully scanned an invalid product {productCode}");
        }

        [InlineData("ABCDABA", 13.25)] // volume pricing for A
        [InlineData("CCCCCCC", 6.0)] // volume pricing with extra for C
        [InlineData("ABCD", 7.25)] // each single price
        [Theory]
        public void ScanMultipleItems_ShouldCalculateTotalCorrectly(string productCodes, decimal totalPrice) {
            // Arrange
            PointOfSaleTerminal terminal = new PointOfSaleTerminal();
            _SetPricing(terminal);

            // Act
            bool success = true;
            foreach (char productCode in productCodes.AsSpan()) {
                success &= terminal.TryScanProduct(productCode.ToString());
            }
            decimal total = terminal.CalculateTotal();

            // Assert
            Assert.True(success, $"Not successfull scanning one or more products {productCodes}");
            Assert.Equal(totalPrice, total);
        }

        [Fact]
        public void SetDuplicatePricing_ShouldThrowError() {
            // Arrange
            PointOfSaleTerminal terminal = new PointOfSaleTerminal();
            List<PricingModel> duplicatedPrices = new List<PricingModel> {
                new PricingModel("A", (decimal)1.25, 3, (decimal)3.0),
                new PricingModel("B", (decimal)4.25),
                new PricingModel("C", (decimal)1.0, 6, (decimal)5.0),
                new PricingModel("D", (decimal)0.75),
                new PricingModel("A", (decimal)1.25, 3, (decimal)3.0),
                new PricingModel("B", (decimal)4.25),
                new PricingModel("C", (decimal)1.0, 6, (decimal)5.0),
                new PricingModel("D", (decimal)0.75)
            };

            // Act

            // Assert
            Assert.Throws<InvalidOperationException>(() => terminal.SetPricing(duplicatedPrices));
        }

        private static void _SetPricing(PointOfSaleTerminal terminal) {
            List<PricingModel> allPrices = new List<PricingModel> {
                new PricingModel("A", (decimal)1.25, 3, (decimal)3.0),
                new PricingModel("B", (decimal)4.25),
                new PricingModel("C", (decimal)1.0, 6, (decimal)5.0),
                new PricingModel("D", (decimal)0.75)
            };
            terminal.SetPricing(allPrices);
        }
    }
}
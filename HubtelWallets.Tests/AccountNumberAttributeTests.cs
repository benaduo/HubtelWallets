using HubtelWallets.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace HubtelWallets.Tests;
public class AccountNumberAttributeTests
{
    private class TestModel
    {
        [AccountNumber("Type", "Scheme")]
        public string AccountNumber { get; set; }
        public string Type { get; set; }
        public string Scheme { get; set; }
    }

    private ValidationContext CreateValidationContext(TestModel model)
    {
        return new ValidationContext(model, null, null);
    }

    [Theory]
    [InlineData("0241234567", "momo", "mtn", true)]
    [InlineData("0201234567", "momo", "vodafone", true)]
    [InlineData("0261234567", "momo", "airteltigo", true)]
    [InlineData("1234567890", "momo", "mtn", false)] // Invalid MTN prefix
    [InlineData("4111111111111111", "card", "visa", true)]
    [InlineData("5500000000000004", "card", "mastercard", true)]
    [InlineData("4111111111111", "card", "visa", false)] // Invalid length for Visa card
    [InlineData("5500000000000004", "card", "amex", false)] // Invalid scheme for card type
    [InlineData("1234567890123456", "card", "visa", false)] // Invalid Visa card number
    [InlineData("1234567890123456", "unknown", "visa", false)] // Invalid type
    public void AccountNumber_Validation(string accountNumber, string type, string scheme, bool expectedIsValid)
    {
        // Arrange
        var model = new TestModel
        {
            AccountNumber = accountNumber,
            Type = type,
            Scheme = scheme
        };
        var context = CreateValidationContext(model);
        var attribute = new AccountNumberAttribute("Type", "Scheme");

        // Act
        var result = attribute.GetValidationResult(model.AccountNumber, context);

        // Assert
        if (expectedIsValid)
        {
            Assert.Equal(ValidationResult.Success, result);
        }
        else
        {
            Assert.NotEqual(ValidationResult.Success, result);
        }
    }
}

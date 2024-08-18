using HubtelWallets.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace HubtelWallets.Tests;
public class PhoneNumberAttributeTests
{
    private class TestModel
    {
        [PhoneNumber]
        public string PhoneNumber { get; set; }
    }

    private ValidationContext CreateValidationContext(TestModel model)
    {
        return new ValidationContext(model, null, null);
    }

    [Theory]
    [InlineData("0241234567", true)] // Valid phone number
    [InlineData("0501234567", true)] // Valid phone number
    [InlineData("1234567890", false)] // Invalid phone number (does not start with 0)
    [InlineData("024123456", false)] // Invalid phone number (too short)
    [InlineData("02412345678", false)] // Invalid phone number (too long)
    [InlineData("024123456a", false)] // Invalid phone number (contains non-digit character)
    [InlineData(null, false)] // Null phone number
    [InlineData("", false)] // Empty phone number
    public void PhoneNumber_Validation(string phoneNumber, bool expectedIsValid)
    {
        // Arrange
        var model = new TestModel
        {
            PhoneNumber = phoneNumber
        };
        var context = CreateValidationContext(model);
        var attribute = new PhoneNumberAttribute();

        // Act
        var result = attribute.GetValidationResult(model.PhoneNumber, context);

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

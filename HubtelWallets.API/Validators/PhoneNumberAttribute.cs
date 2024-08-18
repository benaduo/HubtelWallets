using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HubtelWallets.API.Validators;

public class PhoneNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null || !(value is string))
            return new ValidationResult(ErrorMessage ?? "Invalid phone number format.");

        var phoneNumber = value as string ?? "";
        if (!Regex.IsMatch(phoneNumber, @"^0\d{9}$"))
            return new ValidationResult(ErrorMessage ?? "Invalid phone number format.");

        return ValidationResult.Success;
    }
}

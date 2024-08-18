using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HubtelWallets.API.Validators;

public class AccountNumberAttribute : ValidationAttribute
{
    private readonly string _typePropertyName;
    private readonly string _schemePropertyName;

    public AccountNumberAttribute(string typePropertyName, string schemePropertyName)
    {
        _typePropertyName = typePropertyName;
        _schemePropertyName = schemePropertyName;
    }
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var typeProperty = validationContext.ObjectType.GetProperty(_typePropertyName);
        var schemeProperty = validationContext.ObjectType.GetProperty(_schemePropertyName);

        if (typeProperty is null || schemeProperty is null)
            return new ValidationResult($"Unknown property: {_typePropertyName} or {_schemePropertyName}");

        var typeValue = typeProperty.GetValue(validationContext.ObjectInstance, null) as string;
        var schemeValue = schemeProperty.GetValue(validationContext.ObjectInstance, null) as string;

        if (typeValue is null || schemeValue is null)
            return new ValidationResult($"Type and Account Scheme are required.");

        if (value is null || !(value is string accountNumber))
            return new ValidationResult(ErrorMessage ?? "Invalid account number format.");

        if (typeValue.Equals("momo", StringComparison.OrdinalIgnoreCase))
        {
            if (!Regex.IsMatch(accountNumber, @"^\d{10}$"))
                return new ValidationResult(ErrorMessage ?? "For 'momo' type, account number must be exactly 10 digits.");
            if (schemeValue.Equals("mtn", StringComparison.OrdinalIgnoreCase))
            {
                if (!Regex.IsMatch(accountNumber, @"^(024|025|053|054|055|059)\d{7}$"))
                {
                    return new ValidationResult("Invalid 'mtn' number entered.");
                }
            }
            else if (schemeValue.Equals("vodafone", StringComparison.OrdinalIgnoreCase))
            {
                if (!Regex.IsMatch(accountNumber, @"^(020|050)\d{7}$"))
                {
                    return new ValidationResult("Invalid 'vodafone' number entered.");
                }
            }
            else if (schemeValue.Equals("airteltigo", StringComparison.OrdinalIgnoreCase))
            {
                if (!Regex.IsMatch(accountNumber, @"^(026|056|027|057)\d{7}$"))
                {
                    return new ValidationResult("Invalid 'airteltigo' number entered.");
                }
            }
            else
            {
                return new ValidationResult("Invalid account scheme for 'momo' type.");
            }
        }
        else if (typeValue.Equals("card", StringComparison.OrdinalIgnoreCase))
        {
            if (!Regex.IsMatch(accountNumber, @"^\d{16}$"))
                return new ValidationResult(ErrorMessage ?? "For 'card' type, account number must be exactly 16 digits.");

            if (schemeValue.Equals("visa", StringComparison.OrdinalIgnoreCase))
            {
                if (!accountNumber.StartsWith("4"))
                {
                    return new ValidationResult("Invalid Visa card number.");
                }
            }
            else if (schemeValue.Equals("mastercard", StringComparison.OrdinalIgnoreCase))
            {
                if (!(accountNumber.StartsWith("51") || accountNumber.StartsWith("52") ||
                      accountNumber.StartsWith("53") || accountNumber.StartsWith("54") ||
                      accountNumber.StartsWith("55") ||
                      (accountNumber.CompareTo("2221000000000000") >= 0 &&
                       accountNumber.CompareTo("2720999999999999") <= 0)))
                {
                    return new ValidationResult("Invalid Mastercard number.");
                }
            }
            else
            {
                return new ValidationResult("Invalid account scheme for 'card' type. Allowed values are 'visa' or 'mastercard'.");
            }
        }
        else
        {
            return new ValidationResult(ErrorMessage ?? "Invalid type provided.");
        }

        return ValidationResult.Success;
    }
}


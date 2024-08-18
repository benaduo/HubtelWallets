using HubtelWallets.API.Helpers;
using HubtelWallets.API.Validators;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HubtelWallets.API.DTOs;

public record WalletDto(
    [ModelBinder(BinderType = typeof(TrimModelBinder))]
    [Required]
    [Length(3,30)] string Name,
    [ModelBinder(BinderType = typeof(TrimModelBinder))]
    [Required]
    [AccountNumber("Type","AccountScheme")]
    string AccountNumber,
    [ModelBinder(BinderType = typeof(TrimModelBinder))]
    [Required]
    string AccountScheme,
    [ModelBinder(BinderType = typeof(TrimModelBinder))]
    [Required]
    string Type,
    [ModelBinder(BinderType = typeof(TrimModelBinder))]
    [Required]
    [PhoneNumber]
    string Owner);


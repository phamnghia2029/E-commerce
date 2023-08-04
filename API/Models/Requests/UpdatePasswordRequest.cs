using API.Entities;
using API.Repositories;
using API.Utils;
using FluentValidation;

namespace API.Models.Requests;

public class UpdatePasswordRequest: BaseRequest
{
    public int? AccountId { get; set; }
    public string? OldPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
}

public class UpdatePasswordRequestValidator : BaseValidator<UpdatePasswordRequest>
{
    private readonly AccountRepository _accountRepository;

    public UpdatePasswordRequestValidator(AccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    protected override void AddRules(UpdatePasswordRequest request)
    {
        Account account = _accountRepository.GetOne(request.AccountId ?? -1);
        
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Please fill current password.")
            .Must(email => Encryptions.Match(request.OldPassword, account.Password))
            .WithMessage("Wrong password.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Please fill new password.")
            .Must(password => password.Length >= 8 && password.Length <= 32)
            .WithMessage("Password must have between 8-32 characters.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Please fill confirm password.")
            .Must(x => x.Length >= 8 && x.Length <= 32)
            .WithMessage("Password must have between 8-32 characters.")
            .Must(confirmPassword => Strings.IsEquals(confirmPassword, request.NewPassword))
            .WithMessage("Confirm password must be the same with new password.");

    }
}
using API.Repositories;
using FluentValidation;

namespace API.Models.Requests;

public class UpdateProfileRequest: BaseRequest
{
    public int? AccountId { get; set; }
    public string? Email { get; set; }
    public string? CompanyName { get; set; }
    public string? ContactName { get; set; }
    public string? ContactTitle { get; set; }
    public string? Address { get; set; }
}

public class UpdateProfileRequestValidator : BaseValidator<UpdateProfileRequest>
{
    private readonly AccountRepository _accountRepository;

    public UpdateProfileRequestValidator(AccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    protected override void AddRules(UpdateProfileRequest request)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Please fill email.")
            .Must(email => _IsChangedPassword(request))
            .WithMessage("Email already existed.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Please fill company name.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Please fill address.");

        RuleFor(x => x.ContactName)
            .NotEmpty().WithMessage("Please fill contact name.");

        RuleFor(x => x.ContactTitle)
            .NotEmpty().WithMessage("Please fill contact title.");

    }

    private bool _IsChangedPassword(UpdateProfileRequest request)
    {
        var account = _accountRepository.GetOne(request.AccountId ?? -1);
        bool passwordUnchanged = account.Email.Trim().Equals(request.Email.Trim());
        if (passwordUnchanged)
        {
            return true;
        }
        
        return _accountRepository.FindByEmail(request.Email?.Trim()) == null;
    }
}
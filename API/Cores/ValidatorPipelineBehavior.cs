using API.Models.Exceptions;
using FluentValidation;
using MediatR;

namespace API.Cores;

    public class ValidatorPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
    private readonly IEnumerable<IValidator<TRequest>> _validators;
 
    public ValidatorPipelineBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var failures = _validators
            .Select(validator => validator.Validate(request))
            .SelectMany(result => result.Errors)
            .ToArray();
 
        if (failures.Length > 0)
        {
            // Map the validation failures and throw an error,
            // this stops the execution of the request
            
            Dictionary<String, String[]> errors = failures
                .GroupBy(x => x.PropertyName)
                .ToDictionary(k => k.Key, v => v.Select(x => x.ErrorMessage).ToArray());
            throw new InputValidationException(errors);
        }
 
        // Invoke the next handler
        // (can be another pipeline behavior or the request handler)
        return next();
    }
}
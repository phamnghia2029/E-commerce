using System.Reflection;
using FluentValidation;
using FluentValidation.Results;

namespace API.Models.Requests;

public abstract class BaseValidator<T> : AbstractValidator<T>
{
    protected abstract void AddRules(T request);
    
    public override ValidationResult Validate(ValidationContext<T> context)
    {
        T request = context.InstanceToValidate;
        PropertyInfo[] propertyInfos = request.GetType().GetProperties();
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
            try
            {
                if (propertyInfo.PropertyType == typeof(string))
                {
                    if (propertyInfo.GetValue(request) == null)
                    {
                        propertyInfo.SetValue(request, "");
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        AddRules(request);
        return base.Validate(context);
    }
}
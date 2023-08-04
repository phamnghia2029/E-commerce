using API.Repositories;
using FluentValidation;

namespace API.Models.Requests.Post;

public class NewProductRequest: BaseRequest
{
    public string ProductName { get; set; }
    public int CategoryId { get; set; }
    public string? QuantityPerUnit { get; set; }
    public decimal UnitPrice { get; set; }
    public int UnitsInStock { get; set; }

    public bool Discontinued { get; set; } = false;

}    

public class NewProductRequestValidator : BaseValidator<NewProductRequest>
{
    private readonly CategoryRepository _categoryRepository;

    public NewProductRequestValidator(CategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    protected override void AddRules(NewProductRequest request)
    {
        RuleFor(x => x.ProductName).MinimumLength(10).WithMessage("Tên sản phẩm cần dài hơn 10 kí tự");
        RuleFor(x => x.CategoryId).Must(categoryId => _categoryRepository.IsExists(categoryId)).WithMessage("Không tìm thấy danh mục.");
        RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("Giá cần lớn hơn 0");
        RuleFor(x => x.UnitsInStock).GreaterThan(0).WithMessage("Số lương sản phẩm phải lớn hơn 0");
    }

}
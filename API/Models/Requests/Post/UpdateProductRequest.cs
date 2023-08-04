using API.Repositories;
using FluentValidation;

namespace API.Models.Requests.Post;

public class UpdateProductRequest: BaseRequest
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; } = null!;
    public int? CategoryId { get; set; }
    public string? QuantityPerUnit { get; set; }
    public decimal? UnitPrice { get; set; }
    public int? UnitsInStock { get; set; }
    public bool Discontinued { get; set; }

}

public class UpdateProductRequestValidator : BaseValidator<UpdateProductRequest>
{
    private readonly CategoryRepository _categoryRepository;

    public UpdateProductRequestValidator(CategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    protected override void AddRules(UpdateProductRequest request)
    {
        RuleFor(x => x.ProductName).MinimumLength(10).WithMessage("Tên sản phẩm cần dài hơn 10 kí tự").When(x => x.ProductName != null);
        RuleFor(x => x.CategoryId).Must(categoryId => _categoryRepository.IsExists(categoryId ?? -1)).WithMessage("Không tìm thấy danh mục.").When(x => x.CategoryId != null);
        RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("Giá cần lớn hơn 0").When(x => x.UnitPrice != null);
        RuleFor(x => x.UnitsInStock).GreaterThan(0).WithMessage("Số lương sản phẩm phải lớn hơn 0").When(x => x.UnitsInStock != null);
    }

}
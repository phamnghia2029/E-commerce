using API.Entities;
using API.Models.Domain;
using API.Models.Exceptions;
using API.Models.Requests.Post;
using API.Models.Responses;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController
{
    private readonly ProductRepository _productRepository;
    private readonly OrderDetailRepository orderDetailRepository;


    private readonly IMapper _mapper;

    public ProductController(ProductRepository productRepository, IMapper mapper, OrderDetailRepository orderDetailRepository)
    {
        _productRepository = productRepository;
        this.orderDetailRepository = orderDetailRepository;

        _mapper = mapper;
    }

    [HttpGet]
    [Route("")]
    public ListResult<ProductResponse> FindAll(int? categoryId, int page = 1, int size = 10, bool isAscending = false, String orderBy = "ProductId")
    {
        ListResult<Product> listResult = _productRepository.Search(page, size, isAscending, orderBy, whereClause: x => categoryId == null || x.CategoryId == categoryId);
        List<ProductResponse> responses = listResult.Content.Select(product => _ProductResponseOf(product)).ToList();
        return listResult.WithContent(responses);
    }

    [HttpGet]
    [Route("AdminProduct")]
    public ListResult<ProductResponse> FindAllWithProductName(int? categoryId, String? productName, int page = 1, int size = 10, bool isAscending = false, String orderBy = "ProductId")
    {
        ListResult<Product> listResult = _productRepository.Search(page, size, isAscending, orderBy, whereClause: x => (categoryId == null || x.CategoryId == categoryId || categoryId == 0)
                 && (string.IsNullOrWhiteSpace(productName) || x.ProductName.ToLower().Contains(productName.ToLower())));
        List<ProductResponse> responses = listResult.Content.Select(product => _ProductResponseOf(product)).ToList();
        return listResult.WithContent(responses);
    }

    [HttpGet]
    [Route("Best")]
    public List<ProductResponse> GetBestSeller(int total = 4)
    {
        return _productRepository.FindTopSellers(total).Select(product => _ProductResponseOf(product)).ToList();
    }

    [HttpGet]
    [Route("Newest")]
    public List<ProductResponse> GetTopNewest(int total = 4)
    {
        return _productRepository.FindTopNewest(total).Select(product => _ProductResponseOf(product)).ToList();
    }

    private ProductResponse _ProductResponseOf(Product product)
    {
        ProductResponse response = _mapper.Map<ProductResponse>(product);
        response.Category = _mapper.Map<CategoryResponse>(product.Category);
        return response;
    }

    private ExportExcelProductResponse _ExportExceProductResponseOf(Product product)
    {
        ExportExcelProductResponse response = _mapper.Map<ExportExcelProductResponse>(product);
        return response;
    }

    [HttpGet]
    [Route("{id}")]
    public ProductResponse FindProduct(int id)
    {
        Product product = _productRepository.GetOne(id, "Không tìm thấy sản phẩm.");
        return _ProductResponseOf(product);
    }
    [HttpGet]
    [Route("all")]
    public List<ExportExcelProductResponse> FindAllProduct()
    {
        return _productRepository.FindAll().Select(x => _ExportExceProductResponseOf(x)).ToList();
    }

    [HttpPost]
    [Route("")]
    public ProductResponse AddProduct(NewProductRequest request)
    {
        Product product = _mapper.Map<Product>(request);
        Product createdProduct = _productRepository.Add(product);

        return _ProductResponseOf(createdProduct);
    }

    [HttpPost]
    [Route("AddRange")]
    public List<ProductResponse> AddRangeProduct(List<NewProductRequest> request)
    {
        List<Product> products = _mapper.Map<List<Product>>(request);
        List<Product> createdProducts = _productRepository.AddAll(products);
        return createdProducts.Select(x => _ProductResponseOf(x)).ToList();
    }

    [HttpPut]
    [Route("{id}")]
    public ProductResponse UpdateProduct(int id, UpdateProductRequest request)
    {
        Product product = _productRepository.GetOne(id, "Không tìm thấy sản phẩm.");
        if (request.ProductName != null)
        {
            product.ProductName = request.ProductName;
        }
        if (request.CategoryId != null)
        {
            product.CategoryId = request.CategoryId;
        }
        if (request.QuantityPerUnit != null)
        {
            product.QuantityPerUnit = request.QuantityPerUnit;
        }
        if (request.UnitPrice != null)
        {
            product.UnitPrice = request.UnitPrice;
        }
        if (request.UnitsInStock != null)
        {
            product.UnitsInStock = (short?)request.UnitsInStock;
        }
        if (request.Discontinued != null)
        {
            product.Discontinued = request.Discontinued;
        }
        Product updateProduct = _productRepository.Update(product);

        return _ProductResponseOf(updateProduct);
    }

    [HttpDelete]
    [Route("{id}")]
    public void DeleteProduct(int id)
    {
        Product product = _productRepository.GetOne(id, "Không tìm thấy sản phẩm.");
        List<OrderDetail> orderDetails = orderDetailRepository.FindAll(x => x.ProductId.Equals(product.ProductId)).ToList();
        if (orderDetails.Count() != 0)
        {
            throw ApiException.BadRequest("không thể xóa sản phẩm vì sản phẩm có trong order detail");
        }
        _productRepository.Delete(product);
        //return "Đã xóa sản phẩm thành công.";
    }
}
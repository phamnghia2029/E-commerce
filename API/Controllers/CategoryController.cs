using API.Entities;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController
{
    private readonly CategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryController(IMapper mapper, CategoryRepository categoryRepository)
    {
        _mapper = mapper;
        _categoryRepository = categoryRepository;
    }
    
    [HttpGet]
    [Route("")]
    public List<Category> GetAll()
    {
        return _categoryRepository.FindAll();
    }
}
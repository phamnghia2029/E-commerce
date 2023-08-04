using API.Cores;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using API.Models.Exceptions;
using API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PRN221_DBContext>();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddTransient<AccountRepository>();
builder.Services.AddTransient<CategoryRepository>();
builder.Services.AddTransient<CustomerRepository>();
builder.Services.AddTransient<EmployeeRepository>();
builder.Services.AddTransient<DepartmentRepository>();
builder.Services.AddTransient<OrderDetailRepository>();
builder.Services.AddTransient<OrderRepository>();
builder.Services.AddTransient<ProductRepository>();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddMediatR(typeof(Program));

// Register custom pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));

// Register all Fluent Validators
builder.Services .AddMvc() .AddFluentValidation(s => s.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.Configure<ApiBehaviorOptions>(options => {
    options.InvalidModelStateResponseFactory = context => {
        ValidationProblemDetails problemDetails = new ValidationProblemDetails(context.ModelState);
        throw new InputValidationException(problemDetails.Errors);
    };
});
var app = builder.Build();
app .UseMiddleware<ExceptionHandler>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
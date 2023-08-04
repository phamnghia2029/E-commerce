using API.Entities;
using API.Models.Domain;
using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Configuration;

namespace Client.Pages.Admin.Product
{
    public class IndexModel : ClientModel
    {
        [BindProperty]
        public ListResult<ProductResponse> ListProducts { get; set; }
        [BindProperty]
        public List<Category> Categories { get; set; }
        public async Task<IActionResult> OnGet(int CategoryId, String ProductName, int currentPage = 1, int size = 12, bool asc = true, string orderBy = "ProductId")
        {
            if (!IsAdmin())
            {
                return ToForbiddenPage();
            }
            ViewData["CategoryId"] = CategoryId;
            ViewData["ProductName"] = ProductName;
            Categories = CallGet<List<Category>>($"https://localhost:7176/Category");
            ListProducts = CallGet<ListResult<ProductResponse>>($"https://localhost:7176/Product/AdminProduct?page={currentPage}&size={size}&isAscending{asc}&orderBy={orderBy}&categoryId={CategoryId}&productName={ProductName}");
            return Page();
        }

        public async Task<IActionResult> OnPostImport(IFormFile file)
        {
            var listImport = new List<ProductResponse>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowcount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowcount; row++)
                    {
                        listImport.Add(new ProductResponse
                        {
                            ProductName = worksheet.Cells[row, 1].Value.ToString().Trim(),
                            UnitPrice = Decimal.Parse(worksheet.Cells[row, 2].Value.ToString()),
                            QuantityPerUnit = worksheet.Cells[row, 3].Value.ToString().Trim(),
                            UnitsInStock = short.Parse(worksheet.Cells[row, 4].Value.ToString()),
                            CategoryId = int.Parse(worksheet.Cells[row, 5].Value.ToString()),
                            Discontinued = bool.Parse(worksheet.Cells[row, 6].Value.ToString())
                        });
                    }
                    CallPost<List<ProductResponse>>($"https://localhost:7176/Product/AddRange", listImport);
                }
            }
            return RedirectToPage("/Admin/Product/Index");
        }

        public async Task<IActionResult> OnPostExport()
        {
            var listExport = CallGet<List<ExportExcelProductResponse>>($"https://localhost:7176/Product/all");
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells.LoadFromCollection(listExport, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = "ListProduct.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}

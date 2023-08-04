using API.Models.Domain;
using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Client.Pages.Admin.Order
{
    public class IndexModel : ClientModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [BindProperty]
        public ListResult<OrderResponse> ListOrders { get; set; }
        public async Task<IActionResult> OnGet(DateTime? startDate, DateTime? endDate, int currentPage = 1, int size = 12, bool asc = true, string orderBy = "OrderId")
        {
            if (!IsAdmin())
            {
                return ToForbiddenPage();
            }
            StartDate = startDate;
            EndDate = endDate;
            ListOrders = CallGet<ListResult<OrderResponse>>($"https://localhost:7176/Order/AdminOrder?page={currentPage}&size={size}&isAscending{asc}&orderBy={orderBy}&startDate={startDate}&endDate={endDate}");
            return Page();
        }

        public async Task<IActionResult> OnPost(DateTime? startDate, DateTime? endDate)
        {
            if (startDate > endDate)
            {
                TempData["errorDateExport"] = "Start date after end date";
                return RedirectToPage("/Admin/Order/Index");
            }
            var listExport = CallGet<List<ExportExcelOrderResponse>>($"https://localhost:7176/Order/byDate?startDate={startDate}&endDate={endDate}");
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                List<int> dateColumns = new List<int>();
                int datecolumn = 1;
                foreach (var PropertyInfo in listExport.FirstOrDefault().GetType().GetProperties())
                {
                    if (PropertyInfo.PropertyType == typeof(DateTime) || PropertyInfo.PropertyType == typeof(DateTime?))
                    {
                        dateColumns.Add(datecolumn);
                    }
                    datecolumn++;
                }
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells.LoadFromCollection(listExport, true);
                dateColumns.ForEach(item => worksheet.Column(item).Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss AM/PM");
                package.Save();
            }
            stream.Position = 0;
            string excelName = "ListOrder.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}

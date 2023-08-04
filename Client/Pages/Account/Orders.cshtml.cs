using System.Text;
using API.Models.Responses;
using Aspose.Pdf;
using Base.Functionals;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Pages.Account;

public class OrdersModel : ClientModel
{
    public AccountResponse Account { get; set; }
    public List<OrderResponse> Orders { get; set; }
    public IActionResult OnGet()
    {
        if (!HasLogin())
        {
            return ToForbiddenPage();
        }

        Account = GetCurrentUser();
        Orders = CallGet<List<OrderResponse>>($"https://localhost:7176/Order?customerId={Account.CustomerId}").OrderByDescending(x => x.OrderDate).ToList();
        return Page();
    }

    public IActionResult OnPostCancel(int OrderId)
    {
        if (!HasLogin())
        {
            return ToForbiddenPage();
        }

        return TakeAction(() =>
        {
            CallPatch<None>($"https://localhost:7176/Order/{OrderId}/Cancel");
            return RedirectToPage("/account/orders");
        });
    }

    public IActionResult OnPostConfirm(int OrderId)
    {
        if (!HasLogin())
        {
            return ToForbiddenPage();
        }

        return TakeAction(() =>
        {
            CallPatch<None>($"https://localhost:7176/Order/{OrderId}/Confirm");
            return RedirectToPage("/account/orders");
        });
    }

    public IActionResult OnPostExportPDF(int OrderId)
    {
        if (!HasLogin())
        {
            return ToForbiddenPage();
        }

        return TakeAction(() =>
        {
            OrderResponse order = CallGet<OrderResponse>($"https://localhost:7176/Order/{OrderId}");

            string body = GetBody(order.Customer, order, order.OrderDetails);
            {
                HtmlLoadOptions objLoadOptions = new HtmlLoadOptions();
                objLoadOptions.PageInfo.Margin.Bottom = 10;
                objLoadOptions.PageInfo.Margin.Top = 20;

                Document document = new Document(new MemoryStream(Encoding.UTF8.GetBytes(body)), objLoadOptions);

                using var stream = new MemoryStream();
                document.Save(stream);
                var pdf = new FileContentResult(stream.ToArray(), "application/pdf")
                {
                    FileDownloadName = "Order.pdf"
                };
                return pdf;
            }
            OnGet();
            return Page();
        });
    }

    private string GetBody(CustomerResponse? customer, OrderResponse order, List<OrderDetailResponse> products)
    {
        String content = "";
        AccountResponse? user = GetCurrentUser();
        for (int i = 0; i < products.Count; i++)
        {
            var product = products[i];
            content += $@"
            <tr>
                <td class='no' style='width: 100px'> {i} </td>
                <td class='desc' style='width: 200px'><h3>{product.Product.ProductName}</h3></td>
                <td class='unit' style='width: 200px'>{product.UnitPrice}</td>
                <td class='qty' style='width: 200px'>{product.Quantity}</td>
                <td class='total' style='width: 200px'>{product.UnitPrice * product.Quantity}</td>
            </tr>
            ";
        }
         return $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta http-equiv='X-UA-Compatible' content='IE=edge'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Index</title>
            <link href='wwwroot/css/invoice.css' rel='stylesheet' />
        </head>
        <body>
            <header class='clearfix'>
                <div id='logo'>
                    <img src='wwwroot/img/logo.png'>
                </div>
                <div id='company'>
                    <h2 class='name'>Company Name: {customer.CompanyName}</h2>
                    <div>Company Name: {customer.CompanyName}</div>
                    <div>Contact Title: {customer.ContactTitle}</div>
                    <div>Address: {customer.Address}</div>
                </div>
                </div>
            </header>
            <main>
                <div id='details' class='clearfix'>
                    <div id='client'>
                        <div class='to'>INVOICE TO: {user.Email}</div>
                    </div>
                    <div id='invoice'>
                        <h1>INVOICE NUMBER: {order.OrderId}</h1>
                        <div class='date'>Date of Invoice: {order.OrderDate}</div>
                    </div>
                </div>
                <table border='1px' cellspacing='0' cellpadding='0'>
                    <thead>
                        <tr>
                            <th class='no' style='width: 100px'>#</th>
                            <th class='desc' style='width: 200px'>Product Name</th>
                            <th class='unit' style='width: 200px'>Unit price</th>
                            <th class='qty' style='width: 200px'>Quantity</th>
                            <th class='total' style='width: 200px'>Total</th>
                        </tr>
                    </thead>
                    <tbody id='invoiceItems'>
                        {content}
                    </tbody>
                </table>
                <div id='thanks'>Thank you!</div>
            </main>
        </body>
        </html>
        ";
    }

}
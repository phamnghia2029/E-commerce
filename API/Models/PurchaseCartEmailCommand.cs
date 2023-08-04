using System.Net.Mail;
using System.Text;
using API.Entities;
using API.Utils;
using Aspose.Pdf;
using Microsoft.AspNetCore.Mvc;

namespace API.Models;

public class PurchaseCartEmailCommand
{
    public Order Order { get; set; }
    public Customer Customer { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }
    public string email { get; set; }

    public void Execute()
    {
        FileContentResult pdf = CreatePdfFileToSend();
        var attachmentStream = new MemoryStream(pdf.FileContents);
        var attachmentTitle = pdf.FileDownloadName;

        Attachment attachment = new Attachment(attachmentStream, attachmentTitle);
        Mails.SendTo(email, $"Your Purchased Order #{Order.OrderId}", _GetConfirmMailBody(), Collections.ListOf(attachment));
    }

    private FileContentResult CreatePdfFileToSend()
    {
        string body = GetBody();
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
    }

    private string _GetConfirmMailBody()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<h3>Thank you for your purchase!</h3>");
        sb.Append("<p>Hi " + Customer.ContactName +
                  ", We have received your item and are ready to ship. We will notify you when your order has been dispatched.</p>");
        sb.Append("</br>");
        sb.Append("<h4>Order information</h4>");
        sb.Append("<table>");
        sb.Append("<tr>");
        sb.Append("<td>ProductName</td>");
        sb.Append("<td>Quantity</td>");
        sb.Append("<td>Price</td>");
        sb.Append("</tr>");
        foreach (var item in OrderDetails)
        {
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append(item.Product.ProductName);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append(item.Quantity);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append(item.Quantity * item.Product.UnitPrice);
            sb.Append("</td>");
            sb.Append("</tr>");
        }

        sb.Append("</table>");
        sb.Append("</br>");
        sb.Append("<h4>Customer information</h4>");
        sb.Append("<table>");
        sb.Append("<tr>");
        sb.Append("<td>Company Name:</td>");
        sb.Append("<td>" + Customer.CompanyName + "</td>");
        sb.Append("</tr>");
        sb.Append("<tr>");
        sb.Append("<td>Contact Name:</td>");
        sb.Append("<td>" + Customer.ContactName + "</td>");
        sb.Append("</tr>");
        sb.Append("<td>Contact Title:</td>");
        sb.Append("<td>" + Customer.ContactTitle + "</td>");
        sb.Append("</tr>");
        sb.Append("<tr>");
        sb.Append("<td>Address:</td>");
        sb.Append("<td>" + Customer.Address + "</td>");
        sb.Append("</tr>");
        sb.Append("</table>");
        sb.Append("<div></div>");
        sb.Append("If you have any questions, don't hesitate to contact us at cskh@gmail.com");
        StringReader sr = new StringReader(sb.ToString());
        return sr.ReadToEnd();
    }

    private string GetBody()
    {
        String content = "";
        for (int i = 0; i < OrderDetails.Count; i++)
        {
            var product = OrderDetails[i];
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
                    <h2 class='name'>Company Name: {Customer.CompanyName}</h2>
                    <div>Company Name: {Customer.CompanyName}</div>
                    <div>Contact Title: {Customer.ContactTitle}</div>
                    <div>Address: {Customer.Address}</div>
                </div>
                </div>
            </header>
            <main>
                <div id='details' class='clearfix'>
                    <div id='client'>
                        <div class='to'>INVOICE TO: {email}</div>
                    </div>
                    <div id='invoice'>
                        <h1>INVOICE NUMBER: {Order.OrderId}</h1>
                        <div class='date'>Date of Invoice: {Order.OrderDate}</div>
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
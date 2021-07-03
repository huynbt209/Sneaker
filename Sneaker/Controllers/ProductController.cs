using Microsoft.AspNetCore.Mvc;
using Sneaker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Sneaker.Repository.Interface;
using Microsoft.Extensions.Logging;
using Sneaker.ViewModel;
using System.Threading.Tasks;

namespace Sneaker.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepo _productRepo;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductRepo productRepo, ILogger<ProductController> logger)
        {
            _productRepo = productRepo;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var products = GetProductList();
            return View(products);
        }

        public IActionResult ExportToExcel()
        {
            var products = GetProductList();
            var stream = new MemoryStream();

            using (var xlPackage = new ExcelPackage(stream))
            {
                // Define a worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Product");

                // Styling
                var customStyle = xlPackage.Workbook.Styles.CreateNamedStyle("CustomStyle");
                customStyle.Style.Font.UnderLine = true;
                customStyle.Style.Font.Color.SetColor(Color.Red);

                // First row
                var startRow = 5;
                var row = startRow;

                worksheet.Cells["A1"].Value = "Sample Product Export";
                using (var r = worksheet.Cells["A1:N1"])
                {
                    r.Merge = true;
                    r.Style.Font.Color.SetColor(Color.Green);
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(23, 55, 93));
                }

                worksheet.Cells["A4"].Value = "Id";
                worksheet.Cells["B4"].Value = "Title";
                worksheet.Cells["C4"].Value = "TitleURL";
                worksheet.Cells["D4"].Value = "ProductName";
                worksheet.Cells["E4"].Value = "Image";
                worksheet.Cells["F4"].Value = "Image1";
                worksheet.Cells["G4"].Value = "Quantity";
                worksheet.Cells["H4"].Value = "Price";
                worksheet.Cells["I4"].Value = "Badge";
                worksheet.Cells["J4"].Value = "Category";
                worksheet.Cells["K4"].Value = "ProductCard";
                worksheet.Cells["L4"].Value = "Status";
                worksheet.Cells["M4"].Value = "StatusMessage";
                worksheet.Cells["N4"].Value = "ChangeStatusBy";
                //14
                worksheet.Cells["A4:N4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells["A4:N4"].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                row = 15;
                foreach (var product in products)
                {
                    worksheet.Cells[row, 1].Value = product.Id;
                    worksheet.Cells[row, 2].Value = product.Title;
                    worksheet.Cells[row, 3].Value = product.Title;
                    worksheet.Cells[row, 3].Value = product.ProductName;
                    worksheet.Cells[row, 3].Value = product.Image;
                    worksheet.Cells[row, 3].Value = product.Image1;
                    worksheet.Cells[row, 3].Value = product.Quantity;
                    worksheet.Cells[row, 3].Value = product.Price;
                    worksheet.Cells[row, 3].Value = product.Badge;
                    worksheet.Cells[row, 3].Value = product.Category;
                    worksheet.Cells[row, 3].Value = product.ProductCard;
                    worksheet.Cells[row, 3].Value = product.Status;
                    worksheet.Cells[row, 3].Value = product.StatusMessage;
                    worksheet.Cells[row, 3].Value = product.ChangeStatusBy;

                    row++; // row = row + 1;
                }

                xlPackage.Workbook.Properties.Title = "Product list";
                xlPackage.Workbook.Properties.Author = "Huynbt";

                xlPackage.Save();
            }

            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Product.xlsx");
        }

        //

        [HttpGet]
        public IActionResult ImportProduct()
        {
            var viewModel = _productRepo.ProductTrademarkViewModel();
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ImportProduct(IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file?.Length > 0)
                {
                    // convert to a stream
                    var stream = file.OpenReadStream();

                    List<Product> products = new List<Product>();

                    try
                    {
                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets.First();
                            var rowCount = worksheet.Dimension.Rows;

                            for (var row = 2; row <= rowCount; row++)
                            {
                                try
                                {
                                    var id = int.Parse(worksheet.Cells[row, 1].Value?.ToString().Trim());
                                    var title = worksheet.Cells[row, 2].Value?.ToString().Trim();
                                    var titleURL = worksheet.Cells[row, 3].Value?.ToString().Trim();
                                    var productName = worksheet.Cells[row, 4].Value?.ToString().Trim();
                                    var image = worksheet.Cells[row, 5].Value?.ToString().Trim();
                                    var image1 = worksheet.Cells[row, 6].Value?.ToString().Trim();
                                    var quantity = int.Parse(worksheet.Cells[row, 7].Value?.ToString().Trim());
                                    var price = int.Parse(worksheet.Cells[row, 8].Value?.ToString().Trim());
                                    var badge = worksheet.Cells[row, 9].Value?.ToString().Trim();
                                    var category = worksheet.Cells[row, 10].Value?.ToString().Trim();
                                    var productCard = worksheet.Cells[row, 11].Value?.ToString().Trim();
                                    var status = bool.Parse(worksheet.Cells[row, 12].Value?.ToString().Trim());
                                    var statusMessage = worksheet.Cells[row, 13].Value?.ToString().Trim();
                                    var changeStatusBy = worksheet.Cells[row, 14].Value?.ToString().Trim();

                                    var product = new Product()
                                    {
                                        Id = id,
                                        Title = title,
                                        TitleURL = titleURL,
                                        ProductName = productName,
                                        Image = image,
                                        Image1 = image1,
                                        Quantity = quantity,
                                        Price = price,
                                        Badge = badge,
                                        Category = category,
                                        ProductCard = productCard,
                                        Status = status,
                                        StatusMessage = statusMessage,
                                        ChangeStatusBy = changeStatusBy
                                    };

                                    products.Add(product);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }

                        return View("Index", products);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return View("Index");
        }



        private List<Product> GetProductList()
        {
            var products = _productRepo.GetProducts();
            return (List<Product>)products;
        }
    }
}

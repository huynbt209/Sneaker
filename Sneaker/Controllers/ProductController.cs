﻿using Microsoft.AspNetCore.Mvc;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.OleDb;
using Microsoft.Data.SqlClient;
using System.Text;
using ExcelDataReader;
using System.Net.Http.Headers;

namespace Sneaker.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepo _productRepo;
        private readonly ILogger<ProductController> _logger;
        [Obsolete]
        private IHostingEnvironment _environment;
        private IConfiguration _configuration;

        [Obsolete]
        public ProductController(IProductRepo productRepo, ILogger<ProductController> logger, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _productRepo = productRepo;
            _logger = logger;
            _configuration = configuration;
            this._environment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View(_productRepo.GetTrademarks());
        }

        public IActionResult ListProducts(int id)
        {
            var listProducts = _productRepo.GetProductByTrademark(id);
            _logger.LogInformation("Display list products!");
            return View(listProducts);
        }

        public IActionResult GetListProducts(int id)
        {
            var listProducts = _productRepo.GetProductByTrademark(id);
            return new JsonResult(listProducts);
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
                customStyle.Style.Font.Color.SetColor(new Color( /* Color [Red] */));

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
                                    var trademarkId = int.Parse(worksheet.Cells[row, 15].Value?.ToString().Trim());

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
                                        ChangeStatusBy = changeStatusBy,
                                        TrademarkId = trademarkId
                                    };

                                    products.Add(product);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }

                        return View("ListProducts", products);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return View("ListProducts");
        }


        [HttpGet]
        public IActionResult CreateProduct()
        {
            var viewModel = _productRepo.ProductTrademarkViewModel();
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProduct(ProductTrademarkViewModel productTrademarkViewModel)
        {
            if (ModelState.IsValid)
            {
                _productRepo.CreateProduct(productTrademarkViewModel);
            }
            _logger.LogInformation("///");
            return RedirectToAction("Index");
        }


        private List<Product> GetProductList()
        {
            var products = _productRepo.GetProducts();
            return (List<Product>)products;
        }

        [HttpGet]
        public IActionResult Uploads()
        {
            return View();
        }
        ///////////////////
        [HttpPost]
        [Obsolete]
        public async Task<ActionResult> Uploads(IFormFile FormFile)
        {
            //get file name
            var filename = ContentDispositionHeaderValue.Parse(FormFile.ContentDisposition).FileName.Trim('"');

            //get path
            var MainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files");

            //create directory "Uploads" if it doesn't exists
            if (!Directory.Exists(MainPath))
            {
                Directory.CreateDirectory(MainPath);
            }

            //get file path 
            var filePath = Path.Combine(MainPath, FormFile.FileName);
            using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
            {
                await FormFile.CopyToAsync(stream);
            }

            //get extension
            string extension = Path.GetExtension(filename);


            string conString = string.Empty;

            switch (extension)
            {
                case ".xls": //Excel 97-03.
                    conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                    break;
                case ".xlsx": //Excel 07 and above.
                    conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                    break;
            }

            DataTable dt = new DataTable();
            conString = string.Format(conString, filePath);

            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;

                        //Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                        connExcel.Close();

                        //Read Data from First Sheet.
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        odaExcel.Fill(dt);
                        connExcel.Close();
                    }
                }
            }
            // database connection string
            conString = "Server=(localdb)\\mssqllocaldb;Database=aspnet-Sneaker-22B6C20C-1D81-45E0-9D4E-4BBB13A5B01A;Trusted_Connection=True;";

            using (SqlConnection con = new SqlConnection(conString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name.
                    sqlBulkCopy.DestinationTableName = "dbo.Products";

                    // Map the Excel columns with that of the database table, this is optional but good if you do
                    // 
                    sqlBulkCopy.ColumnMappings.Add("Id", "Id");
                    sqlBulkCopy.ColumnMappings.Add("Title", "Title");
                    sqlBulkCopy.ColumnMappings.Add("TitleURL", "TitleURL");
                    sqlBulkCopy.ColumnMappings.Add("ProductName", "ProductName");
                    sqlBulkCopy.ColumnMappings.Add("Image", "Image");
                    sqlBulkCopy.ColumnMappings.Add("Image1", "Image1");
                    sqlBulkCopy.ColumnMappings.Add("Quantity", "Quantity");
                    sqlBulkCopy.ColumnMappings.Add("Price", "Price");
                    sqlBulkCopy.ColumnMappings.Add("Badge", "Badge");
                    sqlBulkCopy.ColumnMappings.Add("Category", "Category");
                    sqlBulkCopy.ColumnMappings.Add("ProductCard", "ProductCard");
                    sqlBulkCopy.ColumnMappings.Add("Status", "Status");
                    sqlBulkCopy.ColumnMappings.Add("StatusMessage", "StatusMessage");
                    sqlBulkCopy.ColumnMappings.Add("ChangeStatusBy", "ChangeStatusBy");
                    sqlBulkCopy.ColumnMappings.Add("TrademarkId", "TrademarkId");


                    con.Open();
                    sqlBulkCopy.WriteToServer(dt);
                    con.Close();
                }
            }
            //if the code reach here means everthing goes fine and excel data is imported into database
            ViewBag.Message = "File Imported and excel data saved into database";

            return View("Index");
        }

    }
}

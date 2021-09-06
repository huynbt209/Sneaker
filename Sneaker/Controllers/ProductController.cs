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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.OleDb;
using Microsoft.Data.SqlClient;
using System.Text;
using ExcelDataReader;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks.Dataflow;

namespace Sneaker.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepo _productRepo;
        private readonly ILogger<ProductController> _logger;
        [Obsolete] private IHostingEnvironment _environment;
        private IConfiguration _configuration;
        private readonly ITrademarkRepo _trademarkRepo;
        private readonly IAdminRepo _adminRepo;
        private readonly IFeedbackProductRepo _feedbackProductRepo;

        [Obsolete]
        public ProductController(IProductRepo productRepo, ILogger<ProductController> logger,
            IHostingEnvironment hostingEnvironment, IConfiguration configuration,
            ITrademarkRepo trademarkRepo, IAdminRepo adminRepo, IFeedbackProductRepo feedbackProductRepo)
        {
            _productRepo = productRepo;
            _logger = logger;
            _configuration = configuration;
            this._environment = hostingEnvironment;
            _trademarkRepo = trademarkRepo;
            _adminRepo = adminRepo;
            _feedbackProductRepo = feedbackProductRepo;
        }

        //
        public IActionResult Index()
        {
            return View(_productRepo.GetTrademarks());
        }

        //
        public IActionResult ListProducts(int id)
        {
            var listProducts = _productRepo.GetProductByTrademark(id);
            _logger.LogInformation("Display list products!");
            return View(listProducts);
        }

        public IActionResult GetListProducts(int id)
        {
            var listProducts = _productRepo.GetProductByTrademark(id);
            _logger.LogInformation("Display list products!");
            return new JsonResult(listProducts);
        }

        public IActionResult GetProductSale(int id)
        {
            var listSale = _productRepo.GetProductsSale(id);
            _logger.LogInformation("Display list products sale!");
            return new JsonResult(listSale);
        }

        public IActionResult ListProductSale(int id)
        {
            var listSale = _productRepo.GetProductsSale(id);
            _logger.LogInformation("Display list products sale!");
            return View(listSale);
        }

        public IActionResult GetProductNew(int id)
        {
            var listNew = _productRepo.GetProductsNew(id);
            _logger.LogInformation("Display list products new!");
            return new JsonResult(listNew);
        }

        public IActionResult ListProductNew(int id)
        {
            var listNew = _productRepo.GetProductsNew(id);
            _logger.LogInformation("Display list products new!");
            return View(listNew);
        }

        //
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

            _logger.LogInformation("Created New Products");
            return RedirectToAction("Index");
        }

        //
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var productInDb = _productRepo.GetProductById(id);
            if (productInDb == null)
                return NotFound();
            var productVM = new ProductTrademarkViewModel()
            {
                Products = productInDb,
                Trademarks = _trademarkRepo.GetTrademarks()
            };
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(ProductTrademarkViewModel productTrademarkViewModel)
        {
            var product = _productRepo.EditProduct(productTrademarkViewModel);
            if (product == null)
            {
                _logger.LogInformation("Product has been update!");
                return RedirectToAction(nameof(Index));
            }

            ViewData["Message"] = product.StatusMessage;
            return View(product);
        }

        //
        public IActionResult ProductDetails(int id)
        {
            var product = _productRepo.GetProductDetail(id);
            if (product.Product != null)
                _logger.LogInformation($"Product: {product.Product.ProductName}");
            return View(product);
        }

        //
        [HttpPost]
        public async Task<IActionResult> SendFeedback(int productId, string message)
        {
            if (message == null) return Json(new { success = false, message = "Please write your feedback again!" });
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFullName = await _adminRepo.GetUserName(currentUser);
            var sendMessage = await _feedbackProductRepo.SaveComment(productId, currentUser, message);
            if (!sendMessage) return RedirectToAction();
            _logger.LogInformation($"{userFullName} has been sent a feedback: {message}!");
            return Json(new { success = true, userInput = userFullName, messageInput = message });
        }

        //    
        public IActionResult ExportToExcel()
        {
            var products = GetProductList();
            var stream = new MemoryStream();

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("Product");
                var customStyle = xlPackage.Workbook.Styles.CreateNamedStyle("CustomStyle");
                customStyle.Style.Font.UnderLine = true;
                customStyle.Style.Font.Color.SetColor(new Color( /* Color [Red] */));
                var startRow = 5;
                var row = startRow;

                worksheet.Cells["A1"].Value = "Product Export";
                using (var r = worksheet.Cells["A1:O1"])
                {
                    r.Merge = true;
                    r.Style.Font.Color.SetColor(Color.Green);
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(23, 55, 93));
                }

                worksheet.Cells["A2"].Value = "Id";
                worksheet.Cells["B2"].Value = "Title";
                worksheet.Cells["C2"].Value = "TitleURL";
                worksheet.Cells["D2"].Value = "ProductName";
                worksheet.Cells["E2"].Value = "Image";
                worksheet.Cells["F2"].Value = "Image1";
                worksheet.Cells["G2"].Value = "Quantity";
                worksheet.Cells["H2"].Value = "Price";
                worksheet.Cells["I2"].Value = "Badge";
                worksheet.Cells["J2"].Value = "Category";
                worksheet.Cells["K2"].Value = "ProductCard";
                worksheet.Cells["L2"].Value = "Status";
                worksheet.Cells["M2"].Value = "StatusMessage";
                worksheet.Cells["N2"].Value = "ChangeStatusBy";
                worksheet.Cells["O2"].Value = "TrademarkId";
                //14
                worksheet.Cells["A2:O2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells["A2:O2"].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                row = 3;
                foreach (var product in products)
                {
                    worksheet.Cells[row, 1].Value = product.Id;
                    worksheet.Cells[row, 2].Value = product.Title;
                    worksheet.Cells[row, 3].Value = product.Title;
                    worksheet.Cells[row, 4].Value = product.ProductName;
                    worksheet.Cells[row, 5].Value = product.Image;
                    worksheet.Cells[row, 6].Value = product.Image1;
                    worksheet.Cells[row, 7].Value = product.Quantity;
                    worksheet.Cells[row, 8].Value = product.Price;
                    worksheet.Cells[row, 9].Value = product.Badge;
                    worksheet.Cells[row, 10].Value = product.Category;
                    worksheet.Cells[row, 11].Value = product.ProductCard;
                    worksheet.Cells[row, 12].Value = product.Status;
                    worksheet.Cells[row, 13].Value = product.StatusMessage;
                    worksheet.Cells[row, 14].Value = product.ChangeStatusBy;
                    worksheet.Cells[row, 15].Value = product.TrademarkId;

                    row++; // row = row + 1;
                }

                xlPackage.Workbook.Properties.Title = "Product list";
                xlPackage.Workbook.Properties.Author = "Huynbt";

                xlPackage.Save();
            }

            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Product.xlsx");
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
            var filename = ContentDispositionHeaderValue.Parse(FormFile.ContentDisposition).FileName.Trim('"');
            var MainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files");
            if (!Directory.Exists(MainPath))
            {
                Directory.CreateDirectory(MainPath);
            }
            var filePath = Path.Combine(MainPath, FormFile.FileName);
            using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
            {
                await FormFile.CopyToAsync(stream);
            }
            string extension = Path.GetExtension(filename);
            string conString = string.Empty;
            switch (extension)
            {
                case ".xls":
                    conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath +
                                ";Extended Properties='Excel 8.0;HDR=YES'";
                    break;
                case ".xlsx":
                    conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath +
                                ";Extended Properties='Excel 8.0;HDR=YES'";
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
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                        connExcel.Close();
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        odaExcel.Fill(dt);
                        connExcel.Close();
                    }
                }
            }
            conString =
                "Server=localhost,1433;Database=Sneaker;User=sa;Password=Password@123;";

            using (SqlConnection con = new SqlConnection(conString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    sqlBulkCopy.DestinationTableName = "dbo.Products";
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
                    sqlBulkCopy.ColumnMappings.Add("CreateAt", "CreateAt");
                    sqlBulkCopy.ColumnMappings.Add("UpdateAt", "UpdateAt");
                    sqlBulkCopy.ColumnMappings.Add("TrademarkId", "TrademarkId");

                    con.Open();
                    sqlBulkCopy.WriteToServer(dt);
                    con.Close();
                }
            }
            ViewBag.Message = "File Imported and excel data saved into database";
            _logger.LogInformation("File Imported and excel Products data saved into database");
            return View();
        }
    }
}
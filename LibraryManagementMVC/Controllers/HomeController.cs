using LibraryManagementMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace LibraryManagementMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly string baseAddress;

        public HomeController(IOptions<ApiServiceSetting> options)
        {
            baseAddress = options.Value.BaseAddress;
        }

        /// <summary>
        /// Daily Report Action
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> IndexAsync()
        {

            var dailyReportModelList = new List<DailyReportModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/BookTransactions/DailyReport");
                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    dailyReportModelList = JsonConvert.DeserializeObject<List<DailyReportModel>>(response);
                }
                return View(dailyReportModelList);
            }
        }

        public IActionResult Borrow(string isbn)
        {
            var model = new BookBorrowModel();
            model.ISBN = isbn;
            return View(model);
        }

        [HttpPost]
        public IActionResult Borrow(BookBorrowModel model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);

                //HTTP POST
                var postTask = client.PostAsJsonAsync<BookBorrowModel>("api/BookTransactions/Borrow", model);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("IndexAsync");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please Try Again Later.");

            return View(model);
        }

        [HttpGet]
        public IActionResult SearchBook(string title, string author, string isbn)
        {
            var bookList = new List<BookModel>();
            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(author) && string.IsNullOrEmpty(isbn))
                return View(bookList);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);

                //HTTP POST
                var postTask = client.PostAsJsonAsync<SearchBookModel>("api/Books/Search", new SearchBookModel() { Title = title, Author = author, ISBN = isbn });
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = result.Content.ReadAsStringAsync().Result;
                    bookList = JsonConvert.DeserializeObject<List<BookModel>>(response);
                    return View(bookList);
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please Try Again Later.");
            return View(bookList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using LibraryManagementMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;

namespace LibraryManagementMVC.Controllers
{
    public class HomeController : Controller
    {
        private const  string BASE_URI_ADDRESS= "https://localhost:7211/";

        /// <summary>
        /// Daily Report Action
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> IndexAsync()
        {

            var dailyReportModelList = new List<DailyReportModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URI_ADDRESS);
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
                client.BaseAddress = new Uri(BASE_URI_ADDRESS);

                //HTTP POST
                var postTask = client.PostAsJsonAsync<BookBorrowModel>("api/BookTransactions/BorrowBook",model);
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

        public IActionResult SearchBook()
        {
            var bookList = new List<BookModel>();
            return View(bookList);
        }

        [HttpPost]
        public IActionResult SearchBook(string title,string author,string isbn)
        {
            var searchModel = new SearchBookModel() { Title = title, Author = author, ISBN = isbn };
            var bookList = new List<BookModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URI_ADDRESS);

                //HTTP POST
                var postTask = client.PostAsJsonAsync<SearchBookModel>("api/Books/SearchBook", searchModel);
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

            return View();
        }

       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using Blog.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class ArticleController : Controller
    {
        // GET: Article
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //GET: Article/List
        public ActionResult List(int page = 1)
        {
            using (var database = new BlogDbContext())
            {
                var pageSize = 6;

                var articles = database.Articles
                    .OrderByDescending(a => a.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(a => a.Author)
                    .ToList();

                ViewBag.CurrentPage = page;

                var article = articles.FirstOrDefault();

                return View(articles);
            }
        }

        //GET: Article/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                if (article == null)
                {
                    return HttpNotFound();
                }

                return View(article);
            }

        }

        //GET: Article/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //POST: Article/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Article article)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    var authorId = database.Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First()
                        .Id;

                    article.AuthorId = authorId;
                    article.DateAdded = DateTime.UtcNow;

                    if (article.ImageURL == null)
                    {
                        article.ImageURL = "/Content/Images/NoPreview.jpg";
                    }

                    database.Articles.Add(article);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(article);
        }

        //GET: Article/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                if (!IsUserAuthorizedToEdit(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                if (article == null)
                {
                    return HttpNotFound();
                }

                return View(article);
            }
        }

        //POST: Article/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                if (article == null)
                {
                    return HttpNotFound();
                }

                database.Articles.Remove(article);
                database.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        //GET: Article/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .First();

                if (!IsUserAuthorizedToEdit(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                if (article == null)
                {
                    return HttpNotFound();
                }

                var model = new ArticleViewModel();
                model.Id = article.Id;
                model.Title = article.Title;
                model.Content = article.Content;

                return View(model);
            }
        }

        //POST: Article/Edit
        [HttpPost]
        public ActionResult Edit(ArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    var article = database.Articles
                        .FirstOrDefault(a => a.Id == model.Id);

                    article.Title = model.Title;
                    article.Content = model.Content;

                    database.Entry(article).State = EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        private bool IsUserAuthorizedToEdit(Article article)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = article.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }

        //GET: Article/MyArticles
        public ActionResult MyArticles(int page = 1)
        {
            var pageSize = 6;

            var userId = this.User.Identity.GetUserId();

            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var database = new BlogDbContext();

            var articles = database.Articles
                .Where(a => a.Author.Id == userId)
                .OrderByDescending(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.ArticlesNum = database.Articles
                .Where(a => a.Author.Id == userId)
                .Count();

            return View(articles);
        }

        //GET: Article/Categories
        public ActionResult Categories()
        {
            return View();
        }

        //GET: Article/Club
        [ActionName("Club")]
        public ActionResult CategoryClub(int page = 1)
        {
            var pageSize = 6;

            var database = new BlogDbContext();

            var articles = database.Articles
                .Where(a => a.Category == CategoryType.Club)
                .OrderByDescending(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;

            return View(articles);
        }

        //GET: Article/Club
        [ActionName("League")]
        public ActionResult CategoryLeague(int page = 1)
        {
            var pageSize = 6;

            var database = new BlogDbContext();

            var articles = database.Articles
                .Where(a => a.Category == CategoryType.League)
                .OrderByDescending(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;

            return View(articles);
        }

        //GET: Article/Players
        [ActionName("Players")]
        public ActionResult CategoryPlayers(int page = 1)
        {
            var pageSize = 6;

            var database = new BlogDbContext();

            var articles = database.Articles
                .Where(a => a.Category == CategoryType.Players)
                .OrderByDescending(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;

            return View(articles);
        }

        //GET: Article/Game
        [ActionName("Game")]
        public ActionResult CategoryGame(int page = 1)
        {
            var pageSize = 6;

            var database = new BlogDbContext();

            var articles = database.Articles
                .Where(a => a.Category == CategoryType.Game)
                .OrderByDescending(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;

            return View(articles);
        }

        //GET: Article/Transfers
        [ActionName("Transfers")]
        public ActionResult CategoryTransfers(int page = 1)
        {
            var pageSize = 6;

            var database = new BlogDbContext();

            var articles = database.Articles
                .Where(a => a.Category == CategoryType.Transfers)
                .OrderByDescending(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;

            return View(articles);
        }

        //GET: Article/Stats
        [ActionName("Stats")]
        public ActionResult CategoryStats(int page = 1)
        {
            var pageSize = 6;

            var database = new BlogDbContext();

            var articles = database.Articles
                .Where(a => a.Category == CategoryType.Stats)
                .OrderByDescending(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;

            return View(articles);
        }

        //GET: Article/Rate        
        public ActionResult Rate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var datase = new BlogDbContext())
            {
                var article = datase.Articles
                    .Where(a => a.Id == id)
                    .First();

                if (article == null)
                {
                    return new HttpNotFoundResult();
                }

                var articleRate = new ArticleRatingViewModel()
                {
                    ArticleId = article.Id,
                    User = User.Identity.Name,
                };

                return View(articleRate);
            }
        }

        //POST: Article/Rate
        [HttpPost]
        public ActionResult Rate(ArticleRatingViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    var article = database.Articles
                        .Where(a => a.Id == model.ArticleId)
                        .First();

                    if (article == null)
                    {
                        return HttpNotFound();
                    }

                    article.RatesNum++;
                    article.RatesSum += model.Rating;
                    article.AverageRating = (decimal)article.RatesSum / article.RatesNum;

                    database.Entry(article).State = EntityState.Modified;
                    database.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        //GET: Article/TopRated
        public ActionResult TopRated()
        {
            using (var database = new BlogDbContext())
            {
                var articles = database.Articles
                    .OrderByDescending(x => x.AverageRating)
                    .Include(a => a.Author)
                    .Take(10)
                    .ToList();

                return View(articles);
            }
        }
    }
}
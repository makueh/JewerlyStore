using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebProject.Models;
namespace WebProject.Controllers

{

    public class MyController : Controller
    {

        JewerlyStoreEntities db = new JewerlyStoreEntities();
        // GET: My

        public ActionResult Index()
        {
            var query = db.Products.Select(s => s).OrderBy(r => Guid.NewGuid()).Take(15);

            return View(query);
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Customer customer)
        {
            var info = db.Customer.FirstOrDefault(x => x.Email == customer.Email && x.Password == customer.Password);


            if (info != null)
            {
                Session["id"] = info.CustomerID;
                FormsAuthentication.SetAuthCookie(customer.Email, false);
                return RedirectToAction("Index","My");

            }
            else
            {
                ViewBag.Message = "E-mail veya şifre yanlış.";
                return View();

            }
        }
        
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View("Signup");
            }
            if (db.Customer.FirstOrDefault(x => x.Email == customer.Email) == null)
            {
                db.Customer.Add(customer);
                db.SaveChanges();
                ViewBag.Message = "Kayıt oluşturuldu.";
                return RedirectToAction("Login","My");

            }
            if (db.Customer.FirstOrDefault(x => x.Email == customer.Email) != null)

            {
                ViewBag.Message = "Bu Email adresi kayıtlı.";

            }

            return View();

        }

       
        public ActionResult Signout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        public ActionResult Charms()
        {
            var query = db.Products.Where(s => s.CategoryID == 1).Select(s => s);
            return View(query);
        }
        public ActionResult Necklaces()
        {
            var query = db.Products.Where(s => s.CategoryID == 2).Select(s => s);
            return View(query);
        }
        public ActionResult Rings()
        {
            var query = db.Products.Where(s => s.CategoryID == 3).Select(s => s);
            return View(query);
        }
        public ActionResult Earings()
        {
            var query = db.Products.Where(s => s.CategoryID == 4).Select(s => s);
            return View(query);
        }
        public ActionResult Bracelets()
        {
            var query = db.Products.Where(s => s.CategoryID == 5).Select(s => s);
            return View(query);
        }
        public ActionResult Details(int id)
        {
            var query = db.Products.Where(s => s.ProductID == id).Select(s => s);
            return View(query);
        }

        [Authorize(Roles = "U")]
        public ActionResult AddToCart(int productID)
        {
            var price = db.Products.Where(x => x.ProductID == productID).Select(x => x.Price).FirstOrDefault();
       
            int id = (int)Session["id"];
            var count = db.Cart.Select(x => x).Where(x => x.ProductID == productID && x.CustomerID == id).Count();
            if (count == 0)
            {
                var cartItem = new Cart
                {
                    CustomerID = (int)Session["id"],
                    ProductID = productID,
                    Quantity = 1,
                    TotalPrice = price
                };
                db.Cart.Add(cartItem);
            }
            else
            {
                Cart updatedCart = db.Cart.Where(x => x.ProductID == productID).FirstOrDefault();
                updatedCart.Quantity += 1;
                updatedCart.TotalPrice = updatedCart.Quantity * price;
            }
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }


        [Authorize(Roles = "U")]
        public ActionResult Cart()
        {
            int id = (int)Session["id"];
            Session["TotalCartPrice"] = db.Cart.Where(x=>x.CustomerID==id).Sum(x=>x.TotalPrice);
            var query = db.Cart.Where(x => x.CustomerID == id).ToList();
           
            return View(query);
        }
        public ActionResult RemoveToCart(int productID)
        {
            int id = (int)Session["id"];
            db.Cart.RemoveRange(db.Cart.Select(x=>x).Where(x=>x.CustomerID == id && x.ProductID == productID));
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult CheckOut()
        {
            int id = (int)Session["id"];
            var query = db.Cart.Where(x => x.CustomerID == id).ToList();
            return View(query);
        }
        public ActionResult DeleteProduct(int productID)
        {

            db.Products.RemoveRange(db.Products.Where(x => x.ProductID == productID));
            db.SaveChanges();
            return RedirectToAction("Index","My");
        }
        public ActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddProduct(Products product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                Session["Message"] = "Urun eklendi.";
            }
            else
            {
                Session["Message"] = "Urun eklenemedi.";

            }
            return RedirectToAction("AddProduct","My");
        }
        public ActionResult DeleteAllProduct()
        {
            var query = db.Products.Select(x => x);
            return View(query);
        }
    }
}
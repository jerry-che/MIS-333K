using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Che_Jerry_HW6.DAL;
using Che_Jerry_HW6.Models;
using Che_Jerry_HW6.Utilities;

namespace Che_Jerry_HW6.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.Include(o => o.OrderDetails).ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(o => o.OrderDetails).ThenInclude(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,OrderNumber,OrderDate,Notes")] Order order)
        {
            order.OrderNumber = GenerateNextOrderNumber.GetNextOrderNumber(_context);
            order.OrderDate = System.DateTime.Today;

            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();

                return RedirectToAction("AddToOrder", new { id = order.OrderID });
            }
            return View(order);
        }

        public IActionResult AddToOrder(int? id)
        {
            if (id == null)
            {
                return View("Error", new string[] { "You must specify an order to add!" });
            }
            Order ord = _context.Orders.Find(id);
            if (ord == null)
            {
                return View("Error", new string[] { "Order not found!" });
            }

            OrderDetail od = new OrderDetail() { Order = ord };

            ViewBag.AllProducts = GetAllProducts();
            return View("AddToOrder", od);

        }

        [HttpPost]
        public IActionResult AddToOrder(OrderDetail od, int SelectedProduct)
        {
            //find the course associated with the selected course id
            Product product = _context.Products.Find(SelectedProduct);

            //set the registration detail's course equal to the course we just found
            od.Product = product;

            //find the registration based on the id
            Order ord = _context.Orders.Find(od.Order.OrderID);

            //set the registration detail's registration equal to the registration we just found
            od.Order = ord;

            //set the course fee for this detail equal to the current course fee
            od.ProductPrice = od.Product.ProductPrice;

            //add total fees
            od.ExtendedPrice = od.Quantity * od.ProductPrice;

            if (ModelState.IsValid)
            {
                _context.OrderDetails.Add(od);
                _context.SaveChanges();
                return RedirectToAction("Details", new { id = od.Order.OrderID });
            }
            ViewBag.AllProducts = GetAllProducts();
            return View(od);
        }

        // GET: Orders/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _context.Orders
                                       .Include(o => o.OrderDetails)
                                       .ThenInclude(o => o.Product)
                                        .FirstOrDefault(o => o.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit( Order order)
        {
            //Find the related registration in the database
            Order Dbord = _context.Orders.Find(order.OrderID);

            //Update the notes
            Dbord.Notes = order.Notes;

            //Update the database
            _context.Orders.Update(Dbord);

            //Save changes
            _context.SaveChanges();

            //Go back to index
            return RedirectToAction(nameof(Index));

        }


        public IActionResult RemoveFromOrder(int? id)
        {
            if (id == null)
            {
                return View("Error", new string[] { "You need to specify an order id" });
            }

            Order ord = _context.Orders.Include(o => o.OrderDetails).ThenInclude(o => o.Product).FirstOrDefault(o => o.OrderID == id);

            if (ord == null || ord.OrderDetails.Count == 0)//registration is not found
            {
                return RedirectToAction("Details", new { id = id });
            }

            //pass the list of order details to the view
            return View(ord.OrderDetails);
        }


        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }

        private SelectList GetAllProducts()
        {
            List<Product> Products = _context.Products.ToList();
            SelectList allProducts = new SelectList(Products, "ProductID", "ProductName");
            return allProducts;
        }
    }
}

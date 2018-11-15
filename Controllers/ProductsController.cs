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
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(_context.Products.ToList());
        }

        // GET: Products/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products.Include(s => s.SupplierDetails).ThenInclude(s => s.Supplier).FirstOrDefault(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.AllSuppliers = GetAllSuppliers();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int[] SelectedSuppliers, [Bind("ProductID,SKU,ProductName,ProductPrice,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.SKU = GenerateSKU.GetNextSKU(_context);
                _context.Add(product);
                _context.SaveChanges();

                Product dbProduct = _context.Products.FirstOrDefault(p => p.SKU == product.SKU);

                foreach (int i in SelectedSuppliers)
                {
                    Supplier dbSupplier = _context.Suppliers.Find(i);
                    SupplierDetail sd = new SupplierDetail();
                    sd.Product = dbProduct;
                    sd.Supplier = dbSupplier;
                    _context.SupplierDetails.Add(sd);
                    _context.SaveChanges();

                }

                return RedirectToAction(nameof(Index));

            }
            ViewBag.AllSuppliers = GetAllSuppliers();
            return View(product);
        }

        // GET: Products/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products.Include(s => s.SupplierDetails).ThenInclude(s => s.Product).FirstOrDefault(s => s.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.AllSuppliers = GetAllSuppliers(product);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product, int[] SelectedSuppliers)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Product dbProduct = _context.Products
                        .Include(s => s.SupplierDetails)
                            .ThenInclude(s => s.Supplier)
                        .FirstOrDefault(s => s.ProductID == product.ProductID);

                    dbProduct.ProductPrice = product.ProductPrice;
                    dbProduct.ProductName = product.ProductName;
                    dbProduct.Description = product.Description;

                    _context.Update(dbProduct);
                    _context.SaveChanges();

                    //edit department/course relationships

                    //loop through selected departments and find ones that need to be removed
                    List<SupplierDetail> SuppliersToRemove = new List<SupplierDetail>();
                    foreach (SupplierDetail sd in dbProduct.SupplierDetails)
                    {
                        if (SelectedSuppliers.Contains(sd.Supplier.SupplierID) == false)
                        //list of selected depts does not include this departments id
                        {
                            SuppliersToRemove.Add(sd);
                        }
                    }
                    //remove departments you found in list above - this has to be 2 separate steps because you can't 
                    //iterate over a list that you are removing items from
                    foreach (SupplierDetail sd in SuppliersToRemove)
                    {
                        _context.SupplierDetails.Remove(sd);
                        _context.SaveChanges();
                    }

                    //now add the departments that are new
                    foreach (int i in SelectedSuppliers)
                    {
                        if (dbProduct.SupplierDetails.Any(s => s.Supplier.SupplierID == i) == false)
                        //this supplier has not yet been added
                        {
                            //create a new course department
                            SupplierDetail sd = new SupplierDetail();

                            //connect the new course department to the department and course
                            sd.Supplier = _context.Suppliers.Find(i);
                            sd.Product = dbProduct;

                            //update the database
                            _context.SupplierDetails.Add(sd);
                            _context.SaveChanges();
                        }
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.AllSuppliers = GetAllSuppliers(product);
            return View(product);

        }


       
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }

        private MultiSelectList GetAllSuppliers()
        {
            List<Supplier> allSuppliers = _context.Suppliers.ToList();
            MultiSelectList supplierList = new MultiSelectList(allSuppliers, "SupplierID", "SupplierName");
            return supplierList;
        }

        //overload for editing departments
        private MultiSelectList GetAllSuppliers(Product product)
        {
            //create a list of all the departments
            List<Supplier> allSuppliers = _context.Suppliers.ToList();

            //create a list for the department ids that this course already belongs to
            List<int> currentSuppliers = new List<int>();

            //loop through all the details to find the list of current departments
            foreach (SupplierDetail sd in product.SupplierDetails)
            {
                currentSuppliers.Add(sd.Supplier.SupplierID);
            }

            //create the multiselect with the overload for currently selected items
            MultiSelectList supplierList = new MultiSelectList(allSuppliers, "SupplierID", "SupplierName", currentSuppliers);

            //return the list
            return supplierList;
        }
    }
}


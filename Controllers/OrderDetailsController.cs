using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Che_Jerry_HW6.DAL;
using Che_Jerry_HW6.Models;

namespace Che_Jerry_HW6.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly AppDbContext _context;

        public OrderDetailsController(AppDbContext context)
        {
            _context = context;
        }


        // GET: OrderDetails/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = _context.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(OrderDetail orderDetail)
        {
            OrderDetail DbOrdDet = _context.OrderDetails
                                   .Include(o => o.Order)
                                   .Include(o => o.Product)
                                   .FirstOrDefault(o => o.OrderDetailID == orderDetail.OrderDetailID);

            DbOrdDet.Quantity = orderDetail.Quantity;
            DbOrdDet.ProductPrice = DbOrdDet.Product.ProductPrice;
            DbOrdDet.ExtendedPrice = DbOrdDet.Quantity * DbOrdDet.ProductPrice;

            if(ModelState.IsValid)
            {
                _context.OrderDetails.Update(DbOrdDet);
                _context.SaveChanges();
                return RedirectToAction("Details", "Orders", new { id = DbOrdDet.Order.OrderID });

            }

            return View(orderDetail);

        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails.Include(o => o.Order).FirstOrDefaultAsync(o => o.OrderDetailID == id);
            int OrderID = orderDetail.Order.OrderID;
            
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Orders",new { id = OrderID });
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailID == id);
        }
    }
}

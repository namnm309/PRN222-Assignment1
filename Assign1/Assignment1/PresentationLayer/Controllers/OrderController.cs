using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using PresentationLayer.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer.Controllers
{
    public class OrderController : BaseDashboardController
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IDealerContractService _contractService;
        private readonly IEVMReportService _evmService;
        private readonly AppDbContext _dbContext;

        public OrderController(
            IOrderService orderService,
            IProductService productService,
            ICustomerService customerService,
            IDealerContractService contractService,
            IEVMReportService evmService,
            AppDbContext dbContext)
        {
            _orderService = orderService;
            _productService = productService;
            _customerService = customerService;
            _contractService = contractService;
            _evmService = evmService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? dealerId = null, string? status = null)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            
            Console.WriteLine($"[DEBUG] Order Index - UserRole: {userRole}, DealerId: {dealerIdString}, Email: {userEmail}");

            Guid? dealerIdFilter = null;
            
            if (userRole == "DealerManager" || userRole == "DealerStaff")
            {
                if (!string.IsNullOrEmpty(dealerIdString) && Guid.TryParse(dealerIdString, out var dealerIdParsed))
                {
                    dealerIdFilter = dealerIdParsed;
                }
                else
                {
                    TempData["Error"] = "Tài khoản chưa được gán đại lý. Vui lòng liên hệ Admin.";
                    return View(new System.Collections.Generic.List<DataAccessLayer.Entities.Order>());
                }
            }
            else
            {
                dealerIdFilter = dealerId;
            }

            ViewBag.Dealers = await _evmService.GetAllDealersAsync();
            ViewBag.SelectedDealerId = dealerIdFilter;
            ViewBag.SelectedStatus = status;

            var (ok, err, orders) = await _orderService.GetAllAsync(dealerIdFilter, status);
            if (!ok)
            {
                TempData["Error"] = err;
                return View(new System.Collections.Generic.List<DataAccessLayer.Entities.Order>());
            }

            return View(orders);
        }


        [HttpGet]
        public async Task<IActionResult> CreateQuotation()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (userRole != "DealerManager" && userRole != "DealerStaff")
            {
                TempData["Error"] = "Chỉ Dealer Manager và Dealer Staff mới được tạo báo giá.";
                return RedirectToAction("Index");
            }

            
            ViewBag.Products = await _evmService.GetAllProductsAsync();
            ViewBag.Customers = await _evmService.GetAllCustomersAsync();
            ViewBag.SalesStaff = await _evmService.GetAllSalesStaffAsync();

            return View(new OrderCreateViewModel());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuotation(OrderCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _evmService.GetAllProductsAsync();
                ViewBag.Customers = await _evmService.GetAllCustomersAsync();
                ViewBag.SalesStaff = await _evmService.GetAllSalesStaffAsync();
                return View(vm);
            }

            
            Guid dealerId = Guid.Empty;
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            
            Console.WriteLine($"[DEBUG] CreateQuotation - UserRole: {userRole}, Session DealerId: {dealerIdString}");
            
            
            if (userRole == "DealerManager" || userRole == "DealerStaff")
            {
                if (string.IsNullOrEmpty(dealerIdString) || !Guid.TryParse(dealerIdString, out dealerId))
                {
                    Console.WriteLine($"[DEBUG] ERROR: Dealer user has no DealerId in session");
                    TempData["Error"] = "Tài khoản chưa được gán đại lý. Vui lòng liên hệ Admin.";
                    return RedirectToAction("Index");
                }
            }
            
            else if (userRole == "Admin" || userRole == "EVMStaff")
            {
                TempData["Error"] = "Admin và EVM Staff không được tạo báo giá trực tiếp. Chỉ Dealer Manager/Staff mới có quyền này.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền tạo báo giá.";
                return RedirectToAction("Index");
            }

            
            var inventory = await _dbContext.InventoryAllocation
                .FirstOrDefaultAsync(i => i.ProductId == vm.ProductId && i.DealerId == dealerId && i.IsActive);
            
            if (inventory == null || inventory.AvailableQuantity <= 0)
            {
                TempData["Error"] = "Sản phẩm này đã hết hàng trong kho đại lý. Vui lòng đặt hàng từ hãng hoặc chọn sản phẩm khác.";
                ViewBag.Products = await _evmService.GetAllProductsAsync();
                ViewBag.Customers = await _evmService.GetAllCustomersAsync();
                ViewBag.SalesStaff = await _evmService.GetAllSalesStaffAsync();
                return View(vm);
            }

            var (ok, err, order) = await _orderService.CreateQuotationAsync(
                vm.ProductId, vm.CustomerId, dealerId, vm.SalesPersonId,
                vm.Price, vm.Discount, vm.Description, vm.Notes);

            if (!ok)
            {
                ModelState.AddModelError("", err);
                ViewBag.Products = await _evmService.GetAllProductsAsync();
                ViewBag.Customers = await _evmService.GetAllCustomersAsync();
                ViewBag.SalesStaff = await _evmService.GetAllSalesStaffAsync();
                return View(vm);
            }

            TempData["Msg"] = "Tạo báo giá thành công!";
            return RedirectToAction(nameof(Detail), new { id = order.Id });
        }

        
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var (ok, err, order) = await _orderService.GetAsync(id);
            if (!ok) return NotFound();
            return View(order);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(Guid id)
        {
            var (ok, err, order) = await _orderService.ConfirmOrderAsync(id);
            if (!ok)
            {
                TempData["Error"] = err;
                return RedirectToAction(nameof(Detail), new { id });
            }

            TempData["Msg"] = "Xác nhận đơn hàng thành công!";
            return RedirectToAction(nameof(Detail), new { id });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePayment(Guid id, string paymentStatus, string paymentMethod, DateTime? paymentDueDate)
        {
            var (ok, err, order) = await _orderService.UpdatePaymentAsync(id, paymentStatus, paymentMethod, paymentDueDate);
            if (!ok)
            {
                TempData["Error"] = err;
                return RedirectToAction(nameof(Detail), new { id });
            }

            TempData["Msg"] = "Cập nhật thanh toán thành công!";
            return RedirectToAction(nameof(Detail), new { id });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deliver(Guid id, DateTime deliveryDate)
        {
            var (ok, err, order) = await _orderService.DeliverOrderAsync(id, deliveryDate);
            if (!ok)
            {
                TempData["Error"] = err;
                return RedirectToAction(nameof(Detail), new { id });
            }

            TempData["Msg"] = "Cập nhật giao hàng thành công!";
            return RedirectToAction(nameof(Detail), new { id });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var (ok, err, order) = await _orderService.CancelOrderAsync(id);
            if (!ok)
            {
                TempData["Error"] = err;
                return RedirectToAction(nameof(Detail), new { id });
            }

            TempData["Msg"] = "Hủy đơn hàng thành công!";
            return RedirectToAction(nameof(Detail), new { id });
        }

        
        [HttpGet]
        public async Task<IActionResult> CreateContract(Guid orderId)
        {
            var (ok, err, order) = await _orderService.GetAsync(orderId);
            if (!ok || order.Status != "Delivered")
            {
                TempData["Error"] = "Chỉ có thể tạo hợp đồng cho đơn hàng đã giao xe";
                return RedirectToAction(nameof(Detail), new { id = orderId });
            }

            ViewBag.Order = order;
            return View(new ContractCreateViewModel { OrderId = orderId });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContract(ContractCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var (ok2, err2, order2) = await _orderService.GetAsync(vm.OrderId);
                ViewBag.Order = order2;
                return View(vm);
            }

            var (ok, err, contract) = await _contractService.CreateFromOrderAsync(
                vm.OrderId, vm.ContractNumber, vm.Terms, vm.Notes);

            if (!ok)
            {
                ModelState.AddModelError("", err);
                var (ok2, err2, order2) = await _orderService.GetAsync(vm.OrderId);
                ViewBag.Order = order2;
                return View(vm);
            }

            TempData["Msg"] = "Tạo hợp đồng thành công!";
            return RedirectToAction(nameof(ContractDetail), new { id = contract.Id });
        }

        
        [HttpGet]
        public async Task<IActionResult> ContractDetail(Guid id)
        {
            var (ok, err, contract) = await _contractService.GetAsync(id);
            if (!ok) return NotFound();
            return View(contract);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetProductStock(Guid productId)
        {
            try
            {
                var dealerIdString = HttpContext.Session.GetString("DealerId");
                var userRole = HttpContext.Session.GetString("UserRole");
                
                Console.WriteLine($"[GetProductStock] ProductId={productId}, DealerId={dealerIdString}, Role={userRole}");

                
                if (string.IsNullOrEmpty(dealerIdString) || !Guid.TryParse(dealerIdString, out Guid dealerId))
                {
                    Console.WriteLine("[GetProductStock] No DealerId in session - returning generic stock info");
                    
                    
                    var product = await _dbContext.Product.FindAsync(productId);
                    if (product == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
                    }

                    return Json(new
                    {
                        success = true,
                        hasStock = product.StockQuantity > 0,
                        availableQuantity = product.StockQuantity,
                        message = product.StockQuantity > 0 
                            ? $"Tồn kho hệ thống: {product.StockQuantity} xe" 
                            : "Hết hàng trong hệ thống"
                    });
                }

                
                var inventory = await _dbContext.InventoryAllocation
                    .Include(i => i.Product)
                    .FirstOrDefaultAsync(i => i.ProductId == productId && i.DealerId == dealerId && i.IsActive);

                Console.WriteLine($"[GetProductStock] Inventory found: {inventory != null}, Available: {inventory?.AvailableQuantity}");

                if (inventory == null)
                {
                    return Json(new
                    {
                        success = true,
                        hasStock = false,
                        availableQuantity = 0,
                        message = "Sản phẩm này chưa có trong kho đại lý. Vui lòng đặt hàng từ hãng."
                    });
                }

                return Json(new
                {
                    success = true,
                    hasStock = inventory.AvailableQuantity > 0,
                    availableQuantity = inventory.AvailableQuantity,
                    reservedQuantity = inventory.ReservedQuantity,
                    allocatedQuantity = inventory.AllocatedQuantity,
                    minimumStock = inventory.MinimumStock,
                    status = inventory.Status,
                    message = inventory.AvailableQuantity > 0
                        ? $"Còn {inventory.AvailableQuantity} xe trong kho"
                        : "Sản phẩm đã hết hàng. Vui lòng đặt hàng từ hãng."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetProductStock] ERROR: {ex.Message}");
                return Json(new { success = false, message = "Lỗi khi kiểm tra tồn kho: " + ex.Message });
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer(string fullName, string email, string phoneNumber, string address)
        {
            var (ok, err, customer) = await _customerService.CreateAsync(fullName, email, phoneNumber, address);
            if (!ok)
                return Json(new { success = false, message = err });

            return Json(new { success = true, customer = new { id = customer.Id, name = customer.FullName, phone = customer.PhoneNumber } });
        }
    }
}


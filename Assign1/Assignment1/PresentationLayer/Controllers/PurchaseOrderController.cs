using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using PresentationLayer.Models;
using DataAccessLayer.Enum;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    public class PurchaseOrderController : BaseDashboardController
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IEVMReportService _evmService;
        private readonly AppDbContext _dbContext;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService, IEVMReportService evmService, AppDbContext dbContext)
        {
            _purchaseOrderService = purchaseOrderService;
            _evmService = evmService;
            _dbContext = dbContext;
        }

        // TEMP: Action để tạo table PurchaseOrder
        [HttpGet]
        public async Task<IActionResult> CreatePurchaseOrderTable()
        {
            try
            {
                // Check if table exists first
                var tableExists = await _dbContext.Database.CanConnectAsync();
                if (!tableExists)
                {
                    TempData["Error"] = "❌ Cannot connect to database";
                    return RedirectToAction("Index", "Dashboard");
                }

                // Execute SQL to create table
                var sql = @"
                    CREATE TABLE IF NOT EXISTS ""PurchaseOrder"" (
                        ""Id"" uuid NOT NULL,
                        ""DealerId"" uuid NOT NULL,
                        ""ProductId"" uuid NOT NULL,
                        ""RequestedById"" uuid NOT NULL,
                        ""ApprovedById"" uuid NULL,
                        ""OrderNumber"" character varying(50) NOT NULL,
                        ""RequestedQuantity"" integer NOT NULL,
                        ""UnitPrice"" decimal(18,2) NOT NULL,
                        ""TotalAmount"" decimal(18,2) NOT NULL,
                        ""Status"" integer NOT NULL,
                        ""RequestedDate"" timestamp with time zone NOT NULL,
                        ""ApprovedDate"" timestamp with time zone NULL,
                        ""ExpectedDeliveryDate"" timestamp with time zone NULL,
                        ""ActualDeliveryDate"" timestamp with time zone NULL,
                        ""Reason"" character varying(500) NOT NULL,
                        ""Notes"" character varying(1000) NULL,
                        ""RejectReason"" character varying(500) NULL,
                        ""CreatedAt"" timestamp with time zone NOT NULL,
                        ""UpdatedAt"" timestamp with time zone NOT NULL,
                        CONSTRAINT ""PK_PurchaseOrder"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_PurchaseOrder_Dealer_DealerId"" FOREIGN KEY (""DealerId"") REFERENCES ""Dealer"" (""Id"") ON DELETE RESTRICT,
                        CONSTRAINT ""FK_PurchaseOrder_Product_ProductId"" FOREIGN KEY (""ProductId"") REFERENCES ""Product"" (""Id"") ON DELETE RESTRICT,
                        CONSTRAINT ""FK_PurchaseOrder_Users_RequestedById"" FOREIGN KEY (""RequestedById"") REFERENCES ""Users"" (""Id"") ON DELETE RESTRICT,
                        CONSTRAINT ""FK_PurchaseOrder_Users_ApprovedById"" FOREIGN KEY (""ApprovedById"") REFERENCES ""Users"" (""Id"") ON DELETE RESTRICT
                    );

                    CREATE INDEX IF NOT EXISTS ""IX_PurchaseOrder_DealerId"" ON ""PurchaseOrder"" (""DealerId"");
                    CREATE INDEX IF NOT EXISTS ""IX_PurchaseOrder_ProductId"" ON ""PurchaseOrder"" (""ProductId"");
                    CREATE INDEX IF NOT EXISTS ""IX_PurchaseOrder_RequestedById"" ON ""PurchaseOrder"" (""RequestedById"");
                    CREATE INDEX IF NOT EXISTS ""IX_PurchaseOrder_ApprovedById"" ON ""PurchaseOrder"" (""ApprovedById"");

                    INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
                    VALUES ('20241002210000_AddPurchaseOrderEntity', '9.0.5')
                    ON CONFLICT (""MigrationId"") DO NOTHING;
                ";

                await _dbContext.Database.ExecuteSqlRawAsync(sql);
                
                TempData["Success"] = "✅ PurchaseOrder table created successfully! Bây giờ bạn có thể tạo đơn đặt hàng.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Failed to create table: {ex.Message}";
            }
            
            return RedirectToAction("Index", "Dashboard");
        }

        // TEMP: Action để kiểm tra database
        [HttpGet]
        public async Task<IActionResult> CheckDatabase()
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync();
                if (!canConnect)
                {
                    TempData["Error"] = "❌ Cannot connect to database";
                    return RedirectToAction("Index", "Dashboard");
                }

                // Check if PurchaseOrder table exists
                var sql = "SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'PurchaseOrder');";
                var tableExists = await _dbContext.Database.SqlQueryRaw<bool>(sql).FirstOrDefaultAsync();
                
                if (tableExists)
                {
                    TempData["Success"] = "✅ Database connected. PurchaseOrder table exists.";
                    
                    // Try to query PurchaseOrder table
                    try
                    {
                        var purchaseOrders = await _purchaseOrderService.GetAllAsync();
                        TempData["Success"] += $" Found {purchaseOrders.Data.Count} records.";
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"❌ Can query table but service failed: {ex.Message}";
                    }
                }
                else
                {
                    TempData["Error"] = "❌ PurchaseOrder table does not exist in database";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Database check failed: {ex.Message}";
            }
            
            return RedirectToAction("Index", "Dashboard");
        }

        // TEMP: Action để fix DealerId cho dealer account
        [HttpGet]
        public async Task<IActionResult> FixDealerAccount()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (userRole != "DealerManager" && userRole != "DealerStaff")
            {
                TempData["Error"] = "Chỉ dealer account mới cần fix.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Lấy dealer đầu tiên trong database
            var dealers = await _evmService.GetAllDealersAsync();
            if (!dealers.Any())
            {
                TempData["Error"] = "Không có đại lý nào trong hệ thống. Vui lòng tạo đại lý trước.";
                return RedirectToAction("Index", "Dashboard");
            }

            var firstDealer = dealers.First();
            
            // Gán dealerId vào session
            HttpContext.Session.SetString("DealerId", firstDealer.Id.ToString());
            
            TempData["Success"] = $"✅ Đã gán tài khoản {userEmail} vào đại lý: {firstDealer.Name}. Bây giờ bạn có thể truy cập 'Đặt xe từ hãng'.";
            
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Index(PurchaseOrderStatus? status = null)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            // DEBUG: Thêm thông tin debug
            Console.WriteLine($"[DEBUG] PurchaseOrder Index - UserRole: {userRole}, DealerId: {dealerIdString}, Email: {userEmail}");

            Guid? dealerIdFilter = null;

            // Dealer chỉ xem đơn của mình, Admin/EVM xem tất cả
            if (userRole == "DealerManager" || userRole == "DealerStaff")
            {
                if (string.IsNullOrEmpty(dealerIdString) || !Guid.TryParse(dealerIdString, out Guid dealerId))
                {
                    TempData["Error"] = $"Tài khoản {userEmail} chưa được gán đại lý. DealerId trong session: {dealerIdString ?? "NULL"}. Vui lòng liên hệ Admin để gán dealer.";
                    TempData["Debug"] = $"🔍 Debug: Role={userRole}, DealerId={dealerIdString ?? "NULL"}, Email={userEmail}";
                    return RedirectToAction("Index", "Dashboard");
                }
                dealerIdFilter = dealerId;
            }

            var (ok, err, purchaseOrders) = await _purchaseOrderService.GetAllAsync(dealerIdFilter, status);
            if (!ok)
            {
                TempData["Error"] = err;
            }

            ViewBag.Status = status;
            return View(purchaseOrders ?? new List<DataAccessLayer.Entities.PurchaseOrder>());
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            Console.WriteLine($"[DEBUG] PurchaseOrder Detail: Requesting ID = {id}");
            
            var (ok, err, purchaseOrder) = await _purchaseOrderService.GetAsync(id);
            Console.WriteLine($"[DEBUG] PurchaseOrder Detail: GetAsync result = Ok: {ok}, Error: {err}");
            
            if (!ok)
            {
                TempData["Error"] = err;
                return RedirectToAction(nameof(Index));
            }
            
            Console.WriteLine($"[DEBUG] PurchaseOrder Detail: Found order {purchaseOrder.OrderNumber}");

            // Kiểm tra quyền truy cập
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            if (userRole == "DealerManager" || userRole == "DealerStaff")
            {
                if (string.IsNullOrEmpty(dealerIdString) || !Guid.TryParse(dealerIdString, out Guid dealerId))
                {
                    TempData["Error"] = "Tài khoản chưa được gán đại lý.";
                    return RedirectToAction(nameof(Index));
                }

                if (purchaseOrder.DealerId != dealerId)
                {
                    TempData["Error"] = "Bạn không có quyền xem đơn đặt hàng này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(purchaseOrder);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            // Chỉ Dealer mới được tạo đơn đặt hàng
            if (userRole != "DealerManager" && userRole != "DealerStaff")
            {
                TempData["Error"] = "Chỉ Dealer Manager/Staff mới có quyền đặt xe từ hãng.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (string.IsNullOrEmpty(dealerIdString) || !Guid.TryParse(dealerIdString, out Guid dealerId))
            {
                TempData["Error"] = "Tài khoản chưa được gán đại lý. Vui lòng liên hệ Admin.";
                return RedirectToAction("Index", "Dashboard");
            }

            await LoadProductsToViewBag();
            return View(new PurchaseOrderCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseOrderCreateViewModel model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            var userIdString = HttpContext.Session.GetString("UserId");

            // Kiểm tra quyền
            if (userRole != "DealerManager" && userRole != "DealerStaff")
            {
                TempData["Error"] = "Chỉ Dealer Manager/Staff mới có quyền đặt xe từ hãng.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (string.IsNullOrEmpty(dealerIdString) || !Guid.TryParse(dealerIdString, out Guid dealerId))
            {
                TempData["Error"] = "Tài khoản chưa được gán đại lý.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                TempData["Error"] = "Phiên đăng nhập không hợp lệ.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                await LoadProductsToViewBag();
                return View(model);
            }

            Console.WriteLine($"[DEBUG] Creating PurchaseOrder: DealerId={dealerId}, ProductId={model.ProductId}, UserId={userId}");
            Console.WriteLine($"[DEBUG] Quantity={model.RequestedQuantity}, UnitPrice={model.UnitPrice}, Reason={model.Reason}");

            var (ok, err, purchaseOrder) = await _purchaseOrderService.CreateAsync(
                dealerId, model.ProductId, userId, model.RequestedQuantity, model.UnitPrice,
                model.Reason, model.Notes, model.ExpectedDeliveryDate);

            Console.WriteLine($"[DEBUG] CreateAsync result: Ok={ok}, Error={err}");

            if (!ok)
            {
                ModelState.AddModelError("", $"Chi tiết lỗi: {err}");
                TempData["Error"] = $"Lỗi tạo đơn: {err}";
                await LoadProductsToViewBag();
                return View(model);
            }

            TempData["Success"] = $"Tạo đơn đặt hàng thành công! Mã đơn: {purchaseOrder.OrderNumber}";
            return RedirectToAction(nameof(Detail), new { id = purchaseOrder.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            // Kiểm tra quyền hủy đơn
            var (exists, err, purchaseOrder) = await _purchaseOrderService.GetAsync(id);
            if (!exists)
            {
                TempData["Error"] = err;
                return RedirectToAction(nameof(Index));
            }

            // Dealer chỉ được hủy đơn của mình
            if (userRole == "DealerManager" || userRole == "DealerStaff")
            {
                if (string.IsNullOrEmpty(dealerIdString) || !Guid.TryParse(dealerIdString, out Guid dealerId))
                {
                    TempData["Error"] = "Tài khoản chưa được gán đại lý.";
                    return RedirectToAction(nameof(Index));
                }

                if (purchaseOrder.DealerId != dealerId)
                {
                    TempData["Error"] = "Bạn không có quyền hủy đơn đặt hàng này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var (ok, cancelErr, canceledOrder) = await _purchaseOrderService.CancelAsync(id);
            if (!ok)
            {
                TempData["Error"] = cancelErr;
            }
            else
            {
                TempData["Success"] = "Hủy đơn đặt hàng thành công!";
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        // Admin/EVM actions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id, DateTime? expectedDeliveryDate = null, string notes = "")
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền duyệt đơn đặt hàng.";
                return RedirectToAction(nameof(Index));
            }

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                TempData["Error"] = "Phiên đăng nhập không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var (ok, err, approvedOrder) = await _purchaseOrderService.ApproveAsync(id, userId, expectedDeliveryDate, notes);
            if (!ok)
            {
                TempData["Error"] = err;
            }
            else
            {
                TempData["Success"] = "Duyệt đơn đặt hàng thành công!";
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id, string rejectReason)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền từ chối đơn đặt hàng.";
                return RedirectToAction(nameof(Index));
            }

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                TempData["Error"] = "Phiên đăng nhập không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(rejectReason))
            {
                TempData["Error"] = "Vui lòng nhập lý do từ chối.";
                return RedirectToAction(nameof(Detail), new { id });
            }

            var (ok, err, rejectedOrder) = await _purchaseOrderService.RejectAsync(id, userId, rejectReason);
            if (!ok)
            {
                TempData["Error"] = err;
            }
            else
            {
                TempData["Success"] = "Từ chối đơn đặt hàng thành công!";
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid id, PurchaseOrderStatus status, DateTime? actualDeliveryDate = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền cập nhật trạng thái đơn đặt hàng.";
                return RedirectToAction(nameof(Index));
            }

            var (ok, err, updatedOrder) = await _purchaseOrderService.UpdateStatusAsync(id, status, actualDeliveryDate);
            if (!ok)
            {
                TempData["Error"] = err;
            }
            else
            {
                TempData["Success"] = "Cập nhật trạng thái thành công!";
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        private async Task LoadProductsToViewBag()
        {
            var products = await _evmService.GetAllProductsAsync();
            ViewBag.Products = products;
        }
    }
}

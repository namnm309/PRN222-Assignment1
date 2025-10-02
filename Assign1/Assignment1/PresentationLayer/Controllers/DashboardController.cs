using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class DashboardController : BaseDashboardController
    {
        public IActionResult Index()
        {
            // ViewBag đã được set tự động từ BaseDashboardController
            return View();
        }

        public IActionResult Products()
        {
            // Ví dụ trang Products - tự động có sidebar và layout
            return View();
        }

        public IActionResult Orders()
        {
            // Ví dụ trang Orders
            return View();
        }

        public IActionResult Reports()
        {
            // Ví dụ trang Reports
            return View();
        }

        public IActionResult EVMDashboard()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập EVM Dashboard.";
                return RedirectToAction("Index", "Dashboard");
            }
            
            return RedirectToAction("Index", "EVMDashboard");
        }
    }
}

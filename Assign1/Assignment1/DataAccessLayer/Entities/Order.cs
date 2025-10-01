using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Order : BaseEntity
    {

        //Khóa ngoại cho bảng Customer, Dealer, Product
        public Guid CustomerId { get; set; }
        public Guid DealerId { get; set; }
        public Guid ProductId { get; set; }

        
        public virtual Customer Customer { get; set; } //Trong order chứa thông tin khách hàng 

        public virtual Dealer Dealer { get; set; } //Trong order chứa Id của đại lý 

        public virtual Product Product { get; set; } //Trong order chứa Id của sản phẩm

        public string Description { get; set; }

        public double price { get; set; }

        public double discount { get; set; }

        public string status { get; set; } = "Pending"; //Pending, Completed, Canceled : 3 trạng thái của đơn hàng 

    }

}

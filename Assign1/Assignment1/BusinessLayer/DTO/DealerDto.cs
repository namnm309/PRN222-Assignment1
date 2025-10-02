using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTO
{
    public class DealerDto
    {
        public  Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
    public class CreateDealerDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

}

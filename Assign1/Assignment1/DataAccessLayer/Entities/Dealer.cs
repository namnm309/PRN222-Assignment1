using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Dealer : BaseEntity
    {
        //Đại lý

        public string Name { get; set; }

        public string phone { get; set; }

        public string Address { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

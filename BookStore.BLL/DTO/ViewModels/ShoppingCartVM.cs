using BookStore.BLL.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.DTO
{
    public class ShoppingCartVM
    {
        public IEnumerable<DTO.ShoppingCart> ShoppingCartList { get; set; }
        public DTO.Requests.OrderHeaderAddEditRequestModel OrderHeader { get; set; }
    }
}

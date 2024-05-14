using BookStore.BLL.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.DTO
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<BLL.DTO.ShoppingCart> ShoppingCartList { get; set; }
        public BLL.DTO.OrderHeader OrderHeader { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Common.Exceptions
{
    public class ShoppingCartsAppException(string message) : Exception(message)
    {
    }

    public class EntityNotFoundException : ShoppingCartsAppException
    {
        public EntityNotFoundException() : base($"Cart not found")
        {
        }
    }

    public class FailedToRetrieveShoppingCartsException : ShoppingCartsAppException
    {
        public FailedToRetrieveShoppingCartsException() : base($"Failed to retrieve shopping carts")
        {
        }
    }
}

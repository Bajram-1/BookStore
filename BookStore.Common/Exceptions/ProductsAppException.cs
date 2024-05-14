using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Common.Exceptions
{
    public class ProductsAppException : Exception
    {
        public ProductsAppException(string message) : base(message) { }
    }

    public class DuplicateISBNException : ProductsAppException
    {
        public DuplicateISBNException() : base("This ISBN already exists. Check it again please!") { }
    }

    public class DuplicateTitleAndAuthorException : ProductsAppException
    {
        public DuplicateTitleAndAuthorException() : base("This title and author already exist. Check it again please!") { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Net2025
{
    //This class represents a DTO used for book lends and returns
    class BookLend
    {
        public long bookId { get; private set; }
        public int quantity { get; private set; }

        public BookLend(long bookId, int quantity)
        {
            this.bookId = bookId;
            this.quantity = quantity;
        }
    }
}

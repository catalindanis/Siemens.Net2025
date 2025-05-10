using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Net2025
{
    //Model definition
    class Book
    {
        //Keep the encapsulation, by making the setter private
        public long id { get; private set;  }
        public string title { get; set; }
        public string author { get; set; }
        public int quantity { get; set; }
        
        /*
         * The constructor of a book
         * @id : it's id (Long)
         * @title : it's title (string)
         * @author : it's author (string)
         * @quantity : it's quantity (int)
         */
        public Book(long id, string title, string author, int quantity)
        {
            this.id = id;
            this.title = title;
            this.author = author;
            this.quantity = quantity;
        }

        /*
         * This function increases the quantity of a book
         * @count : the increase factor (int)
         */
        public void IncreaseQuantity(int count)
        {
            this.quantity += count;
        }

        /*
         * This function decreases the quantity of a book
         * @count : the decrease factor (int)
         */
        public void DecreaseQuantity(int count)
        {
            this.quantity -= count;
        }

        public bool Equals(Book book)
        {
            return this.id == book.id &&
                    this.title.Equals(book.title) &&
                    this.author.Equals(book.author) &&
                    this.quantity == book.quantity;
        }
    }
}

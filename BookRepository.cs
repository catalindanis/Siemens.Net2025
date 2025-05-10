using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Net2025
{
    //The repository of books
    class BookRepository
    {
        private List<Book> books;
        private string dbFileName;

        /*
         * Default constructor
         */
        public BookRepository()
        {
            this.books = new List<Book>();
            this.dbFileName = "database.txt";
            this.Load();
        }

        /*
         * Custom persistence filename constructor
         * @dbFileName : the filename of the database file (string)
         */
        public BookRepository(string dbFileName)
        {
            this.books = new List<Book>();
            this.dbFileName = dbFileName;
            this.Load();
        }

        /*
         * This function adds a book to the repository
         * if the book already exists, it only increase it's quantity
         * otherwise, it adds the book to the list
         * @book : the book to be added (Book)
         */
        public void Add(Book book)
        {
            try
            {
                int position = this.GetPosition(book);
                this.books[position].IncreaseQuantity(book.quantity);
            }
            catch (Exception)
            {
                this.books.Add(book);
            }
            this.Save();
        }

        /*
         * This function deletes a book from the repository
         * @id : the id of the book to be deleted (long)
         * @throws : Exception (if a book with this id doesn't exists)
         */
        public void DeleteById(long id)
        {
            int position = this.GetPositionById(id);
            this.books.RemoveAt(position);
            this.Save();
        }

        /*
        * This function deletes a certain amount of books from the repository
        * @id : the id of the book to be deleted (long)
        * @count : the number of books to be deleted (int)
        * @throws : Exception (if count is bigger than the actual quantity of books)
        *           Exception (if a book with this id doesn't exists)
        */
        public void DeleteById(long id, int count)
        {
            int position = this.GetPositionById(id);
            if (this.books[position].quantity >= count)
                this.books[position].DecreaseQuantity(count);
            else
                throw new Exception("Not enough books with this id!");
            this.Save();
        }

        /*
        * This function updates a book with a coresponding id from the repository
        * @id : the id of the book to be updated (long)
        * @newBook : the new book (Book)
        * @throws : Exception (if a book with this id doesn't exists)
        */
        public void UpdateById(long id, Book newBook)
        {
            int position = GetPositionById(id);
            this.books[position].title = newBook.title;
            this.books[position].author = newBook.author;
            this.books[position].quantity = newBook.quantity;
            this.Save();
        }

        /*
         * This function returns the list of books from the repository
         * @return : the list of books (List<Book>)
         */
        public List<Book> GetAll()
        {
            return this.books;
        }

        /*
         * This function returns a book by it's id from the repository
         * @return : the book (Book)
         * @throws : Exception (if a book with this id doesn't exists)
         */
        public Book GetById(long id)
        {
            foreach(Book book in this.books)
            {
                if (book.id == id)
                    return book;
            }
            throw new Exception("A book with this id doesn't exists!");
        }

        /*
         * This function returns the number of books from the repository
         * @return : the size of the list (int)
         */
        public int Count()
        {
            return this.books.Count();
        }

        /*
         * This function saves all the books from the repository to the database
         */
        private void Save()
        {
            this.EnsureFileExists();
            using (StreamWriter writer = new StreamWriter(this.dbFileName, false)) 
            {
                foreach(Book book in this.books)
                {
                    writer.WriteLine(this.formatToWrite(book));
                }
                writer.Close();
            }
        }

        /*
         * This function loads all the books to the repository from the database
         */
        private void Load()
        {
            this.EnsureFileExists();
            using (StreamReader reader = new StreamReader(this.dbFileName))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        this.books.Add(bookFromWriteFormat(line));
                    }
                    catch (Exception) { }
                }
                reader.Close();
            }
        }

        /*
         * This function ensure that the database file exists on disk,
         * otherwise it creates it
         */
        private void EnsureFileExists()
        {
            if (!File.Exists(this.dbFileName)) { using (File.Create(this.dbFileName)) { } }
        }

        /*
         * This function formats a book to the database format
         * @return : the formatted book (string)
         */
        private string formatToWrite(Book book)
        {
            return book.id + "," + book.title.Trim() + "," + book.author.Trim() + "," + book.quantity;
        }

        /*
         * This function transforms a formatted string from db into a book
         * @line : the formatted string (string)
         * @returns : the created book (Book)
         * @throws : Exception (if the database is corrupted)
         */
        private Book bookFromWriteFormat(string line)
        {
            string[] values = line.Split(",");
            try
            {
                long id = (long) Convert.ToDouble(values[0].Trim());
                string title = values[1].Trim();
                string author = values[2].Trim();
                int quantity = Convert.ToInt32(values[3].Trim());
                Book book = new Book(id, title, author, quantity);
                return book;
            }
            catch (Exception)
            {
                throw new Exception("Corrupted database!");
            }
        }

        /*
         * This function returns the book position in repository list by it's id
         * @return : the position (int)
         * @throws : Exception (if a book with this id doesn't exists)
         */
        private int GetPositionById(long id)
        {
            for (int i = 0; i < this.books.Count(); i++)
            {
                if (this.books[i].id == id)
                    return i;
            }
            throw new Exception("A book with this id doesn't exists!");
        }

        /*
         * This function returns the book position in repository list
         * @return : the book (Book)
         * @throws : Exception (if the book doesn't exists)
         */
        private int GetPosition(Book book)
        {
            for(int i=0;i<this.books.Count(); i++)
            {
                if (this.books[i].title == book.title &&
                    this.books[i].author == book.author)
                    return i;
            }
            throw new Exception("This book doesn't exists!");
        }
    }
}

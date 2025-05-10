using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Net2025
{
    //The service of books
    class BookService
    {
        private BookRepository repository;
        private List<BookLend> lends;
        private string lendsDbFileName;
        private string configFileName;
        public int criticalStockTreshold { get; private set; } = 10; //Default value if the config file is corrupted

        //Default constructor
        public BookService(BookRepository repository)
        {
            this.repository = repository;
            this.lends = new List<BookLend>();
            this.lendsDbFileName = "lendsdatabase.txt";
            this.loadLends();
            this.configFileName = "config.txt";
            this.loadConfigFile();
        }

        /*
         * Custom persistence filename constructor
         * @lendsDbFileName : the filename of the lends database file (string)
         */
        public BookService(BookRepository repository, string lendsDbFileName)
        {
            this.repository = repository;
            this.lends = new List<BookLend>();
            this.lendsDbFileName = lendsDbFileName;
            this.loadLends();
            this.configFileName = "config.txt";
            this.loadConfigFile();
        }

        /*
         * This function validates the input fields, creates a book
         * and add it to the repository
         * @title : the title of the book (string)
         * @author : the author of the book (string)
         * @quantity : the quantity of the book (string)
         * @throws : Exception (if fields are not valid)
         */
        public void Add(string title, string author, string quantity)
        {
            if (!Validator.ValidateTitle(title))
                throw new Exception("Invalid title!");
            if (!Validator.ValidateAuthor(author))
                throw new Exception("Invalid author!");
            if (!Validator.ValidateQuantity(quantity))
                throw new Exception("Invalid quantity!");

            int quantityConvert = Convert.ToInt32(quantity);
            this.repository.Add(new Book(GenerateId(), title, author, quantityConvert));
        }

        /*
         * This function validates the id and deletes a book from the repository by it's id
         * @id : the id of the book (string)
         * @throws : Exception (the id is not valid or the book doesn't exists)
         */
        public void Delete(string id)
        {
            if (!Validator.ValidateId(id))
                throw new Exception("Invalid id!");

            long idConvert = (long) Convert.ToDouble(id);
            this.repository.DeleteById(idConvert);
        }

        /*
         * This function validates the fields and updates a book from the repository by it's id
         * with the new book created
         * @id : the id of the book (string)
         * @title : the title of the book (string)
         * @author : the author of the book (string)
         * @quantity : the quantity of the book (string)
         * @throws : Exception (if fields are not valid or the book doesn't exists)
         */
        public void Update(string id, string title, string author, string quantity)
        {
            if (!Validator.ValidateId(id))
                throw new Exception("Invalid id!");
            if (!Validator.ValidateTitle(title))
                throw new Exception("Invalid title!");
            if (!Validator.ValidateAuthor(author))
                throw new Exception("Invalid author!");
            if (!Validator.ValidateQuantity(quantity))
                throw new Exception("Invalid quantity!");

            long idConvert = (long) Convert.ToDouble(id);
            int quantityConvert = Convert.ToInt32(quantity);
            this.repository.UpdateById(idConvert, new Book(idConvert, title, author, quantityConvert));
        }

        /*
         * This function validates the id and returns all the books that have that id
         * @id : the id (string)
         * @return : the list of books (List<Book>)
         * @throws : Exception (if id is invalid)
         */
        public List<Book> SearchById(string id)
        {
            if (!Validator.ValidateId(id))
                throw new Exception("Invalid id!");

            return this.GetAll().Where(b => b.id == (long) Convert.ToDouble(id)).ToList();
        }

        /*
         * This function validates the title and returns all the books that contains
         * the substring title in their title's
         * @title : the title (string)
         * @return : the list of books (List<Book>)
         * @throws : Exception (if title is invalid)
         */
        public List<Book> SearchByTitle(string title)
        {
            if (!Validator.ValidateTitle(title))
                throw new Exception("Invalid title!");

            return this.GetAll().Where(b => b.title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /*
         * This function validates the author and returns all the books that contains
         * the substring author in their author's
         * @author : the author (string)
         * @return : the list of books (List<Book>)
         * @throws : Exception (if author is invalid)
         */
        public List<Book> SearchByAuthor(string author)
        {
            if (!Validator.ValidateAuthor(author))
                throw new Exception("Invalid author!");

            return this.GetAll().Where(b => b.author.Contains(author, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /*
         * This function validates the quantity and returns all the books that have that quantity
         * @quantity : the quantity (string)
         * @return : the list of books (List<Book>)
         * @throws : Exception (if quantity is invalid)
         */
        public List<Book> SearchByQuantity(string quantity)
        {
            if (!Validator.ValidateQuantity(quantity))
                throw new Exception("Invalid quantity!");

            return this.GetAll().Where(b => b.quantity == Convert.ToInt32(quantity)).ToList();
        }

        /*
         * This function validates the fields and creates a lend object over a book
         * @id : the id of the book (string)
         * @quantity : the quantity of the lend (string)
         * @throws : Exception (if fields are not valid, book with that id doesn't exists or the quantity
         *           of the lend is bigger than the stock)
         */
        public void Lend(string id, string quantity)
        {
            if (!Validator.ValidateId(id))
                throw new Exception("Invalid id!");
            if (!Validator.ValidateQuantity(quantity))
                throw new Exception("Invalid quantity!");

            long idConvert = (long)Convert.ToDouble(id);
            int quantityConvert = Convert.ToInt32(quantity);
            this.repository.DeleteById(idConvert, quantityConvert);
            this.lends.Add(new BookLend(idConvert, quantityConvert));
            this.saveLends();
        }

        /*
         * This function returns the list of lends
         * @return : the list of lends (List<BookLend>)
         */
        public List<BookLend> GetBookLends()
        {
            return this.lends;
        }

        /*
         * This function validates the id and returns a lend with that id
         * @id : the id of the lend (string)
         * @throws : Exception (if a lend with that id doesn't exists)
         */
        public void Return(string id)
        {
            try
            {
                int idConvert = Convert.ToInt32(id);
                if (idConvert < 0 || idConvert >= this.lends.Count())
                    throw new Exception();

                Book book = this.repository.GetById(this.lends[idConvert].bookId);
                book.IncreaseQuantity(this.lends[idConvert].quantity);
                this.repository.UpdateById(this.lends[idConvert].bookId, book);
                this.lends.RemoveAt(idConvert);
            }
            catch(Exception)
            {
                throw new Exception("Invalid id!");
            }
            this.saveLends();
        }

        /*
         * This function loads all the book lends to the service from the database
         */
        private void loadLends()
        {
            this.EnsureFileExists(this.lendsDbFileName);
            using (StreamReader reader = new StreamReader(this.lendsDbFileName))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        this.lends.Add(lendFromWriteFormat(line));
                    }
                    catch (Exception) { }
                }
                reader.Close();
            }
        }

        /*
         * This function formats a book lend to the database format
         * @return : the formatted lend (string)
         */
        private BookLend lendFromWriteFormat(string line)
        {
            string[] values = line.Split(",");
            try
            {
                long bookId = (long)Convert.ToDouble(values[0].Trim());
                int quantity = Convert.ToInt32(values[1].Trim());
                return new BookLend(bookId, quantity);
            }
            catch (Exception)
            {
                throw new Exception("Corrupted database!");
            }
        }

        /*
         * This function saves all the book lends from the service to the database
         */
        private void saveLends()
        {
            this.EnsureFileExists(this.lendsDbFileName);
            using (StreamWriter writer = new StreamWriter(this.lendsDbFileName, false))
            {
                foreach (BookLend lend in this.lends)
                {
                    writer.WriteLine(this.formatToWrite(lend));
                }
                writer.Close();
            }
        }

        /*
         * This function converts a book lend into a string for the db
         * @lend : the lend (BookLend)
         * @return : the formatted string (string)
         */
        private string formatToWrite(BookLend lend)
        {
            return lend.bookId + "," + lend.quantity;
        }

        /*
         * This function ensure that the database file exists on disk,
         * otherwise it creates it
         * @filename : the name of the file (string)
         */
        private void EnsureFileExists(string filename)
        {
            if (!File.Exists(filename)) { using (File.Create(filename)) { } }

        }

        /*
         * Custom Functionality
         * The function returns the list of books that have the quantity less than the criticalStockTreshold
         * in ascending order
         * @return : the list of books (List<Book)
         */
        public List<Book> getCriticalStockBooks()
        {
            return this.GetAll().Where(b => b.quantity < this.criticalStockTreshold).OrderBy(b => b.quantity).ToList();
        }

        /*
         * This function returns the book list from the repository
         * @return : the list of books (List<Book)
         */
        public List<Book> GetAll()
        {
            return this.repository.GetAll();
        }

        /*
         * This function returns the book from the repository by it's id
         * @return : the book (Book)
         * @throws : Exception (if a book with that id doesn't exists)
         */
        public Book GetById(long id)
        {
            return this.repository.GetById(id);
        }

        /*
         * This function generates an id for a book based on the current maximum id in repository
         * @return : the generated id (long)
         */
        private long GenerateId()
        {
            long max = 0;
            foreach (Book book in this.repository.GetAll())
                if (book.id > max)
                    max = book.id;
            return max + 1;
        }

        /*
         * This function saves the config file to the disk
         */
        private void saveConfigFile()
        {
            this.EnsureFileExists(this.configFileName);
            using (StreamWriter writer = new StreamWriter(this.configFileName, false))
            {
                writer.WriteLine("criticalStockTreshold = " + this.criticalStockTreshold);
                writer.Close();
            }
        }

        /*
         * The setter of the critical treshold
         * @treshold : the new value of critical treshold (string)
         * @throws : Exception (if the input is invalid)
         */
        public void setCriticalTreshold(string treshold)
        {
            try
            {
                int tresholdConvert = Convert.ToInt32(treshold);
                if (tresholdConvert < 0)
                    throw new Exception();
                this.criticalStockTreshold = tresholdConvert;
                this.saveConfigFile();
            }
            catch (Exception)
            {
                throw new Exception("Invalid treshold format!");
            }
        }

        /*
         * This function loads the config file from the disk
         * and initializes the corresponding values
         */
        private void loadConfigFile()
        {
            this.EnsureFileExists(this.configFileName);
            using (StreamReader reader = new StreamReader(this.configFileName))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        switch (line.Split("=")[0].Trim())
                        {
                            case "criticalStockTreshold":
                                criticalStockTreshold = Convert.ToInt32(line.Split("=")[1].Trim());
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception("Corrupted config file!");
                    }
                }
                reader.Close();
            }
        }
    }
}

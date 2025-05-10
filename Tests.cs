using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Net2025
{
    class Tests
    {
        //The function runs all the tests from the application
        public static void runAllTests()
        {
            runDomainTests();
            runRepositoryTests();
            runValidatorTests();
            runServiceTests();
        }

        //The function runs the tests for the model
        private static void runDomainTests()
        {
            Book book = new Book(1, "title", "author", 1);
            Debug.Assert(book.id == 1);
            Debug.Assert(book.title.Equals("title"));
            Debug.Assert(book.author.Equals("author"));
            Debug.Assert(book.quantity.Equals(1));
        }

        //The function runs the tests for the repository
        private static void runRepositoryTests()
        {
            File.Delete("tests.txt");
            using (File.Create("tests.txt")) {}
            using (StreamWriter writer = new StreamWriter("tests.txt", false)) {
                writer.WriteLine("1,title,author,1");
            }

            BookRepository repository = new BookRepository("tests.txt");
            Book book = new Book(1, "title", "author", 1);

            Debug.Assert(repository.Count() == 1);
            Debug.Assert(repository.GetById(1).Equals(book));

            repository.Add(book);
            Debug.Assert(repository.Count() == 1);
            Debug.Assert(repository.GetById(1).quantity == 2);

            Book book2 = new Book(2, "title2", "author2", 2);
            repository.UpdateById(1, book2);

            Book book3 = new Book(1, "title2", "author2", 2);
            Debug.Assert(repository.Count() == 1);
            Debug.Assert(repository.GetById(1).Equals(book3));

            try
            {
                repository.UpdateById(2, book3);
                Debug.Assert(false);
            }
            catch (Exception) { }

            repository.DeleteById(1, 2);
            Debug.Assert(repository.GetById(1).quantity == 0);

            try
            {
                repository.DeleteById(1, 1);
                Debug.Assert(false);
            }
            catch (Exception) { }

            repository.DeleteById(1);
            Debug.Assert(repository.Count() == 0);

            try
            {
                repository.DeleteById(1);
                Debug.Assert(false);
            }
            catch (Exception) { }

            List<Book> books = new List<Book>();
            books.Add(new Book(1, "test1", "test1", 1));
            books.Add(new Book(2, "test2", "test2", 2));
            books.Add(new Book(3, "test3", "test3", 3));
            books.Add(new Book(4, "test4", "test4", 4));

            repository.Add(new Book(1, "test1", "test1", 1));
            repository.Add(new Book(2, "test2", "test2", 2));
            repository.Add(new Book(3, "test3", "test3", 3));
            repository.Add(new Book(4, "test4", "test4", 4));

            Debug.Assert(repository.Count() == 4);

            int position = 0;
            foreach(Book b in repository.GetAll())
            {
                Debug.Assert(b.Equals(books[position++]));
            }

            for (int i = 0; i < 4; i++)
                repository.DeleteById(i + 1);

            Debug.Assert(repository.Count() == 0);

            repository.Add(new Book(1, "title", "author", 1));
        }
    
        //The function runs the tests for the validator
        private static void runValidatorTests()
        {
            Debug.Assert(Validator.ValidateId("") == false);
            Debug.Assert(Validator.ValidateId("1") == true);
            Debug.Assert(Validator.ValidateId("1.0") == false);
            Debug.Assert(Validator.ValidateId("-2") == false);

            Debug.Assert(Validator.ValidateTitle("") == false);
            Debug.Assert(Validator.ValidateTitle("Test") == true);
            Debug.Assert(Validator.ValidateTitle("Test-!.l143 ?;:") == true);
            Debug.Assert(Validator.ValidateTitle("Test@") == false);

            Debug.Assert(Validator.ValidateAuthor("") == false);
            Debug.Assert(Validator.ValidateAuthor("Author") == true);
            Debug.Assert(Validator.ValidateAuthor("Firstname lastname") == true);
            Debug.Assert(Validator.ValidateAuthor("author's") == true);

            Debug.Assert(Validator.ValidateQuantity("") == false);
            Debug.Assert(Validator.ValidateQuantity("1") == true);
            Debug.Assert(Validator.ValidateQuantity("1.0") == false);
            Debug.Assert(Validator.ValidateQuantity("-2") == false);
        }

        //The function runs the tests for the service
        private static void runServiceTests()
        {
            File.Delete("tests.txt");
            using (File.Create("tests.txt")) { }

            BookRepository repository = new BookRepository("tests.txt");
            BookService service = new BookService(repository, "tests.txt");

            Debug.Assert(service.GetAll().Count() == 0);

            service.Add("title1", "author-", "1");
            service.Add("title2", "author--", "2");
            service.Add("title3", "author---", "3");
            service.Add("title4", "author----", "4");

            Debug.Assert(service.GetAll().Count() == 4);

            try
            {
                service.Add("title@", "author", "1");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("Invalid title!"));
            }

            try
            {
                service.Add("title", "author1", "1");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("Invalid author!"));
            }

            try
            {
                service.Add("title", "author", "one");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("Invalid quantity!"));
            }

            try
            {
                service.Add("title", "author", "-1");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("Invalid quantity!"));
            }

            Debug.Assert(service.GetAll().Count() == 4);

            int expectedId = 1;
            foreach (Book book in service.GetAll())
            {
                Debug.Assert(book.id == expectedId++);
            }

            service.Delete("1");
            Debug.Assert(service.GetAll().Count() == 3);

            try
            {
                service.Delete("1");
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message == "A book with this id doesn't exists!");
            }

            service.Update("2", "title1", "author-", "1");
            service.Update("3", "title2", "author--", "2");
            service.Update("4", "title3", "author---", "3");

            Debug.Assert(service.GetAll().Count() == 3);

            try
            {
                service.Update("5", "title", "author", "1");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("A book with this id doesn't exists!"));
            }

            try
            {
                service.Update("2", "title@", "author", "1");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("Invalid title!"));
            }

            try
            {
                service.Update("2", "title", "author1", "1");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("Invalid author!"));
            }

            try
            {
                service.Update("2", "title", "author", "one");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("Invalid quantity!"));
            }

            try
            {
                service.Update("2", "title", "author", "-1");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("Invalid quantity!"));
            }

            Debug.Assert(service.SearchById("1").Count() == 0);
            Debug.Assert(service.SearchById("2").Count() == 1);
            Debug.Assert(service.SearchById("3").Count() == 1);
            Debug.Assert(service.SearchById("4").Count() == 1);

            Debug.Assert(service.SearchByTitle("Title").Count() == 3);
            Debug.Assert(service.SearchByTitle("title").Count() == 3);
            Debug.Assert(service.SearchByTitle("titLe1").Count() == 1);
            Debug.Assert(service.SearchByTitle("Title2").Count() == 1);

            Debug.Assert(service.SearchByAuthor("Author-").Count() == 3);
            Debug.Assert(service.SearchByAuthor("auThor--").Count() == 2);
            Debug.Assert(service.SearchByAuthor("author---").Count() == 1);
            Debug.Assert(service.SearchByAuthor("author----").Count() == 0);

            Debug.Assert(service.SearchByQuantity("1").Count() == 1);
            Debug.Assert(service.SearchByQuantity("2").Count() == 1);
            Debug.Assert(service.SearchByQuantity("3").Count() == 1);
            Debug.Assert(service.SearchByQuantity("4").Count() == 0);

            try
            {
                service.SearchById("one");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("Invalid id!"));
            }

            try
            {
                service.SearchByTitle("@Title");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("Invalid title!"));
            }

            try
            {
                service.SearchByAuthor("Author1");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("Invalid author!"));
            }

            try
            {
                service.SearchByQuantity("-1");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("Invalid quantity!"));
            }

            //"2", "title1", "author-", "1"
            //"3", "title2", "author--", "2"
            //"4", "title3", "author---", "3"
            service.Lend("4", "2");

            Debug.Assert(service.GetAll()[2].quantity == 1);
            Debug.Assert(service.GetBookLends().Count() == 1);
            Debug.Assert(service.GetBookLends()[0].bookId == 4);
            Debug.Assert(service.GetBookLends()[0].quantity == 2);

            try
            {
                service.Lend("2", "3");
                Debug.Assert(false);
            }
            catch(Exception e)
            {
                Debug.Assert(e.Message.Equals("Not enough books with this id!"));
            }

            try
            {
                service.Lend("5", "3");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("A book with this id doesn't exists!"));
            }

            Debug.Assert(service.GetBookLends().Count() == 1);

            try
            {
                service.Return("1");
                Debug.Assert(false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Equals("Invalid id!"));
            }

            service.Return("0");

            Debug.Assert(service.GetAll()[2].quantity == 3);
            Debug.Assert(service.GetBookLends().Count() == 0);
        }
    }
}

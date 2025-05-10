using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Net2025
{
    //The Ui of the application
    class Ui
    {
        BookService service;
        bool exitRequested;
        string operationResultMessage;
        public Ui(BookService service)
        {
            this.service = service;
            this.exitRequested = false;
            this.operationResultMessage = "";
        }

        //Main function of the Ui
        public void Run()
        {
            string? input;
            while (!this.exitRequested)
            {
                Console.Clear();
                Display();
                Console.Write(">>");
                input = Console.ReadLine();
                HandleInput(input);
            }
            Console.WriteLine("Bye, bye!");
        }

        //The function uses the service to display the books list
        private void DisplayAll()
        {
            DisplayList(this.service.GetAll());
        }

        //The function uses the service to add a book to the books list
        private void Add()
        {
            Console.Write("Title: ");
            string? title = Console.ReadLine();
            Console.Write("Author: ");
            string? author = Console.ReadLine();
            Console.Write("Quantity: ");
            string? quantity = Console.ReadLine();


            if (title == null || author == null || quantity == null)
                return;

            try
            {
                this.service.Add(title, author, quantity);
                this.operationResultMessage = "The book was added successfully";
            }
            catch (Exception e)
            {
                this.operationResultMessage = e.Message;
            }
        }

        //The function uses the service to delete a book by it's id
        private void Delete()
        {
            Console.Write("Id: ");
            string? id = Console.ReadLine();

            if (id == null)
                return;

            try
            {
                this.service.Delete(id);
                this.operationResultMessage = "The book was deleted successfully";
            }
            catch (Exception e)
            {
                this.operationResultMessage = e.Message;
            }
        }

        //The function uses the service to update a book by it's id
        private void Update()
        {
            Console.Write("Id: ");
            string? id = Console.ReadLine();
            Console.Write("Title: ");
            string? title = Console.ReadLine();
            Console.Write("Author: ");
            string? author = Console.ReadLine();
            Console.Write("Quantity: ");
            string? quantity = Console.ReadLine();


            if (id == null || title == null || author == null || quantity == null)
                return;

            try
            {
                this.service.Update(id, title, author, quantity);
                this.operationResultMessage = "The book was updated successfully";
            }
            catch (Exception e)
            {
                this.operationResultMessage = e.Message;
            }
        }

        //The function uses the service to search a book by it's id
        private void Search()
        {
            Console.WriteLine("1.Id");
            Console.WriteLine("2.Title");
            Console.WriteLine("3.Author");
            Console.WriteLine("4.Quantity");
            Console.WriteLine("(or anything else to go back)");

            string? input = Console.ReadLine();

            if (input == null)
                return;

            switch (input)
            {
                case "1":
                    Console.Write("Id: ");
                    input = Console.ReadLine();

                    if (input == null)
                        return;

                    try
                    {
                        DisplayList(this.service.SearchById(input));
                    }
                    catch(Exception e)
                    {
                        this.operationResultMessage = e.Message;
                    }
                    break;
                case "2":
                    Console.Write("Title: ");
                    input = Console.ReadLine();

                    if (input == null)
                        return;

                    try
                    {
                        DisplayList(this.service.SearchByTitle(input));
                    }
                    catch (Exception e)
                    {
                        this.operationResultMessage = e.Message;
                    }
                    break;
                case "3":
                    Console.Write("Author: ");
                    input = Console.ReadLine();

                    if (input == null)
                        return;

                    try
                    {
                        DisplayList(this.service.SearchByAuthor(input));
                    }
                    catch (Exception e)
                    {
                        this.operationResultMessage = e.Message;
                    }
                    break;
                case "4":
                    Console.Write("Quantity: ");
                    input = Console.ReadLine();

                    if (input == null)
                        return;

                    try
                    {
                        DisplayList(this.service.SearchByQuantity(input));
                    }
                    catch (Exception e)
                    {
                        this.operationResultMessage = e.Message;
                    }
                    break;
                default:
                    break;
            }
        }

        //The function uses the service to lend a book by it's id and a quantity
        private void Lend()
        {
            Console.Write("Book id: ");
            string? id = Console.ReadLine();
            Console.Write("Quantity: ");
            string? quantity = Console.ReadLine();

            if (id == null || quantity == null)
                return;

            try
            {
                this.service.Lend(id, quantity);
                this.operationResultMessage = "Lend successfully created!";
            }
            catch (Exception e)
            {
                this.operationResultMessage = e.Message;
            }
        }

        //The function uses the service to return a book lend by it's id
        private void Return()
        {
            Console.Write("Book lend id: ");
            string? id = Console.ReadLine();

            if (id == null)
                return;

            try
            {
                this.service.Return(id);
                this.operationResultMessage = "Lend successfully returned!";
            }
            catch (Exception e)
            {
                this.operationResultMessage = e.Message;
            }
        }

        //The function uses the service to display all lends
        private void DisplayAllLends()
        {
            this.operationResultMessage = "Lend id | Quantity | Book title";
            int lendIndex = 0;
            foreach(BookLend lend in this.service.GetBookLends())
            {
                this.operationResultMessage += "\n";
                this.operationResultMessage += String.Format("Lend: #{0} | Quantity: {1} | Book title: {2}", 
                                                lendIndex++,
                                                lend.quantity,
                                                this.service.GetById(lend.bookId).title);
            }
            if (lendIndex == 0)
                this.operationResultMessage = "The lends list is empty!";
        }

        //Custom functionality
        //The function uses the service to display all books that have a critic stock
        private void DisplayCriticalStocks()
        {
            Console.WriteLine("Current critical stocks treshold = " + this.service.criticalStockTreshold);
            Console.WriteLine("1.Display critical books");
            Console.WriteLine("2.Set critical stocks treshold");
            Console.WriteLine("(or anything else to go back)");
            Console.WriteLine(">>");

            string? input = Console.ReadLine();

            if (input == null)
                return;

            switch (input)
            {
                case "1":
                    this.DisplayList(this.service.getCriticalStockBooks());
                    break;
                case "2":
                    try
                    {
                        Console.Write("New treshold: ");
                        input = Console.ReadLine();

                        if (input == null)
                            return;

                        this.service.setCriticalTreshold(input);
                        this.operationResultMessage = "Treshold changed successfully";
                    }
                    catch(Exception e)
                    {
                        this.operationResultMessage = e.Message;
                    }
                    break;
                default:
                    break;
            }
        }

        /*
         * The function handles the input of the user
         * @input : the input of the user (string)
         */
        private void HandleInput(string? input)
        {
            this.operationResultMessage = "";
            if (input == null)
                return;
            switch (input)
            {
                case "0":
                    DisplayAll();
                    break;
                case "1":
                    Add();
                    break;
                case "2":
                    Delete();
                    break;
                case "3":
                    Update();
                    break;
                case "4":
                    Lend();
                    break;
                case "5":
                    Return();
                    break;
                case "6":
                    Search();
                    break;
                case "7":
                    DisplayAllLends();
                    break;
                case "8":
                    DisplayCriticalStocks();
                    break;
                case "9":
                    this.exitRequested = true;
                    break;
                default:
                    this.operationResultMessage = "Invalid option!";
                    break;
            }
        }

        //The function that displays the menu and the last operation result
        private void Display()
        {
            Console.WriteLine("~Library management system~");
            Console.WriteLine("0.Display all");
            Console.WriteLine("1.Add");
            Console.WriteLine("2.Delete");
            Console.WriteLine("3.Update");
            Console.WriteLine("4.Lend");
            Console.WriteLine("5.Return");
            Console.WriteLine("6.Search");
            Console.WriteLine("7.Display all lends");
            Console.WriteLine("8.Critical stocks");
            Console.WriteLine("9.Exit");
            if (!this.operationResultMessage.Equals(""))
                Console.WriteLine(this.operationResultMessage);
        }

        //This function displays a book list in a formatted message
        private void DisplayList(List<Book> list)
        {
            if (list.Count() > 0)
            {
                this.operationResultMessage = "Id | Title | Author | Quantity";
                foreach (Book book in list)
                {
                    this.operationResultMessage += "\n";
                    this.operationResultMessage += FormatBookToPrint(book);
                }
            }
            else
                this.operationResultMessage = "The books list is empty!";
        }

        /*
         * The function returns a formatted book for display
         * @return : the formatted string (string)
         */
        private string FormatBookToPrint(Book book)
        {
            return book.id + " | " + book.title + " | " + book.author + " | " + book.quantity;
        }
    }
}

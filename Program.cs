namespace Siemens.Net2025
{
    class Program
    {
        //Main function of the application
        static void Main(string[] args)
        {
            Tests.runAllTests();

            BookRepository repository = new BookRepository();
            BookService service = new BookService(repository);

            Ui ui = new Ui(service);
            ui.Run();
        }
    }
}

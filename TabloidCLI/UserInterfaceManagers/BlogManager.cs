
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    public class BlogManager : IUserInterfaceManager
    {
        private readonly IUserInterfaceManager _parentUI;
        private BlogRepository _blogRepository;
        private string _connectionString;

        public BlogManager(IUserInterfaceManager parentUI, string connectionString)
        {
            _connectionString = connectionString; 
            _parentUI = parentUI;
            _blogRepository = new BlogRepository(connectionString);
        }

        public IUserInterfaceManager Execute()
        {

            Console.WriteLine("Blog Menu");
            Console.WriteLine(" 1) List Blogs");
            Console.WriteLine(" 2) Blog Details");
            Console.WriteLine(" 3) Add Blog");
            Console.WriteLine(" 4) Edit Blog");
            Console.WriteLine(" 5) Remove Blog");
            Console.WriteLine(" 0) Go Back"); 

            Console.WriteLine("> ");
            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    List(); 
                    return this;
                case "2":
                    Blog blog = Choose();
                    if (blog == null) return this; 
                    else
                    {
                        //TODO - return new BlogDetailsManager
                        return this; 
                    }
                case "3":
                    Add();
                    return this;
                case "4":
                    Edit();
                    return this;
                case "5":
                    Remove();
                    return this;
                case "0":
                    return _parentUI; 
                default:
                    Console.WriteLine("Invalid selection");
                    return this; 
            }
        }

        private Blog Choose(string? prompt = null)
        {
            if (prompt == null)
            {
                prompt = "Please choose a Blog:";
            }

            Console.WriteLine(prompt);

            List<Blog> blogs = _blogRepository.GetAll(); 

            for (int i = 0; i < blogs.Count; i++)
            {
                Blog blog = blogs[i];
                Console.WriteLine($" {i + 1} {blog.Title}");
            }
            Console.Write("> ");

            string response = Console.ReadLine() ?? ""; 

            try
            {
                int choice = int.Parse(response);
                return blogs[choice - 1]; 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid Selection");
                return null;
            }

        }

        private void List()
        {
            List<Blog> blogs = _blogRepository.GetAll(); 

            foreach (Blog blog in blogs)
            {
                Console.WriteLine(blog);
            }
        }

        private void Add()
        {
            Console.WriteLine("New Blog");
            Blog blog = new();

            Console.WriteLine("Title: ");
            blog.Title = Console.ReadLine() ?? "";

            Console.WriteLine("URL: ");
            blog.Url = Console.ReadLine() ?? "";

            _blogRepository.Insert(blog); 
        }

        private void Edit()
        {
            Blog blogToEdit = Choose("Which blog would you like to edit?");

            if (blogToEdit == null) return;

            Console.WriteLine();
            Console.Write("New title (keep blank to leave unchanged:");

            string newTitle = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(newTitle))
            {
                blogToEdit.Title = newTitle;
            }

            Console.Write("New URL (keep blank to leave unchaged: ");

            string newUrl = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(newUrl))
            {
                blogToEdit.Url = newUrl; 
            }

            _blogRepository.Update(blogToEdit); 

        }

        private void Remove()
        {
            Blog blogToDelete = Choose("Which blog would you like to remove?");

            if (blogToDelete != null) _blogRepository.Delete(blogToDelete.Id); 
        }


    }
}

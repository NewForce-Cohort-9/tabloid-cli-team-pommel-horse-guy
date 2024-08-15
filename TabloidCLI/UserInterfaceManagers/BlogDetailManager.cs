using TabloidCLI.Models;
using TabloidCLI.Repositories; 

namespace TabloidCLI.UserInterfaceManagers
{
    internal class BlogDetailManager: IUserInterfaceManager
    {

        private IUserInterfaceManager _parentUI;
        private BlogRepository _blogRepository;
        private PostRepository _postRepository;
        private TagRepository _tagRepository;
        private int _blogId;

        public BlogDetailManager(IUserInterfaceManager parentUI, string connectionString, int blogId)
        {
            _parentUI = parentUI;
            _blogRepository = new BlogRepository(connectionString);
            _postRepository = new PostRepository(connectionString);
            _tagRepository = new TagRepository(connectionString);
            _blogId = blogId;
        }

        public IUserInterfaceManager Execute()
        {
            Blog blog = _blogRepository.Get(_blogId);

            Console.WriteLine($"{blog.Title} Details");

            Console.WriteLine(" 1) View");
            Console.WriteLine(" 2) Add Tag");
            Console.WriteLine(" 3) Remove Tag");
            Console.WriteLine(" 4) View Posts");
            Console.WriteLine(" 0) Go Back");

            Console.Write("> ");
            string choice = Console.ReadLine() ?? ""; 

            switch (choice)
            {
                case "1":
                    View();
                    return this;
                case "2":
                    AddTag();
                    return this;
                case "3":
                    RemoveTag();
                    return this;
                case "4":
                    ViewPosts();
                    return this;
                case "0":
                    return _parentUI;
                default:
                    Console.WriteLine("Invalid Selection");
                    return this; 
            }

        }

        private void View()
        {
            Blog blog = _blogRepository.GetWithBlogTags(_blogId);
            Console.WriteLine($"Title: {blog.Title}");
            Console.WriteLine($"URL: {blog.Url}");

            if (blog.Tags.Count > 0)
            {
                foreach (Tag tag in blog.Tags)
                {
                    Console.WriteLine("Tag is: " + tag);
                }
                Console.WriteLine();
            }
            else Console.WriteLine("This blog has no tags yet."); 

            
        }

        private void AddTag()
        {
            Blog blog = _blogRepository.Get(_blogId);

            Console.WriteLine($"Which tag would you like to add to {blog.Title}?");

            List<Tag> tags = _tagRepository.GetAll(); 

            for (int i = 0; i < tags.Count; i++)
            {
                Tag tag = tags[i];
                Console.WriteLine($" {i + 1}) {tag.Name}");
            }
            Console.Write("> ");

            string response = Console.ReadLine() ?? "";

            try {
                int choice = int.Parse(response);
                Tag tag = tags[choice - 1]; 
                _blogRepository.InsertTag(blog, tag);  
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid Selection. Won't add any tags.");
            }

        }

        private void RemoveTag()
        {
            Blog blog = _blogRepository.GetWithBlogTags(_blogId);

            Console.WriteLine($"Which tag would you like to remove from {blog.Title}?");

            List<Tag> tags = blog.Tags; 

            for (int i = 0; i < tags.Count; i++)
            {
                Tag tag = tags[i];
                Console.WriteLine($" {i + 1} {tag.Name}");

            }
            Console.Write("> ");

            string response = Console.ReadLine() ?? ""; 
            try {
                int choice = int.Parse(response);
                Tag tag = tags[choice - 1];
                _blogRepository.DeleteTag(blog.Id, tag.Id); 
            }

            catch (Exception ex)
            {
                Console.WriteLine("Invalid Selection. Won't remove any tags.");
            }
        }

        private void ViewPosts()
        {
            Blog blog = _blogRepository.Get(_blogId);

            List<Post> posts = _postRepository
                .GetByBlog(_blogId)
                .OrderByDescending(p => p.PublishDateTime)
                .ToList(); 

            if (posts.Count == 0)
            {
                Console.WriteLine($"No posts found for {blog.Title}");
                    return; 
            };

            Console.WriteLine();
            Console.WriteLine($"Showing all posts for {blog.Title}: ");
            
            for (int i = 0; i < posts.Count; i++)
            {
                Console.WriteLine($"{i + 1}) ");
                Console.WriteLine($"Title: {posts[i].Title}");
                Console.WriteLine($"Published on: {posts[i].PublishDateTime}");
                Console.WriteLine();
            }

        }
    }
}

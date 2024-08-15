using System;
using System.Collections.Generic;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    internal class PostManager : IUserInterfaceManager
    {
        private readonly IUserInterfaceManager _parentUI;
        private PostRepository _postRepository;
        private AuthorRepository _authorRepository;
        private BlogRepository _blogRepository;
        private string _connectionString;

        public PostManager(IUserInterfaceManager parentUI, string connectionString)
        {
            _parentUI = parentUI;
            _connectionString = connectionString;
            _postRepository = new PostRepository(connectionString);
            _authorRepository = new AuthorRepository(connectionString);
            _blogRepository = new BlogRepository(connectionString);
        }

        public IUserInterfaceManager Execute()
        {
            Console.WriteLine("Post Management");
            Console.WriteLine(" 1) List Posts");
            Console.WriteLine(" 2) Add Post"); 
            Console.WriteLine(" 3) Remove Post");
            Console.WriteLine(" 4) Edit Post");
            Console.WriteLine(" 5) View Post Details");
            Console.WriteLine(" 0) Go Back");

            Console.Write("> ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ListPosts();
                    return this;
                case "2":
                    AddPost();
                    return this;
                case "3":
                    RemovePost();
                    return this;
                case "4":
                    EditPost();
                    return this;
                case "5":
                    return ViewPostDetails();
                case "0":
                    return _parentUI;
                default:
                    Console.WriteLine("Invalid Selection");
                    return this;
            }
        }

        private void ListPosts()
        {
            List<Post> posts = _postRepository.GetAll();
            foreach (Post post in posts)
            {
                Console.WriteLine($"{post.Title} ({post.Url})");
            }
        }

        private void AddPost()
        {
            Console.WriteLine("New Post");
            Post post = new Post();

            Console.Write("Title: ");
            post.Title = Console.ReadLine();

            Console.Write("URL: ");
            post.Url = Console.ReadLine();

            Console.Write("Publication Date (yyyy-mm-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime publishDate))
            {
                post.PublishDateTime = publishDate;
            }
            else
            {
                Console.WriteLine("Invalid date format. Post not saved.");
                return;
            }

            // Select Author
            post.Author = ChooseAuthor();
            if (post.Author == null)
            {
                Console.WriteLine("Invalid author selection. Post not saved.");
                return;
            }

            // Select Blog
            post.Blog = ChooseBlog();
            if (post.Blog == null)
            {
                Console.WriteLine("Invalid blog selection. Post not saved.");
                return;
            }

            _postRepository.Insert(post);
            Console.WriteLine("Post added successfully.");
        }

        private void RemovePost()
        {
            Post postToRemove = ChoosePost("Which post would you like to remove?");
            if (postToRemove != null)
            {
                _postRepository.Delete(postToRemove.Id);
                Console.WriteLine("Post removed successfully.");
            }
        }

        private void EditPost()
        {
            Post postToEdit = ChoosePost("Which post would you like to edit?");
            if (postToEdit == null)
            {
                return;
            }

            Console.Write("New title (blank to leave unchanged): ");
            string title = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(title))
            {
                postToEdit.Title = title;
            }

            Console.Write("New URL (blank to leave unchanged): ");
            string url = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(url))
            {
                postToEdit.Url = url;
            }

            Console.Write("New publication date (yyyy-mm-dd, blank to leave unchanged): ");
            string publishDateInput = Console.ReadLine();
            if (DateTime.TryParse(publishDateInput, out DateTime publishDate))
            {
                postToEdit.PublishDateTime = publishDate;
            }

            // Edit Author
            Author newAuthor = ChooseAuthor("Select a new author (blank to leave unchanged): ");
            if (newAuthor != null)
            {
                postToEdit.Author = newAuthor;
            }

            // Edit Blog
            Blog newBlog = ChooseBlog("Select a new blog (blank to leave unchanged): ");
            if (newBlog != null)
            {
                postToEdit.Blog = newBlog;
            }

            _postRepository.Update(postToEdit);
            Console.WriteLine("Post updated successfully.");
        }

        private IUserInterfaceManager ViewPostDetails()
        {
            Post post = ChoosePost("Which post would you like to view?");
            if (post != null)
            {
                return new PostDetailManager(this, _connectionString, post);
            }
            return this;
        }

        private Post ChoosePost(string prompt)
        {
            Console.WriteLine(prompt);

            List<Post> posts = _postRepository.GetAll();

            for (int i = 0; i < posts.Count; i++)
            {
                Post post = posts[i];
                Console.WriteLine($" {i + 1}) {post.Title}");
            }
            Console.Write("> ");

            string input = Console.ReadLine();
            try
            {
                int choice = int.Parse(input);
                return posts[choice - 1];
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Selection");
                return null;
            }
        }

        private Author ChooseAuthor(string prompt = null)
        {
            if (prompt != null)
            {
                Console.WriteLine(prompt);
            }

            List<Author> authors = _authorRepository.GetAll();

            for (int i = 0; i < authors.Count; i++)
            {
                Author author = authors[i];
                Console.WriteLine($" {i + 1}) {author.FullName}");
            }
            Console.Write("> ");

            string input = Console.ReadLine();
            try
            {
                int choice = int.Parse(input);
                return authors[choice - 1];
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Selection");
                return null;
            }
        }

        private Blog ChooseBlog(string prompt = null)
        {
            if (prompt != null)
            {
                Console.WriteLine(prompt);
            }

            List<Blog> blogs = _blogRepository.GetAll();

            for (int i = 0; i < blogs.Count; i++)
            {
                Blog blog = blogs[i];
                Console.WriteLine($" {i + 1}) {blog.Title}");
            }
            Console.Write("> ");

            string input = Console.ReadLine();
            try
            {
                int choice = int.Parse(input);
                return blogs[choice - 1];
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Selection");
                return null;
            }
        }
    }
}


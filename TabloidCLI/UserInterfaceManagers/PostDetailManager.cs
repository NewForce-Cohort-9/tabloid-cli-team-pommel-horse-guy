using System;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    internal class PostDetailManager : IUserInterfaceManager
    {
        private readonly IUserInterfaceManager _parentUI;
        private readonly Post _post;
        private readonly TagRepository _tagRepository;
        private readonly string _connectionString;

        public PostDetailManager(IUserInterfaceManager parentUI, string connectionString, Post post)
        {
            _parentUI = parentUI;
            _connectionString = connectionString;
            _post = post;
            _tagRepository = new TagRepository(connectionString);
        }

        public IUserInterfaceManager Execute()
        {
            Console.WriteLine($"Post Details: {_post.Title}");
            Console.WriteLine(" 1) View Details");
            Console.WriteLine(" 2) Add Tag");
            Console.WriteLine(" 3) Remove Tag");
            Console.WriteLine(" 4) Note Management");
            Console.WriteLine(" 0) Go Back");

            Console.Write("> ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ViewDetails();
                    return this;
                case "2":
                    AddTag();
                    return this;
                case "3":
                    RemoveTag();
                    return this;
                case "4":
                    return new NoteManager(this, _connectionString);
                case "0":
                    return _parentUI;
                default:
                    Console.WriteLine("Invalid Selection");
                    return this;
            }
        }

        private void ViewDetails()
        {
            Console.WriteLine($"Title: {_post.Title}");
            Console.WriteLine($"URL: {_post.Url}");
            Console.WriteLine($"Publication Date: {_post.PublishDateTime.ToShortDateString()}");
            Console.WriteLine($"Author: {_post.Author.FullName}");
            Console.WriteLine($"Blog: {_post.Blog.Title}");
        }

        private void AddTag()
        {
            Console.WriteLine("Tagging is not yet implemented.");
        }

        private void RemoveTag()
        {
            Console.WriteLine("Removing a tag is not yet implemented.");
        }
    }
}


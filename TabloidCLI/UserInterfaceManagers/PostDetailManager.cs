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
        private readonly PostTagRepository _postTagRepository;
        private readonly string _connectionString;

        public PostDetailManager(IUserInterfaceManager parentUI, string connectionString, Post post)
        {
            _parentUI = parentUI;
            _connectionString = connectionString;
            _post = post;
            _tagRepository = new TagRepository(connectionString);
            _postTagRepository = new PostTagRepository(connectionString);
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

            List<Tag> tags = _postTagRepository.GetTagsByPostId( _post.Id );
            if ( tags.Count > 0 )
            {
                Console.WriteLine("Tags:");
                foreach ( Tag tag in tags )
                {
                    Console.WriteLine($" - {tag.Name}");
                }
            }
            else
            {
                Console.WriteLine("No tags assigned to this post");
            }
        }

        private void AddTag()
        {
            Console.WriteLine("Select a tag to add:");
            List<Tag> tags = _tagRepository.GetAll();
            for (int i = 0; i < tags.Count; i++)
            {
                Tag tag = tags[i];
                Console.WriteLine($" {i + 1}) {tag.Name}");
            }
            Console.WriteLine("> ");
            string input = Console.ReadLine();

            try
            {
                int choice = int.Parse(input);
                Tag selectedTag = tags[choice - 1];

                _postTagRepository.Insert(_post.Id, selectedTag.Id);
                Console.WriteLine($"Tag '{selectedTag.Name}' added to the post.");
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid selection. No tag added.");
            }
        }

        private void RemoveTag()
        {
            Console.WriteLine("Select a tag to remove:");

            // Display tags associated with the post
            List<Tag> tags = _postTagRepository.GetTagsByPostId(_post.Id);
            if (tags.Count == 0)
            {
                Console.WriteLine("No tags to remove.");
                return;
            }

            for (int i = 0; i < tags.Count; i++)
            {
                Tag tag = tags[i];
                Console.WriteLine($" {i + 1}) {tag.Name}");
            }
            Console.Write("> ");
            string input = Console.ReadLine();

            try
            {
                int choice = int.Parse(input);
                Tag selectedTag = tags[choice - 1];

                // Remove the tag from the post
                _postTagRepository.Delete(_post.Id, selectedTag.Id);
                Console.WriteLine($"Tag '{selectedTag.Name}' removed from the post.");
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid selection. No tag removed.");
            }
        }
    }
}


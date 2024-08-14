using System;
using System.Collections.Generic;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    internal class NoteDetailManager : IUserInterfaceManager
    {
        private IUserInterfaceManager _parentUI;
        private NoteRepository _noteRepository;
        private int _noteId;

        public NoteDetailManager(IUserInterfaceManager parentUI, string connectionString, int noteId)
        {
            _parentUI = parentUI;
            _noteRepository = new NoteRepository(connectionString);
            _noteId = noteId;
        }

        public IUserInterfaceManager Execute()
        {
            Note note = _noteRepository.Get(_noteId);
            Console.WriteLine($"{note.Title} Details");
            Console.WriteLine(" 1) View");
            Console.WriteLine(" 0) Go Back");

            Console.Write("> ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    View();
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
            Note note = _noteRepository.Get(_noteId);
            Console.WriteLine($"Title: {note.Title}");
            Console.WriteLine($"Content: {note.Content}");
            Console.WriteLine($"Date Posted: {note.CreateDateTime}");
            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Post: {note.Post.Title}");
            Console.WriteLine($"Url: {note.Post.Url}");
        }
    }
}
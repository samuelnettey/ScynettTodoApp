﻿using ScynettTodo.Core.Events;
using ScynettTodo.SharedKernel;
using ScynettTodo.SharedKernel.Interfaces;

namespace ScynettTodo.Core.Entities
{
    public class ToDoItem : BaseEntity, IAggregateRoot
    {
        public int UserId  => 1;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; }
        public bool IsDone { get; private set; }

        public void MarkComplete()
        {
            IsDone = true;

            Events.Add(new ToDoItemCompletedEvent(this));
        }

        public override string ToString()
        {
            string status = IsDone ? "Done!" : "Not done.";
            return $"{Id}: Status: {status} - {Title} - {Description}";
        }
    }
}

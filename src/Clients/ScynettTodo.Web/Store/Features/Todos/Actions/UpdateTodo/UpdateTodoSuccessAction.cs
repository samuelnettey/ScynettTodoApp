﻿using ScynettTodo.Web.Models;
using ScynettTodo.Web.Models.Dtos;

namespace ScynettTodo.Web.Store.Features.Todos.Actions.UpdateTodo
{
    public class UpdateTodoSuccessAction
    {
        public UpdateTodoSuccessAction(TodoDto todo) => 
            Todo = todo;

        public TodoDto Todo { get; }
    }
}
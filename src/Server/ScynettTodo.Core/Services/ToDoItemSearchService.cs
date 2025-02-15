﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.Result;
using ScynettTodo.Core.Entities;
using ScynettTodo.Core.Interfaces;
using ScynettTodo.Core.Specifications;
using ScynettTodo.SharedKernel.Interfaces;

namespace ScynettTodo.Core.Services
{
    public class ToDoItemSearchService : IToDoItemSearchService
    {
        private readonly IRepository _repository;

        public ToDoItemSearchService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<ToDoItem>>> GetAllIncompleteItemsAsync(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                var errors = new List<ValidationError>();
                errors.Add(new ValidationError()
                {
                    Identifier = nameof(searchString),
                    ErrorMessage = $"{nameof(searchString)} is required."
                });
                return Result<List<ToDoItem>>.Invalid(errors);
            }

            var incompleteSpec = new IncompleteItemsSpecification();

            try
            {
                var items = await _repository.ListAsync(incompleteSpec);

                return new Result<List<ToDoItem>>(items);
            }
            catch (Exception ex)
            {
                // TODO: Log details here
                return Result<List<ToDoItem>>.Error(new[] { ex.Message });
            }
        }

        public async Task<Result<ToDoItem>> GetNextIncompleteItemAsync()
        {
            var incompleteSpec = new IncompleteItemsSpecification();

            var items = await _repository.ListAsync(incompleteSpec);

            if (!items.Any())
            {
                return Result<ToDoItem>.NotFound();
            }

            return new Result<ToDoItem>(items.First());
        }
    }
}

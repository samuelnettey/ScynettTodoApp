﻿using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using ScynettTodo.Core.Entities;
using ScynettTodo.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ScynettTodo.Api.Endpoints.ToDoItems
{
    public class Create : BaseAsyncEndpoint
        .WithRequest<NewToDoItemRequest>
        .WithResponse<ToDoItemResponse>
    {
        private readonly IRepository _repository;

        public Create(IRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("/ToDoItems")]
        [SwaggerOperation(
            Summary = "Creates a new ToDoItem",
            Description = "Creates a new ToDoItem",
            OperationId = "ToDoItem.Create",
            Tags = new[] { "ToDoItemEndpoints" })
        ]
        public override async Task<ActionResult<ToDoItemResponse>> HandleAsync(NewToDoItemRequest request, CancellationToken cancellationToken)
        {
            var item = new ToDoItem
            {
                Title = request.Title,
                Description = request.Description
            };

            var createdItem = await _repository.AddAsync(item);

            return Ok(createdItem);
        }
    }
}

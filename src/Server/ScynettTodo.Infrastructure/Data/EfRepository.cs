﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ScynettTodo.SharedKernel;
using ScynettTodo.SharedKernel.Interfaces;

namespace ScynettTodo.Infrastructure.Data
{
    public class EfRepository : IRepository
    {
        private readonly AppDbContext _dbContext;

        public EfRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public T GetById<T>(int id) where T : BaseEntity, IAggregateRoot
        {
            return _dbContext.Set<T>().SingleOrDefault(e => e.Id == id);
        }

        public Task<T> GetByIdAsync<T>(int id) where T : BaseEntity, IAggregateRoot
        {
            return _dbContext.Set<T>().SingleOrDefaultAsync(e => e.Id == id);
        }

        public Task<List<T>> ListAsync<T>() where T : BaseEntity, IAggregateRoot
        {
            return _dbContext.Set<T>().ToListAsync();
        }

        public Task<List<T>> ListAsync<T>(ISpecification<T> spec) where T : BaseEntity, IAggregateRoot
        {
            var specificationResult = ApplySpecification(spec);
            return specificationResult.ToListAsync();
        }

        public async Task<T> AddAsync<T>(T entity) where T : BaseEntity, IAggregateRoot
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public Task UpdateAsync<T>(T entity) where T : BaseEntity, IAggregateRoot
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            return _dbContext.SaveChangesAsync();
        }

        public Task DeleteAsync<T>(T entity) where T : BaseEntity, IAggregateRoot
        {
            _dbContext.Set<T>().Remove(entity);
            return _dbContext.SaveChangesAsync();
        }

        private IQueryable<T> ApplySpecification<T>(ISpecification<T> spec) where T : BaseEntity
        {
            var evaluator = new SpecificationEvaluator<T>();
            return evaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
        }
    }
}

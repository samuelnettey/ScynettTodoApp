﻿using System;
using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScynettTodo.Api;
using ScynettTodo.Infrastructure.Data;
using ScynettTodo.UnitTests;

namespace ScynettTodo.FunctionalTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = builder.Build();


            // Get service provider.
            var serviceProvider = host.Services;

            // Create a scope to obtain a reference to the database
            // context (AppDbContext).
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();

                var logger = scopedServices
                    .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                // Ensure the database is created.
                db.Database.EnsureCreated();

                try
                {
                    // Seed the database with test data.
                    SeedData.PopulateTestData(db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the " +
                                        $"database with test messages. Error: {ex.Message}");
                }
            }

            host.Start();
            return host;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseSolutionRelativeContentRoot("src/ScynettTodo.Api")
                .ConfigureServices(services =>
                {
                    // Remove the app's ApplicationDbContext registration.
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                            typeof(DbContextOptions<AppDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add ApplicationDbContext using an in-memory database for testing.
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });

                    //// Create a new service provider.
                    //var serviceProvider = new ServiceCollection()
                    //    .AddEntityFrameworkInMemoryDatabase()
                    //    .BuildServiceProvider();

                    //// Add a database context (AppDbContext) using an in-memory
                    //// database for testing.
                    //services.AddDbContext<AppDbContext>(options =>
                    //{
                    //    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    //    options.UseInternalServiceProvider(serviceProvider);
                    //});

                    services.AddScoped<IMediator, NoOpMediator>();
                });
        }
    }
}
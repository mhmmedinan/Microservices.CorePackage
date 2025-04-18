﻿using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Core.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Core.Abstractions.Messaging.Outbox;

namespace Core.Messaging.SqlServer.Outbox;

/// <summary>
/// Database context for managing outbox messages in SQL Server
/// </summary>
public class OutboxDataContext : EfDbContextBase
{
    /// <summary>
    /// The default database schema for outbox messages
    /// </summary>
    public const string DefaultSchema = "messaging";

    /// <summary>
    /// Gets the DbSet for outbox messages
    /// </summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <summary>
    /// Initializes a new instance of the OutboxDataContext
    /// </summary>
    /// <param name="options">The options to be used by the context</param>
    public OutboxDataContext(DbContextOptions<OutboxDataContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}

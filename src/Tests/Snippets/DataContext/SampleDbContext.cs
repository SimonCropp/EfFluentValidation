﻿using FluentValidation.EntityFramework;
using Microsoft.EntityFrameworkCore;

public class SampleDbContext :
    ValidatingDbContext
{
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;

    public SampleDbContext(DbContextOptions options) :
        base(
            options,
            DefaultValidatorFactory<SampleDbContext>.Factory)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>()
            .HasMany(c => c.Employees)
            .WithOne(e => e.Company)
            .IsRequired();
        modelBuilder.Entity<Employee>();
    }
}
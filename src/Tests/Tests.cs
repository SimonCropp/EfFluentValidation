﻿using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EfFluentValidation;
using Microsoft.EntityFrameworkCore;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class Tests
{
    [Fact]
    public async Task InValidInterface()
    {
        var options = DbContextOptions();

        await using var data = new SampleDbContext(options);
        data.Add(new Employee
        {
            Id = -1,
            Content = "aaa"
        });
        var exception = await Assert.ThrowsAsync<EntityValidationException>(
            () => data.SaveChangesAsync());
        await Verifier.Verify(exception);
    }

    #region InValid

    [Fact]
    public async Task InValid()
    {
        var options = DbContextOptions();

        await using var data = new SampleDbContext(options);
        data.Add(new Employee {Content = ""});
        var exception = await Assert.ThrowsAsync<EntityValidationException>(
            () => data.SaveChangesAsync());
        await Verifier.Verify(exception);
    }

    #endregion

    [Fact]
    public async Task Update()
    {
        var options = DbContextOptions();

        await using var data = new SampleDbContext(options);
        var entity = new Employee
        {
            Content = "Foo"
        };
        data.Add(entity);
        await data.SaveChangesAsync();
        entity.Content = "";
        var exception = await Assert.ThrowsAsync<EntityValidationException>(
            () => data.SaveChangesAsync());
        await Verifier.Verify(exception);
    }

    [Fact]
    public async Task Delete()
    {
        var options = DbContextOptions();

        await using var data = new SampleDbContext(options);
        var entity = new Employee
        {
            Content = "Foo"
        };
        data.Add(entity);
        await data.SaveChangesAsync();
        entity.Content = "";
        data.Remove(entity);
        await data.SaveChangesAsync();
    }

    [Fact]
    public async Task InValid_Multiple()
    {
        var options = DbContextOptions();

        await using var data = new SampleDbContext(options);
        data.Add(new Employee {Content = ""});
        data.Add(new Company {Content = ""});
        var exception = await Assert.ThrowsAsync<EntityValidationException>(
            () => data.SaveChangesAsync());
        await Verifier.Verify(exception);
    }

    [Fact]
    public Task FromAssemblyContaining()
    {
        #region FromAssemblyContaining
        var scanResults = ValidationFinder.FromAssemblyContaining<SampleDbContext>();
        #endregion
        return Verifier.Verify(scanResults);
    }

    [Fact]
    public Task ValidatorTypeCacheUsage()
    {
        #region ValidatorTypeCacheUsage
        var scanResults = ValidationFinder.FromAssemblyContaining<SampleDbContext>();
        var typeCache = new ValidatorTypeCache(scanResults);
        var validators = typeCache.GetValidators(typeof(Employee));
        #endregion

        return Verifier.Verify(validators.ToList().Select(x => x.GetType()));
    }

    [Fact]
    public async Task Valid()
    {
        var options = DbContextOptions();

        await using var data = new SampleDbContext(options);
        data.Add(new Employee {Content = "a"});
        await data.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateSingleProp()
    {
        var options = DbContextOptions();

        await using var data1 = new SampleDbContext(options);
        var employee = new Employee
        {
            Content = "a"
        };
        data1.Add(employee);
        await data1.SaveChangesAsync();

        await using var data2 = new SampleDbContext(options);
        var update = new Employee
        {
            Id = employee.Id
        };
        var entry = data2.Employees.Attach(update);
        update.Age = 10;
        entry.Property(x => x.Age).IsModified = true;
        await data2.SaveChangesAsync();
    }


    [Fact]
    public async Task UpdateValid()
    {
        var options = DbContextOptions();

        await using var data = new SampleDbContext(options);
        var employee = new Employee
        {
            Content = "a"
        };
        data.Add(employee);
        await data.SaveChangesAsync();
        employee.Content = "b";
        await data.SaveChangesAsync();
    }

    static DbContextOptions<SampleDbContext> DbContextOptions(
        [CallerMemberName] string databaseName = "")
    {
        return new DbContextOptionsBuilder<SampleDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
    }
}
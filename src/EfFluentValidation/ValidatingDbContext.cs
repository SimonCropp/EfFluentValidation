﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EfFluentValidation
{
    public abstract class ValidatingDbContext :
        DbContext
    {
        Func<Type, IEnumerable<IValidator>> validatorFactory;

        protected ValidatingDbContext(
            Func<Type, IEnumerable<IValidator>> validatorFactory)
        {
            Guard.AgainstNull(validatorFactory, nameof(validatorFactory));
            this.validatorFactory = validatorFactory;
        }

        protected ValidatingDbContext(
            DbContextOptions options,
            Func<Type, IEnumerable<IValidator>> validatorFactory) :
            base(options)
        {
            Guard.AgainstNull(validatorFactory, nameof(validatorFactory));
            this.validatorFactory = validatorFactory;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DbContextValidator.Validate(this, validatorFactory).GetAwaiter().GetResult();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            await DbContextValidator.Validate(this, validatorFactory);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
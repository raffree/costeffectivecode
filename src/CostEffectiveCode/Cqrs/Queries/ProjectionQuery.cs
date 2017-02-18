﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CostEffectiveCode.Common;
using CostEffectiveCode.Ddd;
using CostEffectiveCode.Ddd.Entities;
using CostEffectiveCode.Ddd.Specifications;
using CostEffectiveCode.Extensions;

namespace CostEffectiveCode.Cqrs.Queries
{
    public class ProjectionQuery<TSource, TDest>
        : IQuery<ILinqSpecification<TDest>, IEnumerable<TDest>>
        , IQuery<ILinqSpecification<TDest>, int>

        , IQuery<Expression<Func<TDest,bool>>, IEnumerable<TDest>>
        , IQuery<Expression<Func<TDest,bool>>, int>

        , IQuery<ExpressionSpecification<TDest>, IEnumerable<TDest>>
        , IQuery<ExpressionSpecification<TDest>, int>

        where TSource : class, IHasId
        where TDest : class
    {
        protected readonly ILinqProvider LinqProvider;
        protected readonly IProjector Projector;

        public ProjectionQuery(ILinqProvider linqProvider, IProjector projector)
        {
            if (linqProvider == null) throw new ArgumentNullException(nameof(linqProvider));
            if (projector == null) throw new ArgumentNullException(nameof(projector));

            LinqProvider = linqProvider;
            Projector = projector;
        }

        protected virtual IQueryable<TDest> Query(object spec)
        {
            return LinqProvider
                .Query<TSource>()
                .MaybeWhere(spec)
                .MaybeSort(spec)
                .Project<TDest>(Projector)
                .MaybeWhere(spec)
                .MaybeSort(spec);
        }

        IEnumerable<TDest> IQuery<ILinqSpecification<TDest>, IEnumerable<TDest>>.Ask(ILinqSpecification<TDest> spec)
            => Query(spec).ToArray();

        int IQuery<ILinqSpecification<TDest>,int>.Ask(ILinqSpecification<TDest> spec)
            => Query(spec).Count();


        IEnumerable<TDest> IQuery<Expression<Func<TDest,bool>>, IEnumerable<TDest>>.Ask(Expression<Func<TDest, bool>> spec)
            => Query(spec).ToArray();

        int IQuery<Expression<Func<TDest, bool>>, int>.Ask(Expression<Func<TDest, bool>> spec)
            => Query(spec).Count();


        IEnumerable<TDest> IQuery<ExpressionSpecification<TDest>, IEnumerable<TDest>>.Ask(ExpressionSpecification<TDest> spec)
            => Query(spec).ToArray();

        int IQuery<ExpressionSpecification<TDest>, int>.Ask(ExpressionSpecification<TDest> spec)
            => Query(spec).Count();

    }
}

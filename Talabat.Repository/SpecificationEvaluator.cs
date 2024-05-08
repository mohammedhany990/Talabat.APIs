using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    public static class SpecificationEvaluator<T> where T:BaseEntity
    {
        public static IQueryable<T> GetQuery (IQueryable<T> InputQuery, ISpecifications<T> Spec ) 
        {
            var Query = InputQuery;

            if(Spec.criteria is not null)
            {
                Query = Query.Where( Spec.criteria );
            }
            
            if (Spec.OrderBy is not null)
            {
                Query = Query.OrderBy(Spec.OrderBy);
            }

            if (Spec.OrderByDescending is not null)
            {
                Query = Query.OrderByDescending(Spec.OrderByDescending);
            }

            if(Spec.IsPagination)
            {
                Query = Query.Skip(Spec.Skip).Take(Spec.Take);
            }
            
            Query = Spec.Includes.Aggregate(Query, (CurrentQuery, IncludeExp) => CurrentQuery.Include(IncludeExp));

            return Query;
        }
    }
}

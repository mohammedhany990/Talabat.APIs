using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    public static class SpecificationsEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> InputQuery, ISpecifications<T> Spec)
        {
            var Query = InputQuery;

            if (Spec.Criteria is not null)
            {
                Query = Query.Where(Spec.Criteria);
            }


            if (Spec.OrderByAsc is not null)
            {
                Query = Query.OrderBy(Spec.OrderByAsc);
            }
            else if (Spec.OrderByDesc is not null)
            {
                Query = Query.OrderByDescending(Spec.OrderByDesc);
            }

            if (Spec.IsPaginationEnabled)
            {
                Query = Query.Skip(Spec.Skip).Take(Spec.Take);
            }

            Query = Spec.Includes
                        .Aggregate(Query, (CurrentQ, IncludeExp) => CurrentQ.Include(IncludeExp));


            return Query;
        }
    }
}

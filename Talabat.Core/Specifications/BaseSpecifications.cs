using System.Linq.Expressions;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{

    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderByDesc { get; set; }
        public Expression<Func<T, object>> OrderByAsc { get; set; }
        public bool IsPaginationEnabled { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }

        public BaseSpecifications()
        {
            //Criteria = null;
        }
        public BaseSpecifications(Expression<Func<T, bool>> CriteriaExp)
        {
            Criteria = CriteriaExp;
        }

        public void ApplyOrderByDesc(Expression<Func<T, object>> orderByDesc)
        {
            OrderByDesc = orderByDesc;
        }

        public void ApplyOrderByAsc(Expression<Func<T, object>> orderByAsc)
        {
            OrderByAsc = orderByAsc;
        }

        public void ApplyPagination(int skip, int take)
        {
            IsPaginationEnabled = true;
            Take = take;
            Skip = skip;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class BaseSpecification<T> : ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get ; set ; }
        public Expression<Func<T, object>> OrderByDescending { get ; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPagination { get ; set ; }

        public BaseSpecification()
        {
            
        }
        public BaseSpecification(Expression<Func<T, bool>> criteriaExpression)
        {
            criteria = criteriaExpression;
        }

        public void AddOrderByAsc(Expression<Func<T, object>> OrderByExpression )
        {
            OrderBy = OrderByExpression;
        }
        public void AddOrderByDesc(Expression<Func<T, object>> orderByDescending)
        {
            OrderByDescending = orderByDescending;
        }
        public void ApplyPagination(int skip, int take)
        {
            IsPagination = true;
           
            Take = take;
            Skip = skip;
        }
       
    }
}

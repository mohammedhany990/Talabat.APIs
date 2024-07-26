using System.Linq.Expressions;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public interface ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; } // where( P => P.id == id )
        public List<Expression<Func<T, object>>> Includes { get; set; } // Include()

        public Expression<Func<T, object>> OrderByDesc { get; set; }
        public Expression<Func<T, object>> OrderByAsc { get; set; }
        public bool IsPaginationEnabled { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }

    }
}

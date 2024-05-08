using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
using Talabat.Repository.Contexts;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _storeContext;

        public GenericRepository(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }



        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _storeContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _storeContext.Set<T>().FindAsync(id);
        }



        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> Spec)
        {
            return await ApplySpec(Spec).ToListAsync();
        }

        public async Task<T> GetByIdWithSpecAsync(ISpecifications<T> Spec)
        {
            return await ApplySpec(Spec).FirstOrDefaultAsync();

        }

        public IQueryable<T> ApplySpec(ISpecifications<T> Spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_storeContext.Set<T>(), Spec);
        }

        public async Task<int> GetCountSpecAsync(ISpecifications<T> Spec)
        {
            return await ApplySpec(Spec).CountAsync();
        }
    }
}

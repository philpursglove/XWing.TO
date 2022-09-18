using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XWingTO.Data
{
    public interface IRepository<T, TKey>
    {
        T GetById(TKey id);

        IQueryable<T> Query();

        void Add(T entity);

        void Update(T entity);

        void Delete(TKey id);
    }

}

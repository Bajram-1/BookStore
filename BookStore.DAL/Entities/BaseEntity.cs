using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Entities
{
    public abstract class BaseEntity { }

    public abstract class BaseEntityWithKey<TKey> : BaseEntity
    {
        public TKey Id { get; set; }
    }
    public abstract class BaseEntityWithKey : BaseEntityWithKey<int> { }
}

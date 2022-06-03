using Play.Catalog.Service.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Play.Catalog.Service.Repositories
{
  public interface IItemsRepository
  {
    Task CreateAsync(Item entity);
    Task<IReadOnlyCollection<Item>> GetAllAsync();
    Task<Item> GetAsync(Guid id);
    Task RemoveAsync(Guid id);
    Task UpdateAsync(Item entity);
  }
}
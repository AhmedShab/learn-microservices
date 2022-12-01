using System;
using Play.Common;

namespace Play.Inventory.Service.Entities
{
    public class CatalogItem : IEntity
    {
        public Guid Id { get; set; }

        public Guid Name { get; set; }

        public Guid Description { get; set; }
    }
}

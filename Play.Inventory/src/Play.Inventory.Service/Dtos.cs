using System;

namespace Play.Inventory.Service.Dto
{
    public record GrandItemsDto(Guid UserId, Guid CatalogItemId, int Quality);

    public record InventoryItemDto(Guid CatalogItemId, string Name, string Description, int Quantity, DateTimeOffset AcquiredDate);

    public record CatalogItemDto(Guid Id, string Name, string Description);
}
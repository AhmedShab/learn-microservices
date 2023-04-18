using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Dto;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers {
  [ApiController]
  [Route("items")]
  [Authorize]
  public class ItemsController : ControllerBase {
    private readonly IRepository<InventoryItem> inventoryItemsRepository;

    private readonly IRepository<CatalogItem> catalogItemRepository;

    public ItemsController(IRepository<InventoryItem> inventoryItemsRepository, IRepository<CatalogItem> catalogItemRepository) {
      this.inventoryItemsRepository = inventoryItemsRepository;
      this.catalogItemRepository = catalogItemRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId) {
      if (userId == Guid.Empty) {
        return BadRequest();
      }

      var inventoryItemEntities = await inventoryItemsRepository.GetAllAsync(item => item.UserId == userId);
      var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);
      var catalogItemEntities = await catalogItemRepository.GetAllAsync(item => itemIds.Contains(item.Id));

      var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem => {
        var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);

        return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
      });

      return Ok(inventoryItemDtos);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(GrandItemsDto grandItemsDto) {
      var inventoryItem = await inventoryItemsRepository.GetAsync(
        item => item.UserId == grandItemsDto.UserId && item.CatalogItemId == grandItemsDto.CatalogItemId
      );

      if (inventoryItem == null) {
        inventoryItem = new InventoryItem {
          CatalogItemId = grandItemsDto.CatalogItemId,
          UserId = grandItemsDto.UserId,
          Quantity = grandItemsDto.Quality,
          AcquiredDate = DateTimeOffset.UtcNow
        };

        await inventoryItemsRepository.CreateAsync(inventoryItem);
      } else {
        inventoryItem.Quantity += grandItemsDto.Quality;
        await inventoryItemsRepository.UpdateAsync(inventoryItem);
      }

      return Ok();
    }
  }
}
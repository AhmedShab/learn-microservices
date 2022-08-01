using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dto;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers {
  [ApiController]
  [Route("items")]
  public class ItemsController : ControllerBase {
    private readonly IRepository<InventoryItem> itemsRepository;
    private readonly CatalogClient catalogClient;

    public ItemsController(IRepository<InventoryItem> itemsRepository, CatalogClient catalogClient) {
      this.itemsRepository = itemsRepository;
      this.catalogClient = catalogClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId) {
      if (userId == Guid.Empty) {
        return BadRequest();
      }

      var catalogItems = await catalogClient.GetCatalogItemsAsync();
      var inventoryItemEntities = await itemsRepository.GetAllAsync(item => item.UserId == userId);

      var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem => {
        var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);

        return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
      });

      return Ok(inventoryItemDtos);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(GrandItemsDto grandItemsDto) {
      var inventoryItem = await itemsRepository.GetAsync(
        item => item.UserId == grandItemsDto.UserId && item.CatalogItemId == grandItemsDto.CatalogItemId
      );

      if (inventoryItem == null) {
        inventoryItem = new InventoryItem {
          CatalogItemId = grandItemsDto.CatalogItemId,
          UserId = grandItemsDto.UserId,
          Quantity = grandItemsDto.Quality,
          AcquiredDate = DateTimeOffset.UtcNow
        };

        await itemsRepository.CreateAsync(inventoryItem);
      } else {
        inventoryItem.Quantity += grandItemsDto.Quality;
        await itemsRepository.UpdateAsync(inventoryItem);
      }

      return Ok();
    }
  }
}
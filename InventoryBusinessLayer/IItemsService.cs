using InventoryModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryBusinessLayer
{
    public interface IItemsService
    {

        List<ItemDto> GetItems();
        List<ItemDto> GetItemsByDateRange(DateTime minDateValue, DateTime maxDateValue);
        List<GetItemsTotalValueDto> GetItemsTotalValues(bool isActive);
        List<GetItemsForListingDto> GetItemsForListingFromProcedure();
        string GetAllItemsPipeDelimitedString();
        List<FullItemDetailDto> GetItemsWithGenresAndCategories();
    }
}

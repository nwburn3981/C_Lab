using InventoryModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryDatabaseLayer
{
    public interface IItemsRepo
    {

        List<ItemDto> GetItems();
        List<ItemDto> GetItemsByDateRange(DateTime minDatValue, DateTime macDateValue);
        List<GetItemsForListingDto> GetItemsForListingFromProcedure();
        List<GetItemsTotalValueDto> GetItemsTotalValues(bool isActive);
        List<FullItemDetailDto> GetItemsWithGenresAndCategories();
    }
}

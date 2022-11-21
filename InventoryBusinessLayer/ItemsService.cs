﻿using AutoMapper;
using EFCore_DBLibrary;
using InventoryDatabaseLayer;
using InventoryModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryBusinessLayer
{
    public class ItemsService: IItemsService
    {
        private readonly IItemsRepo _dbRepo;

        public ItemsService(InventoryDbContext dbContext, IMapper mapper)
        {
            _dbRepo = new ItemsRepo(dbContext, mapper);
        }

        public string GetAllItemsPipeDelimitedString()
        {
            var items = GetItems();
            return string.Join('|', items);
        }

        public List<ItemDto> GetItems()
        {
            return _dbRepo.GetItems();
        }

        public List<ItemDto> GetItemsByDateRange(DateTime minDateValue, DateTime maxDateValue)
        {
            return _dbRepo.GetItemsByDateRange(minDateValue, maxDateValue);
        }

        public List<GetItemsForListingDto> GetItemsForListingFromProcedure()
        {
            return _dbRepo.GetItemsForListingFromProcedure();
        }

        public List<GetItemsTotalValueDto> GetItemsTotalValues(bool isActive)
        {
            return _dbRepo.GetItemsTotalValues(isActive);  
        }

        public List<FullItemDetailDto> GetItemsWithGenresAndCategories()
        {
            return _dbRepo.GetItemsWithGenresAndCategories();
        }
    }
}

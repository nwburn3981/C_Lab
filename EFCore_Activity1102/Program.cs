using AutoMapper;
using EFCore_DBLibrary;
using InventoryBusinessLayer;
using InventoryHelpers;
using InventoryModels.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFCore_Activity1102
{
    public class Program
    {
        private static IConfigurationRoot _configuration;
        private static DbContextOptionsBuilder<InventoryDbContext> _optionsBuilder;
        private static MapperConfiguration _mapperConfig;
        private static IMapper _mapper;
        private static IServiceProvider _serviceProvider;
        private static IItemsService _itemsService;
        private static ICategoriesService _categoriesService;
        private static List<CategoryDto> _categories;

        public static void Main(string[] args)
        {
            BuildOptions();
            BuildMapper();
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                _itemsService = new ItemsService(db, _mapper);
                _categoriesService = new CategoriesService(db, _mapper);
                ListInventory();
                GetItemsForListing();
                GetAllActiveItemsAsPipeDelimitedString();
                GetItemsTotalValues();
                GetFullItemDetails();
                GetItemsForListingLinq();
                ListCategoriesAndColors();

                Console.WriteLine("Would you like to create items?");
                var createItems = Console.ReadLine().StartsWith("y", StringComparison.
                OrdinalIgnoreCase);
                if (createItems)
                {
                    Console.WriteLine("Adding new Item(s)");
                    CreateMultipleItems();
                    Console.WriteLine("Items added");
                    var inventory = _itemsService.GetItems();
                    inventory.ForEach(x => Console.WriteLine($"Item: {x}"));
                }
            }
        }


        private static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
            _optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InventoryManager"));
        }

        private static void BuildMapper()
        {
            var services = new ServiceCollection();
            services.AddAutoMapper(typeof(InventoryMapper));
            _serviceProvider = services.BuildServiceProvider();

            _mapperConfig = new MapperConfiguration(cfg => {
                cfg.AddProfile<InventoryMapper>();
            });
            _mapperConfig.AssertConfigurationIsValid();
            _mapper = _mapperConfig.CreateMapper();
        }

        private static void ListInventory()
        {
            var result = _itemsService.GetItems();
            result.ForEach(x => Console.WriteLine($"New Item: {x}"));
        }

        private static void GetItemsForListing()
        {
            var results = _itemsService.GetItemsForListingFromProcedure();
            foreach (var item in results)
            {
                var output = $"ITEM {item.Name}] {item.Description}";
                if (!string.IsNullOrWhiteSpace(item.CategoryName))
                {
                    output = $"{output} has category: {item.CategoryName}";
                }
                Console.WriteLine(output);
            }
        }

        private static void GetItemsForListingLinq()
        {
            var minDateValue = new DateTime(2021, 1, 1);
            var maxDateValue = new DateTime(2024, 1, 1);

            var results = _itemsService.GetItemsByDateRange(minDateValue, maxDateValue)
                            .OrderBy(y => y.CategoryName).ThenBy(z => z.Name);
            foreach (var itemDto in results)
            {
                Console.WriteLine(itemDto);
            }
        }

        private static void GetAllActiveItemsAsPipeDelimitedString()
        {
            Console.WriteLine($"All active Items: {_itemsService.GetAllItemsPipeDelimitedString()}");
        }

        private static void GetItemsTotalValues()
        {
            var results = _itemsService.GetItemsTotalValues(true);
            foreach (var item in results)
            {
                Console.WriteLine($"New Item] {item.Id,-10}" +
                                    $"|{item.Name,-50}" +
                                    $"|{item.Quantity,-4}" +
                                    $"|{item.TotalValue,-5}");
            }
        }

        private static void GetFullItemDetails()
        {
            var result = _itemsService.GetItemsWithGenresAndCategories();

            foreach (var item in result)
            {
                Console.WriteLine($"New Item] {item.Id,-10}" +
                                    $"|{item.ItemName,-50}" +
                                    $"|{item.ItemDescription,-4}" +
                                    $"|{item.PlayerName,-5}" +
                                    $"|{item.Category,-5}" +
                                    $"|{item.GenreName,-5}");
            }
        }

        private static void ListCategoriesAndColors()
        {
            var results = _categoriesService.ListCategoriesAndDetails();
            foreach (var c in results)
            {
                Console.WriteLine($"Category [{c.Category}] is {c.CategoryDetail.Color}");
            }
            _categories = results;
        }

        private static void CreateMultipleItems()
        {
            Console.WriteLine("Would you like to create items as a batch?");
            bool batchCreate = Console.ReadLine().StartsWith("y", StringComparison.
            OrdinalIgnoreCase);
            var allItems = new List<CreateOrUpdateItemDto>();
            bool createAnother = true;
            while (createAnother == true)
            {
                var newItem = new CreateOrUpdateItemDto();
                Console.WriteLine("Creating a new item.");
                Console.WriteLine("Please enter the name");
                newItem.Name = Console.ReadLine();
                Console.WriteLine("Please enter the description");
                newItem.Description = Console.ReadLine();
                Console.WriteLine("Please enter the notes");
                newItem.Notes = Console.ReadLine();
                Console.WriteLine("Please enter the Category [B]ooks, [M]ovies, [G]ames");
                newItem.CategoryId = GetCategoryId(Console.ReadLine().Substring(0,
                1).ToUpper());

                if (!batchCreate)
                {
                    _itemsService.UpsertItem(newItem);
                }
                else
                {
                    allItems.Add(newItem);
                }
                Console.WriteLine("Would you like to create another item?");
                createAnother = Console.ReadLine().StartsWith("y",
                StringComparison.OrdinalIgnoreCase);

            if (batchCreate && !createAnother)
                {
                    _itemsService.UpsertItems(allItems);
                }
            }
        }

        private static int GetCategoryId(string input)
        {


            switch (input)
            {
                case "B":
                    return _categories.FirstOrDefault(x => x.Category.ToLower().
                    Equals("books"))?.Id ?? -1;
                case "M":
                    return _categories.FirstOrDefault(x => x.Category.ToLower().
                    Equals("movies"))?.Id ?? -1;
                case "G":
                    return _categories.FirstOrDefault(x => x.Category.ToLower().
                    Equals("games"))?.Id ?? -1;
            default:
                    return -1;
            }
        }
    }
}

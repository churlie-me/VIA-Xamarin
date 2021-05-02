using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Via.Models;

namespace Via.Services
{
    public class MockDataStore : IDataStore<MenuItem>
    {
        List<MenuItem> items;

        public MockDataStore()
        {
            items = new List<MenuItem>();
            var mockItems = new List<MenuItem>
            {
                new MenuItem { Id = Guid.NewGuid().ToString(), Text = "First item", Description="This is an item description." },
                new MenuItem { Id = Guid.NewGuid().ToString(), Text = "Second item", Description="This is an item description." },
                new MenuItem { Id = Guid.NewGuid().ToString(), Text = "Third item", Description="This is an item description." },
                new MenuItem { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description="This is an item description." },
                new MenuItem { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description="This is an item description." },
                new MenuItem { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description="This is an item description." },
            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
        }

        public async Task<bool> AddItemAsync(MenuItem item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(MenuItem item)
        {
            var oldItem = items.Where((MenuItem arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((MenuItem arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<MenuItem> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<MenuItem>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}
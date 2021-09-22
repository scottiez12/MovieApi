using MovieApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Services
{
    public class InMemoryRepository : IRepository
    {
        private List<Genre> _genres;

        public InMemoryRepository()
        {
            _genres = new List<Genre>()
            {
                new Genre(){Id= 1, Name="Comedy"},
                new Genre(){Id= 2, Name="Action"}
            };
        }

        public async Task<List<Genre>> GetAllGenres()
        {
            await Task.Delay(1500);
            return _genres;
        }

        public Genre GetGenreById(int Id)
        {
            return _genres.FirstOrDefault(x => x.Id == Id);
        }
    }
}

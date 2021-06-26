using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Songify.Simple.Models;

namespace Songify.Simple.DAL
{
    public class ArtistsRepository : IRepository, IArtistsRepository
    {
        private readonly SongifyDbContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public ArtistsRepository(SongifyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Add(Artist model)
        {
            _context.Artists.Add(model);
        }

        public void Delete(int id)
        {
            _context.Artists.Remove(new Artist {Id = id});
        }

        public Task<Artist> GetArtist(int id)
        {
            return _context.Artists.FirstAsync(artist => artist.Id == id);
        }

        public void Update(Artist model)
        {
            _context.Entry(model).State = EntityState.Modified;
        }
    }
}
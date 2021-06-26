using System.Threading.Tasks;
using Songify.Simple.Models;

namespace Songify.Simple.DAL
{
    public interface IArtistsRepository
    {
        IUnitOfWork UnitOfWork { get; }
        void Add(Artist model);
        Task<Artist> GetArtist(int id);
        void Delete(int id);
        void Update(Artist model);
    }
}
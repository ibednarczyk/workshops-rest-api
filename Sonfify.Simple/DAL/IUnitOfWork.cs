using System;
using System.Threading;
using System.Threading.Tasks;

namespace Songify.Simple.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
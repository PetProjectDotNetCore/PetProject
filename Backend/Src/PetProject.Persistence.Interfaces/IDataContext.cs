using Microsoft.EntityFrameworkCore;
using PetProject.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace PetProject.Persistence.Interfaces
{
	public interface IDataContext
	{
		DbSet<RefreshToken> RefreshTokens { get; set; }
		DbSet<User> Users { get; set; }

		void Migrate();
		int SaveChanges();
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}

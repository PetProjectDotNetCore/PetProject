using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetProject.Domain.Entities;
using PetProject.Persistence.Interfaces;

namespace PetProject.Persistence
{
	public class DataContext : DbContext, IDataContext
	{
		public DataContext(DbContextOptions<DataContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			ConfigureUser(modelBuilder.Entity<User>());
			ConfigureRefreshToken(modelBuilder.Entity<RefreshToken>());
		}

		public DbSet<User> Users { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }

		public void Migrate()
		{
			Database.Migrate();
		}

		private void ConfigureUser(EntityTypeBuilder<User> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.ValueGeneratedOnAdd();

			builder.Property(x => x.LastName)
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(x => x.FirstName)
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(x => x.Email)
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(x => x.PasswordHash)
				.IsRequired();

			builder.Property(x => x.PasswordSalt)
				.IsRequired();

			builder.HasMany(c => c.RefreshTokens)
				.WithOne(e => e.User)
				.HasForeignKey(x => x.UserId);
		}

		private void ConfigureRefreshToken(EntityTypeBuilder<RefreshToken> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.ValueGeneratedOnAdd();

			builder.Property(x => x.Token)
				.IsRequired();

			builder.Property(x => x.Expires)
				.IsRequired();
		}
	}
}

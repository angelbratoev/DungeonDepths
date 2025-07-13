using DungeonDepths.Models;
using Microsoft.EntityFrameworkCore;

namespace DungeonDepths.Data
{
	public class DungeonDepthsContext : DbContext
	{
		public DungeonDepthsContext(DbContextOptions<DungeonDepthsContext> options)
			: base(options)
		{
			
		}
		public DbSet<Character> Characters { get; set; }
		public DbSet<Session> Sessions { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
		}
	}
}

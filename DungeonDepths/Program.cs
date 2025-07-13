using DungeonDepths.Contracts;
using DungeonDepths.Core;
using DungeonDepths.Data;
using DungeonDepths.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureAppConfiguration((context, config) =>
	{
		config.AddUserSecrets<Program>(); //Add user secrets for the connection string
	})
	.ConfigureServices((context, services) => //Add services to the container
			{
				var configuration = context.Configuration;

				string connectionString = configuration.GetConnectionString("DungeonDepthsDatabase");

				services.AddDbContext<DungeonDepthsContext>(options =>
					options.UseSqlServer(connectionString));

				services.AddScoped<ICharacterRepository, CharacterRepository>();
				services.AddScoped<ISessionRepository, SessionRepository>();

				services.AddScoped<ICharacterService, CharacterService>();
				services.AddScoped<ISessionService, SessionService>();

				services.AddScoped<Game>();
			})
			.Build();

//Setting the encoding to UTF8 in order do display ◙
Console.OutputEncoding = System.Text.Encoding.UTF8;

Game game = host.Services.GetRequiredService<Game>();

await game.PlayAsync();
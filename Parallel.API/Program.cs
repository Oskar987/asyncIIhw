using Microsoft.AspNetCore.Http.HttpResults;
using PersonService.Library;
using PersonService.Shared;

namespace Parallel.API;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddAuthorization();

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();

		app.UseAuthorization();

		app.MapGet("/withawait", async () =>
			{
				PersonReader reader = new ();
				try
				{
					List<int> ids = await reader.GetIdsAsync();
					List<Person> persons = new();

					foreach (int id in ids)
					{
						var person = await reader.GetPersonAsync(id);
						persons.Add(person);
					}

					return Results.Ok(persons);
				}
				catch (Exception ex)
				{
					return Results.Problem("Error");
				}
			})
			.WithName("withawait")
			.WithOpenApi();
		
		// OPTION 2: Task w/ Continuation (runs parallel)
		app.MapGet("/WithTask", () => Results.Ok())
			.WithName("WithTask")
			.WithOpenApi();
		
		// OPTION 3: Parallel.ForEachAsync (runs parallel)
		app.MapGet("/WithForEach", () => Results.Ok())
			.WithName("WithForEach")
			.WithOpenApi();

		app.Run();
	}
}
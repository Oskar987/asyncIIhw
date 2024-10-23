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
		//OPTION 1: Using async/await (not parallel)
		app.MapGet("/persons/withawait", async () =>
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
			.WithName("Get persons withawait")
			.WithOpenApi();
		
		// OPTION 2: Task w/ Continuation (runs parallel)
		app.MapGet("/persons/withtask", () => Results.Ok())
			.WithName("Get persons WithTask")
			.WithOpenApi();
		
		// OPTION 3: Parallel.ForEachAsync (runs parallel)
		app.MapGet("/persons/withforeach", () => Results.Ok())
			.WithName("Get persons WithForEach")
			.WithOpenApi();

		app.Run();
	}
}
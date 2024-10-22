using PersonService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPersonProvider, HardCodedPersonProvider>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapGet("/persons", async (IPersonProvider provider) =>
	{
		await Task.Delay(3000);
		return provider.GetPersons();
	})
	.WithName("GetPersons");

app.MapGet("/persons/{id}", async (IPersonProvider provider, int id) =>
	{
		await Task.Delay(1000);
		return provider.GetPersons().FirstOrDefault(p => p.Id == id);
	})
	.WithName("GetPersonById");

app.MapGet("/persons/ids", 
		(IPersonProvider provider) => provider.GetPersons().Select(p => p.Id).ToList())
	.WithName("GetAllPersonIds");

app.Run();
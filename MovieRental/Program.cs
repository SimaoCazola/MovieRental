using MovieRental.Data;
using MovieRental.Customer;
using MovieRental.Movie;
using MovieRental.PaymentProviders;
using MovieRental.Rental;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEntityFrameworkSqlite().AddDbContext<MovieRentalDbContext>();

// Register controller dependencies; missing IMovieFeatures caused startup DI errors.
builder.Services.AddScoped<IMovieFeatures, MovieFeatures>();
// Use scoped to match DbContext lifetime; singleton + scoped dependency caused startup failures.
builder.Services.AddScoped<IRentalFeatures, RentalFeatures>();
builder.Services.AddScoped<IPaymentProvider, MbWayProvider>();
builder.Services.AddScoped<IPaymentProvider, PayPalProvider>();

var app = builder.Build();

// Register global exception handling early (UseExceptionHandler or custom middleware)
// to log errors and return consistent ProblemDetails without leaking sensitive data.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var client = new MovieRentalDbContext())
{
	client.Database.EnsureCreated();
	// Seed a customer for local testing; remove or replace with real data later.
	if (!client.Customers.Any())
	{
		client.Customers.Add(new Customer { Name = "Seed Customer" });
		client.SaveChanges();
	}
}

app.Run();

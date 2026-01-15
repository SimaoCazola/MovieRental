using Microsoft.EntityFrameworkCore;
using MovieRental.Data;
using MovieRental.PaymentProviders;

namespace MovieRental.Rental
{
	public class RentalFeatures : IRentalFeatures
	{
		private readonly MovieRentalDbContext _movieRentalDb;
		private readonly IReadOnlyDictionary<string, IPaymentProvider> _paymentProviders;

		public RentalFeatures(
			MovieRentalDbContext movieRentalDb,
			IEnumerable<IPaymentProvider> paymentProviders)
		{
			_movieRentalDb = movieRentalDb;
			_paymentProviders = paymentProviders.ToDictionary(p => p.Method, StringComparer.OrdinalIgnoreCase);
		}

		// Async avoids blocking the request thread while the DB I/O completes.
		public async Task<Rental> SaveAsync(Rental rental)
		{
			if (string.IsNullOrWhiteSpace(rental.PaymentMethod))
			{
				throw new ArgumentException("Payment method is required.", nameof(rental.PaymentMethod));
			}

			if (!_paymentProviders.TryGetValue(rental.PaymentMethod, out var provider))
			{
				throw new InvalidOperationException(
					$"Payment provider not found for method '{rental.PaymentMethod}'.");
			}

			var paymentSucceeded = await provider.Pay(rental.DaysRented);
			if (!paymentSucceeded)
			{
				throw new InvalidOperationException(
					$"Payment failed using method '{rental.PaymentMethod}'.");
			}

			_movieRentalDb.Rentals.Add(rental);
			await _movieRentalDb.SaveChangesAsync();
			return rental;
		}

		//TODO: finish this method and create an endpoint for it
		public IEnumerable<Rental> GetRentalsByCustomerName(string customerName)
		{
			return _movieRentalDb.Rentals
				.Include(rental => rental.Customer)
				.Where(rental => rental.Customer != null && rental.Customer.Name == customerName)
				.ToList();
		}
	}
}

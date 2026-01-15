namespace MovieRental.PaymentProviders
{
	public interface IPaymentProvider
	{
		string Method { get; }
		Task<bool> Pay(double price);
	}
}

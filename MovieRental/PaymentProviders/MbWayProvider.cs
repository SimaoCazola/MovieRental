namespace MovieRental.PaymentProviders
{
	public class MbWayProvider : IPaymentProvider
	{
		public string Method => "MBWAY";

		public Task<bool> Pay(double price)
		{
			//ignore this implementation
			return Task.FromResult<bool>(true);
		}
	}
}

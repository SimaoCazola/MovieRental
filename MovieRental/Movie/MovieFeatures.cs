using MovieRental.Data;

namespace MovieRental.Movie
{
	public class MovieFeatures : IMovieFeatures
	{
		private readonly MovieRentalDbContext _movieRentalDb;
		public MovieFeatures(MovieRentalDbContext movieRentalDb)
		{
			_movieRentalDb = movieRentalDb;
		}
		
		public Movie Save(Movie movie)
		{
			_movieRentalDb.Movies.Add(movie);
			_movieRentalDb.SaveChanges();
			return movie;
		}

        // TODO: tell us what is wrong in this method? Forget about the async, what other concerns do you have?
        /*Loads all movies without pagination, which can be heavy with many records.
		Doesn’t use AsNoTracking(), so EF performs unnecessary tracking for read-only queries.
		Doesn’t filter or project fields — it returns everything.
		Suggestion: use AsNoTracking() and pagination (Skip/Take) to avoid loading the entire dataset.*/

        public List<Movie> GetAll()
		{
			return _movieRentalDb.Movies.ToList();
		}


	}
}

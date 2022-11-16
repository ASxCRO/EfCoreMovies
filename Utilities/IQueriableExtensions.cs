namespace EfCoreMovies.Utilities
{
    public static class IQueriableExtensions
    { 
        public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int page, int recordToTake)
        {
            return source.Skip((page - 1) * recordToTake).Take(recordToTake);
        }
    }
}

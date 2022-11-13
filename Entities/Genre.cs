using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfCoreMovies.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public HashSet<Movie> Movies { get; set; }
    }
}

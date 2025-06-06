using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trip.Models;

[Table("Country")]
public class Country
{
    [Key]
    public int IdCountry { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = null!;

    public ICollection<CountryTrip> CountryTrips { get; set; } = new List<CountryTrip>();
}
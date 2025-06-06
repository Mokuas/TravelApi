using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trip.Models;

[Table("Trip")]
public class Trip
{
    [Key]
    public int IdTrip { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(220)]
    public string Description { get; set; } = null!;

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public int MaxPeople { get; set; }

    public ICollection<ClientTrip> ClientTrips { get; set; } = new List<ClientTrip>();

    public ICollection<CountryTrip> CountryTrips { get; set; } = new List<CountryTrip>();
}
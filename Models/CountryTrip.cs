using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trip.Models;

[Table("Country_Trip")]
public class CountryTrip
{
    public int IdCountry { get; set; }

    public int IdTrip { get; set; }

    public Country Country { get; set; } = null!;
    public Trip Trip { get; set; } = null!;
}
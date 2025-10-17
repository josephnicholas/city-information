namespace CityInformation.API.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

public class City(string name) {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // New key is generated when a new city is added to the DB, generation depends on the provider
    public int Id { get; set; }
    
    [Required] // Cannot be empty
    [MaxLength(50)]
    public string Name { get; set; } = name;
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    public ICollection<PointOfInterest> PointsOfInterests { get; set; } 
        = new List<PointOfInterest>();
}

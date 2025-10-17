namespace CityInformation.API.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

public class PointOfInterest(string name) {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // New key is generated when a new city is added to the DB, generation depends on the provider
    public int Id { get; set; }
    
    [Required] // Cannot be empty
    [MaxLength(50)]
    public string Name { get; set; } = name;
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    [ForeignKey("CityId")] // Foreign key to navigate to the city is the City Id 
    public City? City { get; set; } // Navigation property and a relationship will be created
    public int CityId { get; set; } // Foreign Key should be named to be the navigation property's class
}

namespace CityInformation.API.Models;

using System.ComponentModel.DataAnnotations;

public class PointOfInterestUpdateDto {
    [Required(ErrorMessage = "Name with a maximum length of 50 characters is required.")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Description { get; set; }
}

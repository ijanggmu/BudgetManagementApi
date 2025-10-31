using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.WebApi.Address;
public class AddressResponseModel
{
    [Required(ErrorMessage = "Address Type is required.")]
    [StringLength(50, ErrorMessage = "Address Type cannot exceed 50 characters.")]
    public string AddressType { get; set; }

    [Required(ErrorMessage = "Province is required.")]
    [StringLength(50, ErrorMessage = "Province cannot exceed 50 characters.")]
    public string Province { get; set; }

    [Required(ErrorMessage = "District is required.")]
    [StringLength(50, ErrorMessage = "District cannot exceed 50 characters.")]
    public string District { get; set; }

    [Required(ErrorMessage = "Municipality is required.")]
    [StringLength(100, ErrorMessage = "Municipality cannot exceed 100 characters.")]
    public string Municipality { get; set; }

    [Required(ErrorMessage = "Ward is required.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Ward must be a numeric value.")]
    public int? Ward { get; set; }

    [Required(ErrorMessage = "Street Address is required.")]
    [StringLength(200, ErrorMessage = "Street Address cannot exceed 200 characters.")]
    public string StreetAddress { get; set; }
}

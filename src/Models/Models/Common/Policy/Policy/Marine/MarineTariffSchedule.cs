using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common.Policy.Policy.Marine;
public class MarineTariffScheduleViewModel
{
    public int SN { get; set; }
    public string Id { get; set; }
    [Required(ErrorMessage = "Product Category is required")]
    [StringLength(50, ErrorMessage = "Must be at most 50 characters long")]
    public string ProductCategory { get; set; }
    [Required(ErrorMessage = "Product Category Code is required")]
    [StringLength(50, ErrorMessage = "Must be at most 5 characters long")]
    public string ProductCategoryCode { get; set; }
    [Required(ErrorMessage = "Product is required")]
    [StringLength(50, ErrorMessage = "Must be at most 50 characters long")]
    public string Product { get; set; }
    [Required(ErrorMessage = "Product Code  is required")]
    [StringLength(50, ErrorMessage = "Must be at most 50 characters long")]
    public string ProductCode { get; set; }
    [Required(ErrorMessage = "All Risk Value  is required")]
    [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,6})?", ErrorMessage = ("Must be a number with 6 decimal points and maximum length of 25"))]
    public decimal AllRiskValue { get; set; }
    [Required(ErrorMessage = "Basic Risk Value  is required")]
    [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,6})?", ErrorMessage = ("Must be a number with 6 decimal points and maximum length of 25"))]
    public decimal BasicRiskValue { get; set; }
    [Required(ErrorMessage = "Minimum Risk Value  is required")]
    [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,6})?", ErrorMessage = ("Must be a number with 6 decimal points and maximum length of 25"))]
    public decimal MinimumRiskValue { get; set; }
    [Required(ErrorMessage = "Product Description  is required")]
    [StringLength(100, ErrorMessage = "Must be at most 50 characters long")]
    public string ProductDescription { get; set; }
    public string CreatedBy { get; set; }
    public string CreatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Aviation
{
    public class AircraftListViewModel
    {
        public string Identifier { get; set; }
        public string SN { get; set; }
        public string ItemNumber { get; set; }
        [Required(ErrorMessage = ("Please enter aircraft type"))]
        public string AircraftType { get; set; }
        public string YearOfManufacture { get; set; }
        public string SeatingCapacity { get; set; }
        [Required(ErrorMessage = ("Please enter a registration number"))]
        public string RegistrationNumber { get; set; }
        public decimal AgreedHullValue { get; set; }
    }

    public class EndorseAircraftListViewModel : AircraftListViewModel
    {
        public bool IsNew { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsExisting { get; set; }
    }
}

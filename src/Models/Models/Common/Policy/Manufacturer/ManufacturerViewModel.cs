using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Manufacturer
{
    public class ManufacturerViewModel
    {
        public string ManufacturerName { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public decimal CC { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal KiloWatt { get; set; }
        public decimal CarryingCapacityInTon { get; set; }
    }
}

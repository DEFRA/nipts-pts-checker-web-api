using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Models
{
    public class OwnerAddress
    {
        public string? AddressLineOne { get; set; }
        public string? AddressLineTwo { get; set; }
        public string? TownOrCity { get; set; }
        public string? County { get; set; }
        public string? PostCode { get; set; }
    }
}

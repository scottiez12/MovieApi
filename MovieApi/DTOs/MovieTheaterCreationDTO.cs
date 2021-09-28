﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.DTOs
{
    public class MovieTheaterCreationDTO
    {
        [Required]
        [StringLength(maximumLength: 120)]
        public string Name { get; set; }

        [Range(-90,90)]
        public double Latitude { get; set; }

        [Range(-180, 180)]
        public double Longitude { get; set; }

    }
}

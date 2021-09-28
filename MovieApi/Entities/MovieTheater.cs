using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Entities
{
    public class MovieTheater
    {
        public int Id { get; set; }

        [Required]
        [StringLength(74)]
        public string Name { get; set; }

        public Point Location { get; set; }
    }
}

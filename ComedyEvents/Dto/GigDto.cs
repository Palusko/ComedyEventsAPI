using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ComedyEvents.Dto
{
    public class GigDto
    {
        [Required]
        public int GigId { get; set; }

        [Required]
        [StringLength(20)]
        public string GigHeadline { get; set; }

        [Range(15,120)]
        public int GigLengthInMinutes { get; set; }

        public int EventId { get; set; }
        public EventDto Event { get; set; }

        public int ComedianId { get; set; }
        public ComedianDto Comedian { get; set; }
    }
}

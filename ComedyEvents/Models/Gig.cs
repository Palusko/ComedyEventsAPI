﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ComedyEvents.Models
{
    public class Gig
    {
        public int GigId { get; set; }
        public string GigHeadline { get; set; }
        public int GigLengthInMinutes { get; set; }
        public Event Event { get; set; }
        public Comedian Comedian { get; set; }
    }
}

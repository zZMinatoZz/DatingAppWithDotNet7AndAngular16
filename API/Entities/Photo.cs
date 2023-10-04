using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }

        // adding two props to define relation ship with AppUser, and make AppUserId is not null instead of nullable
        public int AppUserId { get; set; }
        public AppUser appUser { get; set; }
    }
}
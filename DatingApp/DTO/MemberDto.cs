using System;
using System.Collections.Generic;
using DatingApp.Entities;

namespace DatingApp.DTO
{
    public class MemberDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public int Age { get; set; }

        public string KnownAs { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActivated { get; set; } 

        public string Gender { get; set; }

        public string Introduction { get; set; }

        public string LookingFor { get; set; }

        public string Interests { get; set; }

        public string Country { get; set; }

        public string Street { get; set; }

        public ICollection<PhotoDto> Photos { get; set; }
    }
}
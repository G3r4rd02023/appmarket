using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Appmarket.Data.Entities
{
    public class Country
    {
        public int Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }

        [DisplayName("Cities Number")]
        public int CitiesNumber => Cities == null ? 0 : Cities.Count;

    }

}

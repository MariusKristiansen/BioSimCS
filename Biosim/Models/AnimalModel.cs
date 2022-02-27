using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Biosim.Models
{
    public abstract class AnimalModel
    {
        [Key]
        public int ID { get; set; }
        public virtual int? HerbivoreID { get; set; } = null;
        public virtual int? CarnivoreID { get; set; } = null;
        public int Age { get; set; }
        public double Weight { get; set; }
        public int NumberOfBirths { get; set; }
        public double Fitness { get; set; }
        //public List<int> Parents { get; set; }
    }

    public class HerbivoreModel : AnimalModel
    {
        public override int? HerbivoreID { get; set; } = null;
    }

    public class CarnivoreModel : AnimalModel
    {
        public override int? CarnivoreID { get; set; } = null;
    }
}

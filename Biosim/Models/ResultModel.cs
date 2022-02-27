using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biosim.Models
{
    public class ResultModel
    {
        [Key]
        public int ID { get; set; }
        public int Year { get; set; }
        public int Herbivores { get; set; }
        public int Carnivores { get; set; }
        public double HerbivoreAvgFitness { get; set; }
        public double CarnivoreAvgFitness { get; set; }
        public double HerbivoreAvgAge { get; set; }
        public double CarnivoreAvgAge { get; set; }
        public double HerbivoreAvgWeight { get; set; }
        public double CarnivoreAvgWeight { get; set; }
        public int HerbivoreBirths { get; set; }
        public int CarnivoreBirths { get; set; }
    }
}

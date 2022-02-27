using System.Collections.Generic;

namespace Biosim.Parameters
{
    public interface IAnimalParams
    {
        double BirthWeight { get; set; }
        double BirthSigma { get; set; }
        double Beta { get; set; }
        double Eta { get; set; }
        double AHalf { get; set; }
        double PhiAge { get; set; }
        double WHalf { get; set; }
        double PhiWeight { get; set; }
        double Mu { get; set; }
        double Gamma { get; set; }
        double Zeta { get; set; }
        double Xi { get; set; }
        double Omega { get; set; } // Passive chance of death (1 - omega) 
        double F { get; set; }
        double DeltaPhiMax { get; set; }

        Dictionary<string, double> CopyParameters();
        void OverloadParameters(double bWeight, double bSigma, double beta, double eta, double aHalf,
            double phiAge, double wHalf, double phiWeight, double mu, double gamma, double zeta,
            double xi, double omega, double f, double deltaPhiMax);
        void OverloadParameters(Dictionary<string, double> newParameters);
        void OverloadParameters(IAnimalParams parameters);
    }
}

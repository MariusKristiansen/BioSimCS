using System;
using System.Collections.Generic;
using System.Text;

namespace Biosim.Tools
{
    public interface ICommandData
    {
        Command Command { get; set; }
        string Parameters { get; set; }
        int ActivationYear { get; set; }
    }
}

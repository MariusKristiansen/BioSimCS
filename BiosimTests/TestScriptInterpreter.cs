using System;
using System.Collections.Generic;
using System.Text;
using Biosim.Tools;
using Xunit;

namespace BiosimTests
{
    public class TestScriptInterpreter
    {


        [Fact]
        public void CommandIsParsedTest()
        {
            /*Validate that the function returns a new CommandData object*/
            var Cparser = new ScriptInterpreter("../../../../BioSim/testscript.biosim");
            var commands = Cparser.Parse();
            Assert.True(commands.Count == 3);
        }


        [Fact]
        public void FileIsReadTest()
        {
            var Cparser = new ScriptInterpreter("../../../../BioSim/testscript.biosim");
            Assert.False(Cparser.Parse() is null);
        }


    }
}

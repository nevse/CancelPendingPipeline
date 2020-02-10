using System;
using System.Linq;
using CommandLine;

namespace CancelPendingPipeline {
    class Options {
        [Option('p', "project", Required = true, HelpText = "project name, i.e. XpfSchedulerPerformanceTestRunner for example")]
        public String ProjectName { get; set; }

        [Option('t', "token", Required = true, HelpText = "secret token to access")]
        public String Token { get; set; }
    }
}

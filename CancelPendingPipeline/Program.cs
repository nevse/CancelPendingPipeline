using CommandLine;
using GitLabApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CancelPendingPipeline {
    class Program {
        static void Main(string[] args) {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }
        static void RunOptions(Options options) {
            DoCancelWork(options).Wait();
        }
        static void HandleParseError(IEnumerable<Error> obj) {
            Console.WriteLine("Wrong params!");
        }

        async static Task DoCancelWork(Options options) {
            Console.WriteLine("Start cancelling...");
            var client = new GitLabClient("http://gitserver", options.Token);
            var projects = await client.Projects.GetAsync().ConfigureAwait(false);
            var project = projects.Where(x => string.Equals(x.Name, options.ProjectName, StringComparison.Ordinal)).First();
            var pipelines = await client.Pipelines.GetAsync(project.Id).ConfigureAwait(false);
            Console.WriteLine($"Found for {options.ProjectName} {pipelines.Count} pipelines");
            Console.WriteLine("Start removing pending pipelines...");
            var pendingPipelines = pipelines.Where(x => x.Status == GitLabApiClient.Models.Pipelines.PipelineStatus.Pending);
            Console.WriteLine($"Found {pendingPipelines.Count()} pending pipelines. Cancelling them...");
            foreach (var pipelineToCancel in pendingPipelines) {
                client.Pipelines.CancelAsync(project.Id, pipelineToCancel.Id).Wait();
                Console.WriteLine($"  canceled {pipelineToCancel.Id}");
            }
            Console.WriteLine("Done!");
        }
    }
}

using Aspire.Hosting;
using Projects;

var builder = DistributedApplication
    .CreateBuilder(args);

builder.AddProject<WorkerServiceLoggy>(nameof(WorkerServiceLoggy));
//builder.AddProject("WorkerServiceLoggy");


builder
    .Build()
    .Run();

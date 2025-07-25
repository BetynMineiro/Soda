var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.Soda_Api>("Soda-Api");
builder.Build().Run();
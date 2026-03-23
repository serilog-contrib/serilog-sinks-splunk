var builder = DistributedApplication.CreateBuilder(args);

var splunkPassword = builder.AddParameter("splunk-password", "changemeplease!", secret: true);

var splunk = builder.AddDockerfile("splunk", "../splunk")
    .WithVolume("splunk-var", "/opt/splunk/var")
    .WithBindMount("../splunk/default.yml", "/tmp/defaults/default.yml")
    .WithHttpEndpoint(port: 8000, targetPort: 8000, name: "web")
    .WithEndpoint(port: 8088, targetPort: 8088, name: "hec", scheme: "http")
    .WithEndpoint(port: 8089, targetPort: 8089, name: "api")
    .WithHttpHealthCheck("/", statusCode: 200, endpointName: "web")
    .WithEnvironment("SPLUNK_GENERAL_TERMS", "--accept-sgt-current-at-splunk-com")
    .WithEnvironment("SPLUNK_START_ARGS", "--accept-license --answer-yes --seed-passwd changeme")
    .WithEnvironment("SPLUNK_PASSWORD", splunkPassword);

builder.AddProject<Projects.Sample>("sample")
    .WaitFor(splunk)
    .WithEnvironment("SPLUNK_HEC_ENDPOINT", splunk.GetEndpoint("hec"))
    .WithEnvironment("SPLUNK_HEC_TOKEN", "00112233-4455-6677-8899-AABBCCDDEEFF");

await builder.Build().RunAsync();

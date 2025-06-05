// Program.cs
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register Semantic Kernel and Ollama Connector with Dependency Injection
builder.Services.AddSingleton<Kernel>(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    kernelBuilder.AddOllamaChatCompletion(
        modelId: builder.Configuration["Ollama:ModelId"] ?? "phi3",
        endpoint: new Uri(builder.Configuration["Ollama:Endpoint"] ?? "http://localhost:11434")
    );
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    return kernelBuilder.Build();
});

builder.Services.AddSingleton<IChatCompletionService>(sp =>
{
    var kernel = sp.GetRequiredService<Kernel>();
    return kernel.GetRequiredService<IChatCompletionService>();
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- NEW/MODIFIED STATIC FILE CONFIGURATION ---

// 1. Enable Default Files (e.g., index.html)
// This must be called *before* UseStaticFiles.
// It rewrites requests to a directory (like "/") to a default file (like "/index.html").
app.UseDefaultFiles();

// 2. Enable Static Files
// This middleware serves files from the 'wwwroot' folder.
app.UseStaticFiles();

// --- END NEW/MODIFIED STATIC FILE CONFIGURATION ---


app.UseAuthorization();

app.MapControllers();

app.Run();
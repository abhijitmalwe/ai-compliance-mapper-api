using App.Api.Configuration;
using App.Api.Middleware;
using App.Api.Validation;
using App.Application.Interfaces;
using App.Application.Security;
using App.Application.Service;
using App.Infrastructure.Git;
using App.Infrastructure.Integrations.GitHub;
using App.Infrastructure.Repo;
using App.Infrastructure.Vector;
using FluentValidation;
using FluentValidation.AspNetCore;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAppDependencies();
        builder.Services.AddConfiguredDbContexts(builder.Configuration);

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();

        builder.Services.AddScoped<ISecurityMappingService, SecurityMappingService>();
        builder.Services.AddScoped<Lib2SharpGitService>();

        builder.Services.AddScoped<IVectorService, QdrantService>();
        builder.Services.AddScoped<RepoService>();
        builder.Services.AddScoped<IGitHubIssueService>(sp =>
            new GitHubIssueService(builder.Configuration["GitHub:Token"]));

        builder.Services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<UsersValidator>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
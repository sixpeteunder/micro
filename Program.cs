﻿using System.Text.Json;
using CG.Web.MegaApiClient;
using Micro;

const string path = "./config.json";
var delay = TimeSpan.FromMilliseconds(1500);

Configuration configuration = new();

AnsiConsole.MarkupLine("[bold yellow]Hello there![/]");
await Task.Delay(delay: delay);

AnsiConsole.MarkupLine("[yellow]Let me just set up a few things![/]");
AnsiConsole.WriteLine();
await Task.Delay(delay: delay);

// Synchronous
await AnsiConsole.Status()
    .StartAsync("Initializing...", async ctx =>
    {
        Thread.Sleep(timeout: delay);
        AnsiConsole.MarkupLine("[blue][[INFO]]: Initialized.[/]");
        ctx.Status("Checking for config file...");


        Thread.Sleep(timeout: delay);
        var exists = File.Exists("./config.json");
        if (!exists)
        {
            AnsiConsole.MarkupLine("[yellow][[WARN]]: No config file found.[/]");
            ctx.Status("Cresting config...");

            Thread.Sleep(timeout: delay);
            File.Create("./config.json").Close();
            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(configuration));

            AnsiConsole.MarkupLine("[blue][[INFO]]: Config file created.[/]");
            ctx.Status("Loading default config...");
        }
        else
        {
            AnsiConsole.MarkupLine("[blue][[INFO]]: Config file found.[/]");
            ctx.Status("Reading config...");

            Thread.Sleep(timeout: delay);
            var text = await File.ReadAllTextAsync("./config.json");
            configuration = JsonSerializer.Deserialize<Configuration>(text) ?? configuration;
        }

        Thread.Sleep(timeout: delay);
        AnsiConsole.MarkupLine("[blue][[INFO]]: Configuration loaded successfully.[/]");
    });

AnsiConsole.WriteLine();
if (!configuration.Configured)
{
    AnsiConsole.MarkupLine("[yellow]This is your first time running this program![/]");
    AnsiConsole.MarkupLine("[yellow]Let's get you set up![/]");
    AnsiConsole.WriteLine();

    var name = AnsiConsole.Ask<string>("What should I call you?");
    var username =
        AnsiConsole.Ask<string>("[yellow]Now, enter the email address you use to sign in on mega.co.nz:[/]");
    var password = AnsiConsole.Prompt<string>(
        new TextPrompt<string>("[yellow]Now, enter the password you use to sign in on mega.co.nz:[/]").Secret()
    );
    
    AnsiConsole.WriteLine();

    configuration = configuration with {Configured = true, Name = name, Username = username, Password = password};
    await File.WriteAllTextAsync(path, JsonSerializer.Serialize(configuration));
    AnsiConsole.MarkupLine("[blue][[INFO]]: Configuration saved successfully.[/]");
}
else
{
    AnsiConsole.MarkupLine($"[yellow]Welcome back, [blue]{configuration.Name!}[/].[/]");
    AnsiConsole.WriteLine();
}

AnsiConsole.MarkupLine("[blue][[INFO]]: Starting up...[/]");
AnsiConsole.MarkupLine("[blue][[INFO]]: Logging you in...[/]");

var client = new MegaApiClient();
var token = client.Login(configuration.Username!, configuration.Password!);

AnsiConsole.MarkupLine($"[blue][[INFO]]: Login successful.[/]");

AnsiConsole.Clear();
AnsiConsole.MarkupLine("[yellow]Micro v1.0.0 \"Arc\"[/]");
AnsiConsole.MarkupLine("[yellow]Welcome to Micro![/]");
AnsiConsole.MarkupLine($"[yellow]Logged in as [blue]{configuration.Username!}[/].[/]");
AnsiConsole.WriteLine();
AnsiConsole.MarkupLine("[yellow]Type \"help\" for a list of commands.[/]");
AnsiConsole.MarkupLine("[yellow]Type \"exit\" to exit.[/]");
AnsiConsole.WriteLine();

// Prompt
while (AnsiConsole.Ask<string>("[yellow]micro > [/]") is not "exit")
{
    AnsiConsole.MarkupLine("[yellow]Processing...[/]");
    await Task.Delay(delay: delay);
    AnsiConsole.MarkupLine("[yellow]Done.[/]");
    AnsiConsole.WriteLine();
}

AnsiConsole.MarkupLine("[yellow]Goodbye![/]");
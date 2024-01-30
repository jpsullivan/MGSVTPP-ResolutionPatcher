// See https://aka.ms/new-console-template for more information

using MGSVTPP_ResolutionPatcher;
using Spectre.Console;

AnsiConsole.Markup("[underline red]Hello[/] World!");

var resolutions = new[]
{
    new ResolutionChoice("1024x763 (4:3)", [1024, 768]),
    new ResolutionChoice("1280x1024 (5:4)", [1280, 1024]),
    new ResolutionChoice("1366x768 (16:9)", [1366, 768]),
    new ResolutionChoice("1440x900 (16:10)", [1400, 900]),
    new ResolutionChoice("1680x1050 (16:10)", [1680, 1050]),
    new ResolutionChoice("1600x1200 (16:10)", [1600, 1200]),
    new ResolutionChoice("1920x1080 (16:9)", [1920, 1080]),
    new ResolutionChoice("2560x1080 (21:9)", [2560, 1080]),
    new ResolutionChoice("3440x1440 (21:9)", [3440, 1440]),
    new ResolutionChoice("5040x1050 (48:10)", [5040, 1050]),
    new ResolutionChoice("5120x1440 (32:9)", [5120, 1440]),
    new ResolutionChoice("4800x900 (48:9)", [4800, 900]),
    new ResolutionChoice("7860x1080 (48:9)", [7860, 1080]),
    new ResolutionChoice("7860x2160 (48:9)", [7860, 2160])
};

var selection = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Which resolution would you like to patch into MGSVTPP?")
        .PageSize(10)
        .AddChoices(resolutions.Select(r => r.Title)));

var resolution = resolutions.First(r => r.Title == selection).Value;

var _ = new Patcher(resolution[0], resolution[1]);

public record ResolutionChoice(string Title, int[] Value);
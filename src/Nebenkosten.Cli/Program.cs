using CommandLine;
using Nebenkosten.Cli.Options;
using Nebenkosten.Core.Domain;
using Serilog;

namespace Nebenkosten.Cli;

/// <summary>
/// Programm-Entry Point fuer die Nebenkostenabrechnung CLI.
/// Orchestriert das Laden, Validieren, Berechnen und Rendern.
/// </summary>
public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            // Serilog-Logger konfigurieren
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Console()
                .CreateLogger();

            // Command-Line Parser
            var parseResult = Parser.Default.ParseArguments<AbrechnungOptions>(args);

            return await parseResult.MapResult(
                async (AbrechnungOptions opts) => await RunAbrechnung(opts),
                errors => Task.FromResult(ExitCodes.ValidationError));
        }
        catch (NebenkosteException ex)
        {
            Console.Error.WriteLine(ex.Error.ToString());
            return ex.ExitCode;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FEHLER {ExitCodes.UnexpectedError}: Unerwarteter Fehler");
            Console.Error.WriteLine($"  {ex.Message}");
            Log.Fatal(ex, "Unexpect error in Nebenkostenabrechnung");
            return ExitCodes.UnexpectedError;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Fuehre die Abrechnung aus: Laden -> Validieren -> Rechnen -> Rendern.
    /// Phase A: Grundgeruest (nur Validierung und Artefakt-Grundvertrag).
    /// </summary>
    private static async Task<int> RunAbrechnung(AbrechnungOptions opts)
    {
        // Validiere CLI-Parameter
        opts.Validate();

        var billingDate = opts.GetBillingDate();

        // TODO (Phase B): Lade JSON und deserialisiere zu InputDTOs
        // TODO (Phase B): Validiere Schema
        // TODO (Phase B): Validiere Referenzen und Konsistenz
        // TODO (Phase C): Rechne Umlage, Verbrauch, Saldo
        // TODO (Phase C): Assembliere `einzelabrechnung.json`
        // TODO (Phase D): Rendere HTML
        // TODO: (Phase D): Wenn nicht --skip-pdf, rendere PDF

        // Phase A Placeholder: Schreibe erfolgreiche Test-Artefakt
        var outputPath = System.IO.Path.Combine(
            opts.OutputDirectory, 
            "test-mietpartei", 
            "einzelabrechnung.json");

        var outputDir = System.IO.Path.GetDirectoryName(outputPath);
        if (!System.IO.Directory.Exists(outputDir))
        {
            System.IO.Directory.CreateDirectory(outputDir!);
        }

        // Test-Output: einfaches JSON
        var testJson = @"{
  ""phase"": ""A - Artefaktvertrag & Validierung"",
  ""mietpartei_id"": ""test-mietpartei"",
  ""billing_date"": """ + billingDate.ToString("yyyy-MM-dd") + @""",
  ""status"": ""Phase A Initialisation erfolgreich""
}";

        await System.IO.File.WriteAllTextAsync(outputPath, testJson);
        Console.WriteLine($"✓ Abrechnung gestartet. Artefakte in: {opts.OutputDirectory}");

        return ExitCodes.Success;
    }
}

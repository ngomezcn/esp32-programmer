using System;
using System.IO;
using LibGit2Sharp;

public class FirmwareRepository
{
  private readonly string _repoUrl;
  private readonly string _localPath;

  public FirmwareRepository(string repoUrl, string localPath)
  {
    _repoUrl = repoUrl;
    _localPath = localPath;
  }

  public void FetchFirmware()
  {
    ConsoleHelper.PrintSectionHeader("Comprobando firmware");

    if (Directory.Exists(_localPath))
    {
      ConsoleHelper.PrintInfo("Verificando actualizaciones en el repositorio...");


      using (var repo = new Repository(_localPath))
      {
        // Realiza un fetch de los cambios remotos
        Commands.Fetch(repo, "origin", new[] { "refs/heads/*:refs/remotes/origin/*" }, null, null);

        // Compara el hash de la rama local con el hash de la rama remota
        var localBranch = repo.Branches["main"]; // Cambia "main" si usas otra rama
        var remoteBranch = repo.Branches["origin/main"]; // Cambia "main" si usas otra rama

        if (localBranch.Tip.Sha != remoteBranch.Tip.Sha)
        {
          ConsoleHelper.PrintWarning("Hay actualizaciones disponibles en la nube.");
          ConsoleHelper.PrintWarning($"ID del commit remoto: {remoteBranch.Tip.Sha}");

          // Pregunta al usuario si quiere descargar las actualizaciones
          ConsoleHelper.PrintUserInput("¿Quieres utilizar este puerto? (s/n): ");
          string respuesta = Console.ReadLine()?.Trim().ToUpper();

          if (respuesta == "s" || respuesta == "S")
          {
            // Realiza el pull para actualizar la rama local
            var signature = new Signature("usuario", "email@ejemplo.com", DateTime.Now); // Cambia por tus datos
            var options = new PullOptions();
            Commands.Pull(repo, signature, options);
            ConsoleHelper.PrintSuccess("Firmware actualizado con éxito.");

          }
          else
          {
            Console.WriteLine("Actualizaciones no descargadas.");
          }
        }
        else
        {
          ConsoleHelper.PrintSuccess("El firmware está actualizado.");
        }
      }
    }
    else
    {
      ConsoleHelper.PrintInfo("Clonando el repositorio...");
      Repository.Clone(_repoUrl, _localPath);
      ConsoleHelper.PrintSuccess("Firmware clonado en: " + _localPath);
    }
  }
}

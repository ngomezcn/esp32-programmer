using System;
using System.Collections.Generic; // Necesario para usar List
using System.Diagnostics;

public class PowerShellExecutor
{
  // Propiedades para configurar el comportamiento del proceso
  public bool UseShellExecute { get; set; } = false;
  public bool CreateNoWindow { get; set; } = true;
  public string ExecutionPolicy { get; set; } = "Bypass"; // Política de ejecución por defecto

  // Lista de cadenas de error a verificar
  private readonly List<string> errorKeywords = new List<string>
    {
        "error",
        "exception",
        "fail",
    };

  public PowerShellExecutor() { }

  public bool ExecuteCommand(string command, string path)
  {
    try
    {
      // Crea un nuevo proceso
      using (Process proceso = new Process())
      {
        proceso.StartInfo.FileName = "powershell.exe"; // Usa powershell.exe para ejecutar comandos
        proceso.StartInfo.WorkingDirectory = path; // Establecer el directorio de trabajo
        proceso.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy {ExecutionPolicy} -Command \"{command}\""; // Ejecuta el comando en PowerShell
        proceso.StartInfo.RedirectStandardOutput = true; // Redirige la salida estándar
        proceso.StartInfo.RedirectStandardError = true; // Redirige la salida de error estándar
        proceso.StartInfo.UseShellExecute = UseShellExecute; // Usar configuración definida
        proceso.StartInfo.CreateNoWindow = CreateNoWindow; // Crear ventana según configuración

        // Variable para rastrear si se encontraron errores
        bool hasError = false;

        // Suscribirse al evento para recibir la salida del proceso en tiempo real
        proceso.OutputDataReceived += (sender, e) =>
        {
          if (!string.IsNullOrEmpty(e.Data))
          {
            // Verifica si la salida contiene palabras clave de error
            if (ContainsErrorKeyword(e.Data))
            {
              ConsoleHelper.PrintError(e.Data);
              hasError = true;
            }
            else
            {
              ConsoleHelper.PrintResponse(e.Data);
            }
          }
        };

        // Suscribirse al evento para recibir la salida de error
        proceso.ErrorDataReceived += (sender, e) =>
        {
          if (!string.IsNullOrEmpty(e.Data))
          {
            ConsoleHelper.PrintError(e.Data);
            hasError = true; // Se encontró un error
          }
        };

        // Inicia el proceso
        proceso.Start();
        proceso.BeginOutputReadLine(); // Comienza a leer la salida en tiempo real
        proceso.BeginErrorReadLine(); // Comienza a leer la salida de error en tiempo real
        proceso.WaitForExit(); // Espera a que el proceso termine

        return !hasError; // Devuelve true si no hubo errores
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Ocurrió un error al ejecutar el comando: {ex.Message}");
      return false; // Devuelve false si ocurre una excepción
    }
  }

  private bool ContainsErrorKeyword(string message)
  {
    foreach (var keyword in errorKeywords)
    {
      if (message.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
      {
        return true;
      }
    }
    return false;
  }
}

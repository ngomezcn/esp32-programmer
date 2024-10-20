using System;

public static class ConsoleHelper
{
  public static void PrintInfo(string message)
  {
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("[INFO] " + message);
    Console.ResetColor();
  }

  public static void PrintSuccess(string message)
  {
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("[ÉXITO] " + message);
    Console.ResetColor();
  }

  public static void PrintError(string message)
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[ERROR] " + message);
    Console.ResetColor();
  }

  public static void PrintWarning(string message)
  {
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("[ADVERTENCIA] " + message);
    Console.ResetColor();
  }

  public static void PrintTitle(string message)
  {
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine("=== " + message.ToUpper() + " ===");
    Console.ResetColor();
  }

  public static void PrintSectionHeader(string message)
  {
    Console.WriteLine("");
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("--- " + message + " ---");
    Console.ResetColor();
  }

  public static void PrintDecorated(string message, ConsoleColor color, char decoration = '*', int decorationCount = 10)
  {
    string border = new string(decoration, decorationCount);
    Console.ForegroundColor = color;
    Console.WriteLine(border + " " + message + " " + border);
    Console.ResetColor();
  }

  public static void PrintCommand(string message)
  {
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.Write(">> ");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(message);
    Console.ResetColor();
  }

  internal static void PrintResponse(string message)
  {
    string[] lines = message.Split("\r\n").Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

    for (int i = 0; i < lines.Length; i++)
    {
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.Write("> ");
      Console.ResetColor();
      Console.WriteLine(lines[i]);
      Console.ResetColor();
    }

    Console.WriteLine();
  }

  public static void PrintUserInput(string prompt)
  {
    Console.ForegroundColor = ConsoleColor.Yellow; // Color del texto del prompt
    Console.WriteLine(prompt); // Muestra el mensaje de solicitud al usuario
    Console.ResetColor(); // Reinicia el color de la consola
  }

  internal static void PrintDeviceInfo(DeviceInfo device)
  {
    ConsoleHelper.PrintTitle("FINALIZADO");
    ConsoleHelper.PrintTitle("INFORMACIÓN DEL DISPOSITIVO");

    ConsoleHelper.PrintSectionHeader("Detalles del Dispositivo");
    ConsoleHelper.PrintInfo($"Nombre Interno: {device.InternalDeviceName}");
    ConsoleHelper.PrintInfo($"Clave de Acceso: {device.AccessKey}");
    ConsoleHelper.PrintInfo($"Cadena de Conexión: {device.ConnectionString}");
    ConsoleHelper.PrintInfo($"Cadena de Conexión (Encriptada): {device.ConnectionStringEncrypted}");
    ConsoleHelper.PrintInfo($"Marca de Tiempo: {device.TimeStamp}");
    ConsoleHelper.PrintInfo($"Versión del Chip: {device.ChipVersion}");
    ConsoleHelper.PrintInfo($"MAC Bluetooth: {device.MacBluetooth}");
    ConsoleHelper.PrintInfo($"Hash MAC Bluetooth: {device.MacBluetoothHash}");
    ConsoleHelper.PrintInfo($"ID del Chip: {device.ChipID}");
    ConsoleHelper.PrintInfo($"Frecuencia de CPU (MHz): {device.CpuFreqMHz}");
    ConsoleHelper.PrintInfo($"Tamaño de Flash (KB): {device.FlashSizeKB}");
    ConsoleHelper.PrintInfo($"RAM Libre (KB): {device.FreeRAMKB}");
  }

  internal static void PrintAppSettings()
  {
    ConsoleHelper.PrintSectionHeader("Cargando la configuración");
    ConsoleHelper.PrintInfo($"Credenciales de Google: {AppSettings.GoogleCredentials}");
    ConsoleHelper.PrintInfo($"Repositorio de Firmware: {AppSettings.FirmwareRepo}");
    ConsoleHelper.PrintInfo($"Carpeta de Firmware: {AppSettings.FirmwareFolder}");
    ConsoleHelper.PrintInfo($"Fuente BLE de Firmware: {AppSettings.FirmwareBleSrc}");
    ConsoleHelper.PrintInfo($"Duración de Activación del Relé (ms): {AppSettings.RelayActivationDuration}");
    ConsoleHelper.PrintInfo($"Tasa de Baudios Serial: {AppSettings.SerialBaudRate}");
  }
}

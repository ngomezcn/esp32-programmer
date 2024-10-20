using System;
using System.IO;
using System.Text.Json;

public static class AppSettings
{
  public static string GoogleCredentials { get; private set; }
  public static string FirmwareRepo { get; private set; }
  public static string FirmwareFolder { get; private set; }
  public static string FirmwareBleSrc { get; private set; }
  public static int RelayActivationDuration { get; private set; }
  public static int SerialBaudRate { get; set; }


  public static void Initialize(string configFilePath)
  {
    try
    {
      // Lee el contenido del archivo JSON
      var jsonString = File.ReadAllText(configFilePath);
      var config = JsonSerializer.Deserialize<Config>(jsonString);

      // Carga los valores en las propiedades estáticas
      GoogleCredentials = config.AppSettings.GoogleCredentials;
      FirmwareRepo = config.AppSettings.FirmwareRepo;
      FirmwareFolder = config.AppSettings.FirmwareFolder;
      FirmwareBleSrc = config.AppSettings.FirmwareBleSrc;
      RelayActivationDuration = config.AppSettings.RelayActivationDuration;
      SerialBaudRate = config.AppSettings.SerialBaudRate;

    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error al cargar el archivo de configuración: {ex.Message}");
      throw ex;
    }
  }

  private class Config
  {
    public AppSettingsSection AppSettings { get; set; }
  }

  private class AppSettingsSection
  {
    public string GoogleCredentials { get; set; }
    public string FirmwareRepo { get; set; }
    public string FirmwareFolder { get; set; }
    public string FirmwareBleSrc { get; set; }
    public int RelayActivationDuration { get; set; }
    public int SerialBaudRate { get; set; }

  }
}

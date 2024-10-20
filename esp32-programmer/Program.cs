using Google.Apis.Sheets.v4.Data;

class Program
{
  static void Main(string[] args)
  {

    AppSettings.Initialize("appsettings.json");

    SerialPortManager serialPortManager = new SerialPortManager();
    PowerShellExecutor powerShellExecutor = new PowerShellExecutor();
    FileManager fileManager = new FileManager();
    QrGenerator qrGenerator = new QrGenerator();
    FirmwareRepository firmwareRepository = new FirmwareRepository(AppSettings.FirmwareRepo, AppSettings.FirmwareFolder);
    PortSelector portSelector = new PortSelector();
    SpreadsheetManager spreadsheetManager = new SpreadsheetManager(AppSettings.GoogleCredentials);

    ConsoleHelper.PrintTitle("Gestión de Dispositivos ESP32");
    ConsoleHelper.PrintAppSettings();

    firmwareRepository.FetchFirmware();
    string selectedPort = portSelector.SelectPort();
    if (string.IsNullOrEmpty(selectedPort))
    {
      ConsoleHelper.PrintError("No se seleccionó ningún puerto. Saliendo...");
      return;
    }

    try
    {
      serialPortManager.SetPort(selectedPort);
      serialPortManager.SetBaudRate(AppSettings.SerialBaudRate);
      Esp32CommandHandler esp32CommandHandler = new Esp32CommandHandler(serialPortManager, powerShellExecutor);

      DeviceManager deviceManager = new DeviceManager(esp32CommandHandler, spreadsheetManager, fileManager, qrGenerator);

      deviceManager.ProgramDevice();
      ConsoleHelper.PrintSuccess("Dispositivo programado con éxito.");
    }
    catch (Exception ex)
    {
      ConsoleHelper.PrintError("Error: " + ex.Message);
    }
    finally
    {
      // Cerrar la conexión
      serialPortManager.Disconnect();
      ConsoleHelper.PrintInfo("Conexión cerrada.");
    }
  }
}

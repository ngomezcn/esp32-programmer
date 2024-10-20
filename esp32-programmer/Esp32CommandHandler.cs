using Google.Apis.Sheets.v4.Data;
using System;
using System.Diagnostics;
using System.Threading;
using static Google.Apis.Requests.BatchRequest;

public class Esp32CommandHandler
{
  private readonly SerialPortManager _serialPortManager;
  private readonly PowerShellExecutor _powerShellExecutor;

  public Esp32CommandHandler(SerialPortManager serialPortManager, PowerShellExecutor powerShellExecutor)
  {
    _serialPortManager = serialPortManager;
    _powerShellExecutor = powerShellExecutor;
  }
  public void PioUploadBle()
  {
    string path = AppSettings.FirmwareFolder + "\\esp32-device-ble";
    string command = $"pio run --target upload --upload-port {_serialPortManager.PortName}";

    ConsoleHelper.PrintCommand(command);
    if (!_powerShellExecutor.ExecuteCommand(command, path))
    {
      throw new InvalidOperationException("Error subiendo el codigo al ESP32");

    }

    _serialPortManager.ResetSerial();
  }

  public void PioUploadInisght()
  {
    string path = AppSettings.FirmwareFolder + "\\esp32-device-inisght";
    string command = $"pio run --target upload --upload-port {_serialPortManager.PortName}";

    ConsoleHelper.PrintCommand(command);
    if (!_powerShellExecutor.ExecuteCommand(command, path))
    {
      throw new InvalidOperationException("Error subiendo el codigo al ESP32");

    }
    _serialPortManager.ResetSerial();
  }

  public string GetDeviceInfo()
  {
    string command = "get_info()";

    ConsoleHelper.PrintCommand(command);
    string response = _serialPortManager.SendCommand(command);
    ConsoleHelper.PrintResponse($"DeviceInfo: {response}");

    _serialPortManager.ResetSerial();
    return response;
  }
}

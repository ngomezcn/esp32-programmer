using Google.Apis.Sheets.v4.Data;
using System;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class DeviceManager
{
  private readonly Esp32CommandHandler _esp32CommandHandler;
  private readonly SpreadsheetManager _spreadsheetManager;
  private readonly FileManager _fileManager;
  private readonly QrGenerator _qrGenerator;

  private const string spreadsheetId = "14Yu18XuKcnKui1i87jyPgpIyIfTeNHb1zYjjRM-uVvs"; // TODO PARAMETRIZAR
  private const string sheet_devices_history = "Sheet1"; // TODO PARAMETRIZAR

  public DeviceManager(Esp32CommandHandler atCommandHandler, SpreadsheetManager spreadsheetManager, FileManager fileManager, QrGenerator qrGenerator)
  {
    _esp32CommandHandler = atCommandHandler;
    _spreadsheetManager = spreadsheetManager;
    _fileManager = fileManager;
    _qrGenerator = qrGenerator;
  }

  private string StandardizeMacAddress(string macAddress)
  {
    string[] parts = macAddress.Split(':');
    for (int i = 0; i < parts.Length; i++)
    {
      if (parts[i].Length == 1)
      {
        parts[i] = "0" + parts[i];  // Añadir un '0' al frente si el fragmento tiene solo un carácter
      }
    }
    return string.Join(":", parts);  // Reunir las partes de nuevo en una cadena
  }

  private DeviceInfo ParseDeviceInfo(string data)
  {
    DeviceInfo deviceInfo = new DeviceInfo();

    string[] pairs = data.Split(',');

    foreach (var pair in pairs)
    {
      string[] keyValue = pair.Split('=');
      if (keyValue.Length == 2)
      {
        string key = keyValue[0];
        string value = keyValue[1];

        switch (key)
        {
          case "chipVersion":
            deviceInfo.ChipVersion = value;
            break;
          case "macBluetooth":
            deviceInfo.RawMacBluetooth = value;
            break;
          case "chipID":
            deviceInfo.ChipID = value;
            break;
          case "cpuFreqMHz":
            deviceInfo.CpuFreqMHz = int.Parse(value);
            break;
          case "flashSizeKB":
            deviceInfo.FlashSizeKB = int.Parse(value);
            break;
          case "freeRAMKB":
            deviceInfo.FreeRAMKB = int.Parse(value);
            break;
        }
      }
    }

    deviceInfo.StandardizedMacBluetooth = StandardizeMacAddress(deviceInfo.RawMacBluetooth).ToUpper();

    deviceInfo.MacBluetoothHash = CryptoHelper.Encode(deviceInfo.RawMacBluetooth);
    deviceInfo.AccessKey = GenerateAccessKey(deviceInfo.RawMacBluetooth);
    deviceInfo.InternalDeviceName = $"SG-{GenerateDeviceId(deviceInfo.RawMacBluetooth)}-XY";
    deviceInfo.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    deviceInfo.ConnectionString = $"BLE,{deviceInfo.StandardizedMacBluetooth},{deviceInfo.InternalDeviceName},{deviceInfo.AccessKey},000";
    deviceInfo.ConnectionStringEncrypted = CryptoHelper.Encode(deviceInfo.ConnectionString);

    return deviceInfo;
  }

  private bool CreateCppHeader(string code)
  {
    string name = "compile_defines";
    string extension = ".h";

    ConsoleHelper.PrintInfo($"Generando {name}{extension}");
    ConsoleHelper.PrintInfo(code);

    string path = $"{Directory.GetCurrentDirectory()}\\{AppSettings.FirmwareBleSrc}";


    bool status = _fileManager.CreateFile(path, name, extension, code);
    ConsoleHelper.PrintSuccess($"Generado correctamente {path}");

    return status;
  }


  public void ProgramDevice()
  {

    ConsoleHelper.PrintSectionHeader("Preparando el Dispositivo");


    ConsoleHelper.PrintInfo("Ejecutando PioUploadInisght...");
    _esp32CommandHandler.PioUploadInisght();

    ConsoleHelper.PrintInfo("Ejecutando GetDeviceInfo...");
    String rawDeviceInfo = _esp32CommandHandler.GetDeviceInfo();

    DeviceInfo deviceInfo = ParseDeviceInfo(rawDeviceInfo);

    string compileDefines =
    $"#define ACCESS_KEY \"{deviceInfo.AccessKey}\" \n" +
    $"#define DEVICE_NAME \"{deviceInfo.InternalDeviceName}\"  \n" +
    $"#define RELAY_ACTIVATION_DURATION {AppSettings.RelayActivationDuration}";

    if (this.CreateCppHeader(compileDefines))
    {

      ConsoleHelper.PrintInfo("Ejecutando PioUploadBle...");
      _esp32CommandHandler.PioUploadBle();
    }

    ConsoleHelper.PrintDeviceInfo(deviceInfo);

    ConsoleHelper.PrintSectionHeader("Acceso QR");
    string qrCodeAsAscii = _qrGenerator.GenerateQrCode(deviceInfo.ConnectionString);
    Console.WriteLine(qrCodeAsAscii);

    _spreadsheetManager.SetSpreadsheetId(spreadsheetId);
    _spreadsheetManager.SetSheetName(sheet_devices_history);

    IList<object> nuevaFila = new List<object>
    {
      deviceInfo.TimeStamp,
      deviceInfo.InternalDeviceName,
      deviceInfo.StandardizedMacBluetooth,
      deviceInfo.AccessKey,
      deviceInfo.ConnectionString,
      deviceInfo.ChipID,
      deviceInfo.ChipVersion,
      deviceInfo.CpuFreqMHz,
      deviceInfo.FlashSizeKB,
      deviceInfo.FreeRAMKB,
      deviceInfo.RawMacBluetooth,
      deviceInfo.ConnectionStringEncrypted,
      deviceInfo.MacBluetoothHash
    };

    _spreadsheetManager.CreateRow(nuevaFila, "A1");
  }

  public static string GenerateBlePin(string input)
  {
    string digits = "";

    foreach (char c in input)
    {
      if (char.IsDigit(c))
      {
        digits += c;
        if (digits.Length >= 4)
        {
          break;
        }
      }
    }

    return digits.PadLeft(4, '0');
  }

  public static string GenerateDeviceId(string input)
  {
    string digits = "";

    for (int i = input.Length - 1; i >= 0; i--)
    {
      char c = input[i];
      if (char.IsDigit(c))
      {
        digits = c + digits;

        if (digits.Length >= 4)
        {
          break;
        }
      }
    }

    return digits.PadLeft(4, '0');
  }

  public static string GenerateAccessKey(string input)
  {
    // Crear un hash SHA256 de la cadena de entrada
    using (SHA256 sha256 = SHA256.Create())
    {
      byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
      // Convertir el hash a una cadena hexadecimal
      StringBuilder hashString = new StringBuilder();
      foreach (byte b in hashBytes)
      {
        hashString.Append(b.ToString("x2")); // Convertir cada byte a hexadecimal
      }

      // Tomar los primeros 4 caracteres del hash
      string accessKey = hashString.ToString().Substring(0, 4).ToUpper();

      // Asegurarse de que solo haya letras o números
      StringBuilder finalKey = new StringBuilder();
      foreach (char c in accessKey)
      {
        if (char.IsLetterOrDigit(c))
        {
          finalKey.Append(c);
        }
        // Detenerse si ya tenemos 4 caracteres válidos
        if (finalKey.Length == 4)
        {
          break;
        }
      }

      // Si no hay suficientes caracteres válidos, completar con '0'
      while (finalKey.Length < 4)
      {
        finalKey.Append('0');
      }

      return finalKey.ToString();
    }
  }
}



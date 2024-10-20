using System;
using System.IO.Ports;
using System.Threading;

public class SerialPortManager
{
  public SerialPort SerialPort { get; private set; }
  private string portName;
  private int baudRate; // Eliminar el valor por defecto

  public string PortName
  {
    get => portName; // Getter para portName
    set => portName = value; // Setter para portName
  }

  public int BaudRate
  {
    get => baudRate; // Getter para baudRate
    set => baudRate = value; // Setter para baudRate
  }

  public bool IsConnected => SerialPort != null && SerialPort.IsOpen;

  // Constructor vacío
  public SerialPortManager() { }

  // Método para establecer el puerto
  public void SetPort(string portName)
  {
    PortName = portName; // Utiliza el setter
  }

  // Método para establecer el baudRate
  public void SetBaudRate(int baudRate)
  {
    BaudRate = baudRate; // Utiliza el setter
  }

  // Método para conectar el puerto
  public void Connect()
  {
    if (string.IsNullOrEmpty(PortName))
    {
      throw new InvalidOperationException("El puerto no está establecido. Usa el método Set() primero.");
    }

    // Verificar que se ha establecido un baudRate
    if (baudRate <= 0)
    {
      throw new InvalidOperationException("El baudRate no está establecido. Usa el método SetBaudRate() primero.");
    }

    SerialPort = new SerialPort(PortName, BaudRate, Parity.None, 8, StopBits.One);
    try
    {
      SerialPort.Open();
      ConsoleHelper.PrintInfo($"Conexión establecida con el puerto: {PortName} a {BaudRate} bps");
    }
    catch (Exception ex)
    {
      ConsoleHelper.PrintError("Error al conectar: " + ex.Message);
      throw;
    }
  }

  public void Disconnect()
  {
    if (IsConnected)
    {
      SerialPort.Close();
      ConsoleHelper.PrintInfo($"Conexión cerrada");
    }
    Thread.Sleep(500);
  }

  public string SendCommand(string command)
  {
    // Intentar conectar
    this.Connect();

    // Tiempo de espera total (6 segundos)
    int timeout = 6000;
    // Intervalo entre comprobaciones (1 segundo)
    int interval = 1000;
    int elapsedTime = 0;

    // Bucle de comprobación con tiempo de espera de 6 segundos
    while (!this.IsConnected && elapsedTime < timeout)
    {
      Thread.Sleep(interval);
      elapsedTime += interval;

      // Intentar conectar nuevamente en cada iteración
      this.Connect();

      // Si en algún momento se conecta, salimos del bucle
      if (this.IsConnected)
      {
        break;
      }
    }

    // Verificar si al final del bucle está conectado
    if (!this.IsConnected)
    {
      throw new InvalidOperationException("El puerto serie no está conectado después de 6 segundos.");
    }

    // Enviar el comando
    this.SerialPort.Write(command + "\r\n");

    // Esperar la respuesta (puedes ajustar el tiempo de espera aquí si es necesario)
    Thread.Sleep(1000);
    string response = this.SerialPort.ReadExisting();
    this.Disconnect();

    return response;
  }

  public void ResetSerial()
  {
    ConsoleHelper.PrintInfo("Saneando la conexión serial");
    try
    {
      // Conectar al puerto
      this.Connect();

      // Esperar 1 segundo
      Thread.Sleep(1000);

      // Desconectar del puerto
      this.Disconnect();
    }
    catch (Exception ex)
    {
      Console.WriteLine("Error durante el reset del puerto serie: " + ex.Message);
      throw;
    }

    ConsoleHelper.PrintInfo("Saneado");
    Console.WriteLine();
  }
}

using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

public class PortSelector
{
  public string SelectPort()
  {
    // Mostrar los puertos conectados al inicio
    string[] initialPorts = SerialPort.GetPortNames();
    ConsoleHelper.PrintSectionHeader("Dispositivos conectados inicialmente:");
    if (initialPorts.Length == 0)
    {
      ConsoleHelper.PrintInfo("No se encontraron dispositivos conectados.");
    }
    else
    {
      for (int i = 0; i < initialPorts.Length; i++)
      {
        ConsoleHelper.PrintInfo($"{i + 1}: {initialPorts[i]}");
      }
    }

    // Mensaje de espera para la conexión del ESP32
    ConsoleHelper.PrintUserInput("Esperando la conexión del dispositivo ESP32...");

    string[] currentPorts;
    string esp32Port = null;

    // Bucle que espera hasta que se detecte un nuevo puerto (como el ESP32)
    while (true)
    {
      currentPorts = SerialPort.GetPortNames();

      // Detectar si hay un nuevo puerto en comparación con los iniciales
      var newPorts = currentPorts.Except(initialPorts).ToArray();

      if (newPorts.Length > 0)
      {
        esp32Port = newPorts.First(); // Asumimos que el primer nuevo puerto es el ESP32
        ConsoleHelper.PrintInfo($"Puerto COM detectado: {esp32Port}");

        // Preguntar al usuario si desea utilizar el nuevo puerto o seleccionar otro
        ConsoleHelper.PrintUserInput("¿Quieres utilizar este puerto? (s/n): ");
        string respuesta = Console.ReadLine()?.Trim().ToUpper();

        if (respuesta == "S" || respuesta == "s")
        {
          return esp32Port;
        }
        else
        {
          break;
        }
      }


      Thread.Sleep(500);
    }

    // Si el bucle se rompe, mostrar la lista de puertos
    ConsoleHelper.PrintSectionHeader("Lista de puertos disponibles:");
    for (int i = 0; i < currentPorts.Length; i++)
    {
      ConsoleHelper.PrintInfo($"{i + 1}: {currentPorts[i]}");
    }

    Console.Write("Selecciona el número del puerto serie: ");
    if (!int.TryParse(Console.ReadLine(), out int portIndex2) || portIndex2 < 1 || portIndex2 > currentPorts.Length)
    {
      ConsoleHelper.PrintError("Selección inválida. Saliendo...");
      return null;
    }

    return currentPorts[portIndex2 - 1]; // Devolver el puerto seleccionado manualmente
  }


}

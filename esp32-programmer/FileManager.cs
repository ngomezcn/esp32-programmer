using System;
using System.IO;

public class FileManager
{

  public bool CreateFile(string path, string name, string extension, string text)
  {
    string fileName = $"{name}{extension}";
    string content = text;
    string filePath = Path.Combine(path, fileName);

    try
    {
      File.WriteAllText(filePath, content);
      return true;
    }
    catch (Exception ex)
    {
      ConsoleHelper.PrintError($"Error al crear el archivo: {ex.Message}");
      return false; // Si ocurre un error, devolvemos false
    }
  }
}

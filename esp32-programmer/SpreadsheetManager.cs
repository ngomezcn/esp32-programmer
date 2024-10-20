using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;

public class SpreadsheetManager
{
  private readonly SheetsService _service;
  private string _spreadsheetId;
  private string _sheetName;  // Ahora lo configuraremos con un setter

  public SpreadsheetManager(string credentials)
  {
    var credential = GoogleCredential.FromFile(credentials).CreateScoped(SheetsService.Scope.Spreadsheets);
    _service = new SheetsService(new BaseClientService.Initializer()
    {
      HttpClientInitializer = credential,
      ApplicationName = "Google Sheets API .NET Quickstart",
    });
  }

  // Método para establecer el spreadsheetId dinámicamente
  public void SetSpreadsheetId(string spreadsheetId)
  {
    _spreadsheetId = spreadsheetId;
  }

  // Método para establecer el sheetName dinámicamente
  public void SetSheetName(string sheetName)
  {
    _sheetName = sheetName;
  }

  // Verificar que el spreadsheetId y sheetName estén configurados
  private void VerificarConfiguraciones()
  {
    if (string.IsNullOrEmpty(_spreadsheetId))
      throw new InvalidOperationException("El Spreadsheet ID no ha sido establecido.");
    if (string.IsNullOrEmpty(_sheetName))
      throw new InvalidOperationException("El nombre de la hoja no ha sido establecido.");
  }

  // Crear una nueva fila
  public void CreateRow(IList<object> rowData, string startCell = "A1")
  {
    VerificarConfiguraciones();
    var range = $"{_sheetName}!{startCell}";
    var valueRange = new ValueRange { Values = new List<IList<object>> { rowData } };

    var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _spreadsheetId, range);
    appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
    appendRequest.Execute();

  }

  // Leer filas de un rango
  public IList<IList<object>> ReadRows(string range)
  {
    VerificarConfiguraciones();
    var fullRange = $"{_sheetName}!{range}";
    var request = _service.Spreadsheets.Values.Get(_spreadsheetId, fullRange);
    var response = request.Execute();
    return response.Values;
  }

  // Actualizar una fila existente
  public void UpdateRow(IList<object> rowData, string startCell = "A1")
  {
    VerificarConfiguraciones();
    var range = $"{_sheetName}!{startCell}";
    var valueRange = new ValueRange { Values = new List<IList<object>> { rowData } };

    var updateRequest = _service.Spreadsheets.Values.Update(valueRange, _spreadsheetId, range);
    updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
    updateRequest.Execute();

    Console.WriteLine("Fila actualizada correctamente.");
  }

  // Eliminar una fila
  public void DeleteRow(int sheetId, int startIndex, int endIndex)
  {
    VerificarConfiguraciones();
    var requests = new List<Request>
        {
            new Request
            {
                DeleteDimension = new DeleteDimensionRequest
                {
                    Range = new DimensionRange
                    {
                        SheetId = sheetId,
                        Dimension = "ROWS",
                        StartIndex = startIndex,
                        EndIndex = endIndex
                    }
                }
            }
        };

    var batchUpdateRequest = new BatchUpdateSpreadsheetRequest { Requests = requests };
    _service.Spreadsheets.BatchUpdate(batchUpdateRequest, _spreadsheetId).Execute();

    Console.WriteLine("Fila eliminada.");
  }
}

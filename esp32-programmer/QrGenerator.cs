using System;
using QRCoder;

public class QrGenerator
{
  public string GenerateQrCode(string text)
  {
    if (string.IsNullOrEmpty(text))
    {
      throw new ArgumentException("El texto no puede estar vacío.", nameof(text));
    }

    // Crear el generador de códigos QR
    using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
    {
      // Generar los datos del código QR
      QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
      AsciiQRCode qrCode = new AsciiQRCode(qrCodeData);
      // Obtener el gráfico ASCII del código QR
      return qrCode.GetGraphic(1);
    }
  }
}

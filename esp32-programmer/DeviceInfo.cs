using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class DeviceInfo
{
  public string InternalDeviceName { get; set; }
  public string AccessKey { get; set; }
  public string ConnectionString { get; set; }
  public string ConnectionStringEncrypted { get; set; }
  public string TimeStamp { get; set; }

  public string ChipVersion { get; set; }
  public string MacBluetooth { get; set; }
  public string MacBluetoothHash { get; set; }
  public string ChipID { get; set; }
  public int CpuFreqMHz { get; set; }
  public int FlashSizeKB { get; set; }
  public int FreeRAMKB { get; set; }
}

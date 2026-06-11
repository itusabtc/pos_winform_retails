using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace NailsChekin.Models.Helper
{
    public static class RawPrinterHelper
    {
        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool OpenPrinter(string pPrinterName,
                                           out IntPtr phPrinter,
                                           IntPtr pDefault);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, int level,
                                                   ref DOC_INFO_1 pDocInfo);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool WritePrinter(IntPtr hPrinter,
                                                IntPtr pBytes,
                                                int dwCount,
                                                out int dwWritten);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DOC_INFO_1
        {
            public string pDocName;
            public string pOutputFile;
            public string pDataType;
        }

        public static bool SendBytes(string printerName, byte[] data, string jobName)
        {
            IntPtr hPrinter;
            if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
                return false;

            var di = new DOC_INFO_1
            {
                pDocName = jobName,
                pDataType = "RAW"
            };

            bool ok = StartDocPrinter(hPrinter, 1, ref di)
                      && StartPagePrinter(hPrinter);

            if (ok)
            {
                int dwWritten;
                var unmanagedPointer = Marshal.AllocCoTaskMem(data.Length);
                Marshal.Copy(data, 0, unmanagedPointer, data.Length);
                ok = WritePrinter(hPrinter, unmanagedPointer, data.Length, out dwWritten);
                Marshal.FreeCoTaskMem(unmanagedPointer);
                EndPagePrinter(hPrinter);
                EndDocPrinter(hPrinter);
            }

            ClosePrinter(hPrinter);
            return ok;
        }
    }
}

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace RadiologyCenter.Desktop.Security;

internal static class DpapiProtector
{
    private const string PlainPrefix = "plain:";
    private const string DpapiPrefix = "dpapi:";

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DATA_BLOB
    {
        public int cbData;
        public IntPtr pbData;
    }

    [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CryptProtectData(
        ref DATA_BLOB pDataIn,
        IntPtr szDataDescr,
        IntPtr pOptionalEntropy,
        IntPtr pvReserved,
        IntPtr pPromptStruct,
        int dwFlags,
        out DATA_BLOB pDataOut);

    [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CryptUnprotectData(
        ref DATA_BLOB pDataIn,
        IntPtr ppszDataDescr,
        IntPtr pOptionalEntropy,
        IntPtr pvReserved,
        IntPtr pPromptStruct,
        int dwFlags,
        out DATA_BLOB pDataOut);

    [DllImport("kernel32.dll")]
    private static extern IntPtr LocalFree(IntPtr hMem);

    public static string Protect(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        try
        {
            if (OperatingSystem.IsWindows())
            {
                var bytes = Encoding.UTF8.GetBytes(value);
                var input = new DATA_BLOB { cbData = bytes.Length, pbData = Marshal.AllocHGlobal(bytes.Length) };
                Marshal.Copy(bytes, 0, input.pbData, bytes.Length);
                try
                {
                    if (CryptProtectData(ref input, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, out var output))
                    {
                        var outBytes = new byte[output.cbData];
                        Marshal.Copy(output.pbData, outBytes, 0, output.cbData);
                        if (output.pbData != IntPtr.Zero)
                            LocalFree(output.pbData);
                        return DpapiPrefix + Convert.ToBase64String(outBytes);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(input.pbData);
                }
            }
        }
        catch
        {
            // DPAPI unavailable (e.g. elevated process restrictions) - fall through to plaintext.
        }

        Debug.WriteLine("[DpapiProtector] DPAPI unavailable – falling back to plaintext protection.");
        return PlainPrefix + value;
    }

    public static string Unprotect(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        if (value.StartsWith(PlainPrefix, StringComparison.Ordinal))
            return value[PlainPrefix.Length..];

        if (value.StartsWith(DpapiPrefix, StringComparison.Ordinal))
        {
            try
            {
                var bytes = Convert.FromBase64String(value[DpapiPrefix.Length..]);
                var input = new DATA_BLOB { cbData = bytes.Length, pbData = Marshal.AllocHGlobal(bytes.Length) };
                Marshal.Copy(bytes, 0, input.pbData, bytes.Length);
                try
                {
                    if (CryptUnprotectData(ref input, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, out var output))
                    {
                        var outBytes = new byte[output.cbData];
                        Marshal.Copy(output.pbData, outBytes, 0, output.cbData);
                        if (output.pbData != IntPtr.Zero)
                            LocalFree(output.pbData);
                        return Encoding.UTF8.GetString(outBytes);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(input.pbData);
                }
            }
            catch
            {
                // Corrupt or undecryptable value - treat as not present.
            }
        }

        return value;
    }
}
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace RadiologyCenter.Desktop.Security;

internal static class DpapiProtector
{
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
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Windows could not protect the saved clinic session.");
                }
                finally
                {
                    CryptographicOperations.ZeroMemory(bytes);
                    Marshal.Copy(bytes, 0, input.pbData, bytes.Length);
                    Marshal.FreeHGlobal(input.pbData);
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("The clinic session could not be saved securely on this Windows account.", ex);
        }

        throw new PlatformNotSupportedException("Saved clinic sessions require Windows data protection.");
    }

    public static string Unprotect(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

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
                        try { return Encoding.UTF8.GetString(outBytes); }
                        finally
                        {
                            CryptographicOperations.ZeroMemory(outBytes);
                            Marshal.Copy(outBytes, 0, output.pbData, outBytes.Length);
                            if (output.pbData != IntPtr.Zero) LocalFree(output.pbData);
                        }
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

        return string.Empty;
    }
}

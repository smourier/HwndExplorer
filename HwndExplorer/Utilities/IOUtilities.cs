using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace HwndExplorer.Utilities
{
    public static class IOUtilities
    {
        public const string LongFileNamePrefix = @"\\?\";
        public const string ApplicationOctetStream = "application/octet-stream";
        public static readonly DateTime MinFileTime = new(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public const int ERROR_WRITE_FAULT = unchecked((int)0x8007001D);

        public static long? PathGetSize(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var data = new WIN32_FILE_ATTRIBUTE_DATA();
            if (!GetFileAttributesEx(path, GetFileExInfoStandard, ref data))
                return null;

            if (data.fileAttributes.HasFlag(FileAttributes.Directory))
                return null;

            return data.fileSize;
        }

        public static FileAttributes? PathGetAttributes(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var data = new WIN32_FILE_ATTRIBUTE_DATA();
            if (!GetFileAttributesEx(path, GetFileExInfoStandard, ref data))
                return null;

            return data.fileAttributes;
        }

        public static DateTime? PathGetCreationTime(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var data = new WIN32_FILE_ATTRIBUTE_DATA();
            if (!GetFileAttributesEx(path, GetFileExInfoStandard, ref data))
                return null;

            if (data.ftCreationTime.IsZero)
                return null;

            return DateTime.FromFileTimeUtc(data.ftCreationTime.ToTicks()).ToLocalTime();
        }

        public static DateTime? PathGetLastAccessTime(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var data = new WIN32_FILE_ATTRIBUTE_DATA();
            if (!GetFileAttributesEx(path, GetFileExInfoStandard, ref data))
                return null;

            if (data.ftLastAccessTime.IsZero)
                return null;

            return DateTime.FromFileTimeUtc(data.ftLastAccessTime.ToTicks()).ToLocalTime();
        }

        public static DateTime? PathGetLastWriteTime(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var data = new WIN32_FILE_ATTRIBUTE_DATA();
            if (!GetFileAttributesEx(path, GetFileExInfoStandard, ref data))
                return null;

            if (data.ftLastWriteTime.IsZero)
                return null;

            return DateTime.FromFileTimeUtc(data.ftLastWriteTime.ToTicks()).ToLocalTime();
        }

        public static void PathSetCreationTime(string path, DateTime dateTime)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!Conversions.IsValidFileTime(dateTime))
                return;

            var data = new WIN32_FILE_ATTRIBUTE_DATA();
            if (!GetFileAttributesEx(path, GetFileExInfoStandard, ref data))
                return;

            if (DateTime.FromFileTimeUtc(data.ftCreationTime.ToTicks()) == dateTime)
                return;

            if (data.fileAttributes.HasFlag(FileAttributes.Directory))
            {
                Directory.SetCreationTime(path, dateTime);
            }
            else
            {
                File.SetCreationTime(path, dateTime);
            }
        }

        public static void PathSetLastAccessTime(string path, DateTime dateTime)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!Conversions.IsValidFileTime(dateTime))
                return;

            var data = new WIN32_FILE_ATTRIBUTE_DATA();
            if (!GetFileAttributesEx(path, GetFileExInfoStandard, ref data))
                return;

            if (DateTime.FromFileTimeUtc(data.ftLastAccessTime.ToTicks()) == dateTime)
                return;

            if (data.fileAttributes.HasFlag(FileAttributes.Directory))
            {
                Directory.SetLastAccessTime(path, dateTime);
            }
            else
            {
                File.SetLastAccessTime(path, dateTime);
            }
        }

        public static void PathSetLastWriteTime(string path, DateTime dateTime)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!Conversions.IsValidFileTime(dateTime))
                return;

            var data = new WIN32_FILE_ATTRIBUTE_DATA();
            if (!GetFileAttributesEx(path, GetFileExInfoStandard, ref data))
                return;

            if (DateTime.FromFileTimeUtc(data.ftLastWriteTime.ToTicks()) == dateTime)
                return;

            if (data.fileAttributes.HasFlag(FileAttributes.Directory))
            {
                Directory.SetLastWriteTime(path, dateTime);
            }
            else
            {
                File.SetLastWriteTime(path, dateTime);
            }
        }

        public static void PathSetCreationTimeUtc(string path, DateTime dateTime)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!Conversions.IsValidFileTime(dateTime))
                return;

            if (dateTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException(null, nameof(dateTime));

            var data = new WIN32_FILE_ATTRIBUTE_DATA();
            if (!GetFileAttributesEx(path, GetFileExInfoStandard, ref data))
                return;

            if (DateTime.FromFileTimeUtc(data.ftCreationTime.ToTicks()) == dateTime)
                return;

            if (data.fileAttributes.HasFlag(FileAttributes.Directory))
            {
                Directory.SetCreationTimeUtc(path, dateTime);
            }
            else
            {
                File.SetCreationTimeUtc(path, dateTime);
            }
        }

        public static void PathSetLastAccessTimeUtc(string path, DateTime dateTime)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!Conversions.IsValidFileTime(dateTime))
                return;

            if (dateTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException(null, nameof(dateTime));

            var data = new WIN32_FILE_ATTRIBUTE_DATA();
            if (!GetFileAttributesEx(path, GetFileExInfoStandard, ref data))
                return;

            if (DateTime.FromFileTimeUtc(data.ftLastAccessTime.ToTicks()) == dateTime)
                return;

            if (data.fileAttributes.HasFlag(FileAttributes.Directory))
            {
                Directory.SetLastAccessTimeUtc(path, dateTime);
            }
            else
            {
                File.SetLastAccessTimeUtc(path, dateTime);
            }
        }

        public static void PathSetLastWriteTimeUtc(string path, DateTime dateTime)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (dateTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException(null, nameof(dateTime));

            if (!Conversions.IsValidFileTime(dateTime))
                return;

            var data = new WIN32_FILE_ATTRIBUTE_DATA();
            if (!GetFileAttributesEx(path, GetFileExInfoStandard, ref data))
                return;

            if (DateTime.FromFileTimeUtc(data.ftLastWriteTime.ToTicks()) == dateTime)
                return;

            if (data.fileAttributes.HasFlag(FileAttributes.Directory))
            {
                Directory.SetLastWriteTimeUtc(path, dateTime);
            }
            else
            {
                File.SetLastWriteTimeUtc(path, dateTime);
            }
        }

        private const int GetFileExInfoStandard = 0;

        [StructLayout(LayoutKind.Sequential)]
        private struct WIN32_FILE_ATTRIBUTE_DATA
        {
            public FileAttributes fileAttributes;
            public FILE_TIME ftCreationTime;
            public FILE_TIME ftLastAccessTime;
            public FILE_TIME ftLastWriteTime;
            public long fileSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FILE_TIME
        {
            public uint ftTimeLow;
            public uint ftTimeHigh;

            public FILE_TIME(long fileTime)
            {
                ftTimeLow = (uint)fileTime;
                ftTimeHigh = (uint)(fileTime >> 32);
            }

            public bool IsZero => ftTimeHigh == 0 && ftTimeLow == 0;
            public long ToTicks() => ((long)ftTimeHigh << 32) + ftTimeLow;
        }

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private extern static bool GetFileAttributesEx(string name, int fileInfoLevel, ref WIN32_FILE_ATTRIBUTE_DATA lpFileInformation);

        [DllImport("kernel32", SetLastError = true)]
        private extern static bool SetFilePointerEx(SafeFileHandle hFile, long liDistanceToMove, out long lpNewFilePointer, SeekOrigin dwMoveMethod);

        [DllImport("kernel32", SetLastError = true)]
        private extern static bool SetEndOfFile(SafeFileHandle hFile);

        [DllImport("urlmon", CharSet = CharSet.Unicode)]
        private extern static int FindMimeFromData(nint pBC,
            [MarshalAs(UnmanagedType.LPWStr)] string? pwzUrl,
            byte[] pBuffer,
            int cbSize,
            string? pwzMimeProposed,
            uint dwMimeFlags,
            out nint ppwzMimeOut,
            int dwReserverd
            );

        public static string? FindContentType(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            const int FMFD_ENABLEMIMESNIFFING = 0x2;
            const int FMFD_RETURNUPDATEDIMGMIMES = 0x20;
            _ = FindMimeFromData(nint.Zero, null, bytes, bytes.Length, null, FMFD_RETURNUPDATEDIMGMIMES | FMFD_ENABLEMIMESNIFFING, out nint ptr, 0);
            if (ptr == nint.Zero)
                return null;

            var ct = Marshal.PtrToStringUni(ptr);
            Marshal.FreeCoTaskMem(ptr);
            return ct != ApplicationOctetStream ? ct : null;
        }

        public static string? FindContentType(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!PathIsFile(path))
                return null;

            var bytes = new byte[256];
            using (var file = File.OpenRead(path))
            {
                file.Read(bytes, 0, bytes.Length);
            }
            return FindContentType(bytes);
        }

        public static bool PathEnsureDirectory(string path, bool throwOnError = true)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!IsPathRooted(path))
            {
                path = Path.GetFullPath(path);
            }

            // for QonarQube...
            if (!Path.IsPathRooted(path))
            {
                path = Path.GetFullPath(path);
            }

            var dir = Path.GetDirectoryName(path);
            if (dir == null || PathIsDirectory(dir))
                return false;

            try
            {
                Directory.CreateDirectory(dir);
                return true;
            }
            catch
            {
                if (throwOnError)
                    throw;

                return false;
            }
        }

        public static void DirectoryDelete(string path, bool recursive = false, bool throwOnError = true)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!PathIsDirectory(path))
                return;

            try
            {
                Directory.Delete(path, recursive);
            }
            catch
            {
                if (throwOnError)
                    throw;
            }
        }

        public static void DirectoryCreate(string path, bool throwOnError = true)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (PathIsDirectory(path))
                return;

            try
            {
                Directory.CreateDirectory(path);
            }
            catch
            {
                if (!throwOnError)
                    return;

                if (PathIsDirectory(path))
                    return;

                throw;
            }
        }

        public static bool FileSetEnd(string path, long size, bool throwOnError = true)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            using (var file = File.OpenWrite(path))
            {
                if (!SetFilePointerEx(file.SafeFileHandle, size, out var newPointer, SeekOrigin.Begin))
                {
                    if (throwOnError)
                        throw new Win32Exception(Marshal.GetLastWin32Error());

                    return false;
                }

                if (!SetEndOfFile(file.SafeFileHandle))
                {
                    if (throwOnError)
                        throw new Win32Exception(Marshal.GetLastWin32Error());

                    return false;
                }

                return true;
            }
        }

        public static bool FileMove(string source, string destination, bool unprotect = true, bool throwOnError = true)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            // we don't use File.Move in case where we cannot delete target
            // this is safer, we delete the existing target and the source only if it succeeded
            var copy = FileOverwrite(source, destination, unprotect, throwOnError);
            if (!copy)
                return false;

            FileDelete(source, true, true);
            return true;
        }

        public static void DirectoryCopy(string sourcePath, string targetPath, bool throwOnError = true)
        {
            if (sourcePath == null)
                throw new ArgumentNullException(nameof(sourcePath));

            if (targetPath == null)
                throw new ArgumentNullException(nameof(targetPath));

            DirectoryCopy(new DirectoryInfo(sourcePath), new DirectoryInfo(targetPath), throwOnError);
        }

        public static void DirectoryCopy(DirectoryInfo source, DirectoryInfo target, bool throwOnError = true)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (!target.Exists)
            {
                if (throwOnError)
                {
                    target.Create();
                }
                else
                {
                    try
                    {
                        target.Create();
                    }
                    catch
                    {
                        return;
                    }
                }
            }

            foreach (var file in source.EnumerateFiles())
            {
                if (throwOnError)
                {
                    file.CopyTo(Path.Combine(target.FullName, file.Name), true);
                }
                else
                {
                    try
                    {
                        file.CopyTo(Path.Combine(target.FullName, file.Name), true);
                    }
                    catch
                    {
                        // do nothing
                    }
                }
            }

            foreach (var dir in source.EnumerateDirectories())
            {
                DirectoryInfo subDir;
                if (throwOnError)
                {
                    subDir = target.CreateSubdirectory(dir.Name);
                }
                else
                {
                    try
                    {
                        subDir = target.CreateSubdirectory(dir.Name);
                    }
                    catch
                    {
                        // do nothing
                        continue;
                    }
                }

                DirectoryCopy(dir, subDir, throwOnError);
            }
        }

        public static bool DirectoryMove(string source, string destination, bool throwOnError = true)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            if (!throwOnError && !PathIsDirectory(source))
                return false;

            if (throwOnError)
            {
                Directory.Move(source, destination);
            }
            else
            {
                try
                {
                    Directory.Move(source, destination);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public static bool FileDelete(string path, bool unprotect = true, bool throwOnError = true)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!PathIsFile(path))
                return false;

            if (throwOnError)
            {
                Delete();
            }
            else
            {
                try
                {
                    Delete();
                }
                catch
                {
                    return false;
                }
            }
            return true;

            void Delete()
            {
                var attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly && unprotect)
                {
                    File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
                }

                File.Delete(path);
            }
        }

        public static bool FileOverwrite(string source, string destination, bool unprotect = true, bool throwOnError = true)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            if (PathIsEqual(source, destination))
                return false;

            FileDelete(destination, unprotect, throwOnError);
            PathEnsureDirectory(destination, throwOnError);

            if (throwOnError)
            {
                File.Copy(source, destination, true);
            }
            else
            {
                try
                {
                    File.Copy(source, destination, true);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public static bool DirectoryExists(string path) => PathIsDirectory(path);
        public static string PathGetExtension(string path)
        {
            if (path == null)
                return string.Empty;

            var pos = path.LastIndexOf('.');
            if (pos < 0)
                return string.Empty;

            return path.Substring(pos);
        }

        public static string PathGetName(string path)
        {
            if (path == null)
                return string.Empty;

            var pos = path.LastIndexOf(Path.DirectorySeparatorChar);
            if (pos < 0)
                return path;

            return path.Substring(pos + 1);
        }

        public static string PathGetNameWithoutExtension(string path)
        {
            var name = PathGetName(path);
            var pos = name.LastIndexOf('.');
            if (pos < 0)
                return name;

            return name.Substring(0, pos);
        }

        public static bool PathIsEqual(string path1, string path2, bool normalize = true)
        {
            if (path1 == null)
                throw new ArgumentNullException(nameof(path1));

            if (path2 == null)
                throw new ArgumentNullException(nameof(path2));

            if (normalize)
            {
                path1 = Path.GetFullPath(path1);
                path2 = Path.GetFullPath(path2);
            }

            return path1.EqualsIgnoreCase(path2);
        }

        public static bool PathIsChildOrEqual(string path, string child, bool normalize = true) => PathIsChild(path, child, normalize) || PathIsEqual(path, child, normalize);
        public static bool PathIsChild(string path, string child, bool normalize = true) => PathIsChild(path, child, normalize, out _);
        public static bool PathIsChild(string path, string child, bool normalize, out string? subPath)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (child == null)
                throw new ArgumentNullException(nameof(child));

            subPath = null;

            try
            {
                if (normalize)
                {
                    path = Path.GetFullPath(path);
                    child = Path.GetFullPath(child);
                }

                path = StripTerminatingPathSeparators(path)!;
                if (child.Length < path.Length + 1)
                    return false;

                var newChild = Path.Combine(path, child.Substring(path.Length + 1));
                var b = newChild.EqualsIgnoreCase(child);
                if (b)
                {
                    subPath = child.Substring(path.Length);
                    while (subPath.StartsWith(Path.DirectorySeparatorChar.ToString()))
                    {
                        subPath = subPath.Substring(1);
                    }
                }
                return b;
            }
            catch
            {
                return false;
            }
        }

        public static string? StripTerminatingPathSeparators(string path)
        {
            if (path == null)
                return null;

            while (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path = path.Substring(0, path.Length - 1);
            }
            return path;
        }

        public static string? UrlCombine(params string[] urls)
        {
            if (urls == null)
                return null;

            var sb = new StringBuilder();
            foreach (var url in urls)
            {
                if (string.IsNullOrEmpty(url))
                    continue;

                if (sb.Length > 0 && sb[sb.Length - 1] != '/' && url[0] != '/')
                {
                    sb.Append('/');
                }
                sb.Append(url);
            }
            return sb.ToString();
        }

        private static readonly string[] _reservedFileNames = new[]
        {
            "con", "prn", "aux", "nul",
            "com0", "com1", "com2", "com3", "com4", "com5", "com6", "com7", "com8", "com9",
            "lpt0", "lpt1", "lpt2", "lpt3", "lpt4", "lpt5", "lpt6", "lpt7", "lpt8", "lpt9",
        };

        private static bool IsAllDots(string fileName)
        {
            foreach (var c in fileName)
            {
                if (c != '.')
                    return false;
            }
            return true;
        }

        private static int GetDriveNameEnd(string path)
        {
            var pos = path.IndexOf(':');
            if (pos < 0)
                return -1;

            var pos2 = path.IndexOfAny(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
            if (pos2 < pos)
                return -1;

            return pos;
        }

        private static int GetServerNameEnd(string path, out bool onlyServer)
        {
            onlyServer = false;
            if (!path.StartsWith(@"\\"))
                return -1;

            var pos = path.IndexOf(Path.DirectorySeparatorChar, 3);
            if (pos < 3)
                return -1;

            var pos2 = path.IndexOf(Path.DirectorySeparatorChar, pos + 1);
            if (pos2 < pos)
            {
                onlyServer = true;
                return -1;
            }
            return pos2;
        }

        public static string NormalizePath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!Path.IsPathRooted(path))
                return path;

            if (!path.StartsWith(LongFileNamePrefix))
                return LongFileNamePrefix + path;

            return path;
        }

        public static string DenormalizePath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!Path.IsPathRooted(path))
                return path;

            if (!path.StartsWith(LongFileNamePrefix))
                return path;

            return path.Substring(LongFileNamePrefix.Length);
        }

        public static string PathToValidFilePath(string path, string? reservedNameFormat = null, string? reservedCharFormat = null)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var sb = new StringBuilder(path.Length);
            var fn = new StringBuilder();
            var serverNameEnd = GetServerNameEnd(path, out bool onlyServer);
            if (onlyServer)
                return path;

            var start = 0;
            if (serverNameEnd >= 0)
            {
                // path includes? server name? just skip it, don't validate it
                start = serverNameEnd + 1;
            }
            else
            {
                var driveNameEnd = GetDriveNameEnd(path);
                if (driveNameEnd >= 0)
                {
                    start = driveNameEnd + 1;
                }
            }

            for (var i = start; i < path.Length; i++)
            {
                var c = path[i];
                if (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)
                {
                    if (fn.Length > 0)
                    {
                        sb.Append(PathToValidFileName(fn.ToString(), reservedNameFormat, reservedCharFormat));
                        fn.Length = 0;
                    }
                    sb.Append(c);
                    continue;
                }

                fn.Append(c);
            }

            if (fn.Length > 0)
            {
                sb.Append(PathToValidFileName(fn.ToString(), reservedNameFormat, reservedCharFormat));
            }

            var s = start == 0 ? sb.ToString() : path.Substring(0, start) + sb.ToString();
            if (s.EqualsIgnoreCase(path))
                return path;

            return s;
        }

        public static string PathToValidFileName(string fileName, string? reservedNameFormat = null, string? reservedCharFormat = null)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            if (string.IsNullOrWhiteSpace(reservedNameFormat))
            {
                reservedNameFormat = "_{0}_";
            }

            if (string.IsNullOrWhiteSpace(reservedCharFormat))
            {
                reservedCharFormat = "_x{0}_";
            }

            if (Array.IndexOf(_reservedFileNames, fileName.ToLowerInvariant()) >= 0 || IsAllDots(fileName))
                return string.Format(reservedNameFormat, fileName);

            var invalid = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(fileName.Length);
            foreach (var c in fileName)
            {
                if (Array.IndexOf(invalid, c) >= 0)
                {
                    sb.AppendFormat(reservedCharFormat, (short)c);
                }
                else
                {
                    sb.Append(c);
                }
            }

            var s = sb.ToString();
            if (s.Length >= 255) // a segment is always 255 max even with long file names
            {
                s = s.Substring(0, 254);
            }

            if (s.EqualsIgnoreCase(fileName))
                return fileName;

            return s;
        }

        public static bool PathIsFile(string path)
        {
            var atts = PathGetAttributes(path);
            if (!atts.HasValue)
                return false;

            return !atts.Value.HasFlag(FileAttributes.Directory);
        }

        public static bool PathIsDirectory(string? path)
        {
            if (path == null)
                return false;

            return PathGetAttributes(path).GetValueOrDefault().HasFlag(FileAttributes.Directory);
        }

        public static bool PathExists(string? path)
        {
            if (path == null)
                return false;

            return PathGetAttributes(path).HasValue;
        }

        public static void PathDelete(string path, bool recursive = false, bool throwOnError = true)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (PathIsDirectory(path))
            {
                DirectoryDelete(path, recursive, throwOnError);
            }
            else
            {
                FileDelete(path, true, throwOnError);
            }
        }

        public static bool PathHasInvalidChars(string? path)
        {
            if (path == null)
                return true;

            for (var i = 0; i < path.Length; i++)
            {
                var c = path[i];
                if (c == 0x22 ||
                    c == 0x3C ||
                    c == 0x3E ||
                    c == 0x7C ||
                    c < 0x20)
                    return true;
            }
            return false;
        }

        public static bool PathIsValidFileName(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return false;

            if (Array.IndexOf(_reservedFileNames, fileName.ToLowerInvariant()) >= 0)
                return false;

            return !IsAllDots(fileName);
        }

        public static bool IsPathRooted(string? path)
        {
            if (path == null)
                return false;

            var length = path.Length;
            if (length < 1 || path[0] != Path.DirectorySeparatorChar && path[0] != Path.AltDirectorySeparatorChar)
                return length >= 2 && path[1] == Path.VolumeSeparatorChar;

            return true;
        }

        public static string? PathRemoveEndSlash(string? path)
        {
            if (path == null)
                return null;

            if (!path.EndsWith(@"\"))
                return path;

            return path.Substring(0, path.Length - 1);
        }

        public static string? PathRemoveStartSlash(string? path)
        {
            if (path == null)
                return null;

            if (!path.StartsWith(@"\"))
                return path;

            return path.Substring(1);
        }

        private static bool IsPathRootedNoCheck(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            // ::{clsid} => true for Path.IsPathRooted...
            if (path.StartsWith("::{"))
                return false;

            return path[0] == Path.DirectorySeparatorChar || path.Length >= 2 && path[1] == Path.VolumeSeparatorChar;
        }

        public static string? PathCombineNoCheck(params string[] paths) => PathCombineNoCheck(Path.DirectorySeparatorChar, paths);
        public static string? PathCombineNoCheck(char separator, params string[] paths)
        {
            if (paths == null)
                return null;

            if (paths.Length == 0)
                return null;

            if (paths.Length == 1)
                return paths[0];

            var sb = new StringBuilder();
            for (var i = 0; i < paths.Length; i++)
            {
                if (string.IsNullOrEmpty(paths[i]))
                    continue;

                if (IsPathRootedNoCheck(paths[i]))
                {
                    sb = new StringBuilder(paths[i]);
                    continue;
                }

                if (sb.Length > 0)
                {
                    if (sb[sb.Length - 1] == separator)
                    {
                        if (paths[i][0] == separator)
                        {
                            sb.Append(paths[i].Substring(1));
                        }
                        else
                        {
                            sb.Append(paths[i]);
                        }
                    }
                    else
                    {
                        if (paths[i][0] == separator)
                        {
                            sb.Append(paths[i]);
                        }
                        else
                        {
                            sb.Append(separator);
                            sb.Append(paths[i]);
                        }
                    }
                }
                else
                {
                    sb.Append(paths[i]);
                }
            }
            return sb.ToString();
        }

        public static long CopyTo(this Stream input, Stream output, long count = long.MaxValue, int bufferSize = 0x14000)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (count <= 0)
                throw new ArgumentException(null, nameof(count));

            if (bufferSize <= 0)
                throw new ArgumentException(null, nameof(bufferSize));

            if (count < bufferSize)
            {
                bufferSize = (int)count;
            }

            var bytes = new byte[bufferSize];
            var total = 0;
            do
            {
                var max = (int)Math.Min(count - total, bytes.Length);
                var read = input.Read(bytes, 0, max);
                if (read == 0)
                    break;

                output.Write(bytes, 0, read);
                total += read;
                if (total == count)
                    break;
            }
            while (true);
            return total;
        }

        public static Task<long> CopyToAsync(this Stream input, Stream output, long count = long.MaxValue, int bufferSize = 0x14000) => input.CopyToAsync(output, CancellationToken.None, count, bufferSize);
        public static async Task<long> CopyToAsync(this Stream input, Stream output, CancellationToken cancellationToken, long count = long.MaxValue, int bufferSize = 0x14000)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (count <= 0)
                throw new ArgumentException(null, nameof(count));

            if (bufferSize <= 0)
                throw new ArgumentException(null, nameof(bufferSize));

            if (count < bufferSize)
            {
                bufferSize = (int)count;
            }

            var bytes = new byte[bufferSize];
            var total = 0;
            do
            {
                var max = (int)Math.Min(count - total, bytes.Length);
                var read = await input.ReadAsync(bytes.AsMemory(0, max), cancellationToken).ConfigureAwait(false);
                if (read == 0)
                    break;

                await output.WriteAsync(bytes.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
                total += read;
                if (total == count)
                    break;
            }
            while (true);
            return total;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace HwndExplorer.Utilities
{
    public static class Conversions
    {
        // see: https://stackoverflow.com/questions/2483023/how-to-test-if-a-type-is-anonymous
        public static bool IsAnonymousType(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && type.Name.StartsWith("<>")
                && type.Attributes.HasFlag(TypeAttributes.NotPublic);
        }

        public static long ToFileTime(this DateTimeOffset dt)
        {
            var ticks = dt.ToUniversalTime().Ticks;
            ticks -= _fileTimeOffset;
            return ticks;
        }

        public static long ToFileTime(this DateTime dt)
        {
            var ticks = dt.Kind != DateTimeKind.Utc ? dt.ToUniversalTime().Ticks : dt.Ticks;
            ticks -= _fileTimeOffset;
            return ticks;
        }

        public static System.Runtime.InteropServices.ComTypes.FILETIME ToFileTime(this long fileTime)
        {
            var ft = new System.Runtime.InteropServices.ComTypes.FILETIME
            {
                dwLowDateTime = (int)(fileTime & uint.MaxValue),
                dwHighDateTime = (int)(fileTime >> 32)
            };
            return ft;
        }

        public static long ToFileTime(this System.Runtime.InteropServices.ComTypes.FILETIME fileTime) => ((long)fileTime.dwHighDateTime << 32) + fileTime.dwLowDateTime;
        public static DateTimeOffset ToDateTimeOffset(this System.Runtime.InteropServices.ComTypes.FILETIME fileTime)
        {
            var ft = ((long)fileTime.dwHighDateTime << 32) + fileTime.dwLowDateTime;
            return DateTimeOffset.FromFileTime(ft);
        }

        public static DateTime ToDateTime(this System.Runtime.InteropServices.ComTypes.FILETIME fileTime)
        {
            var ft = ((long)fileTime.dwHighDateTime << 32) + fileTime.dwLowDateTime;
            return DateTime.FromFileTime(ft);
        }

        public static DateTime ToDateTimeUtc(this System.Runtime.InteropServices.ComTypes.FILETIME fileTime)
        {
            var ft = ((long)fileTime.dwHighDateTime << 32) + fileTime.dwLowDateTime;
            return DateTime.FromFileTimeUtc(ft);
        }

        private const long _ticksPerMillisecond = 10000;
        private const long _ticksPerSecond = _ticksPerMillisecond * 1000;
        private const long _ticksPerMinute = _ticksPerSecond * 60;
        private const long _ticksPerHour = _ticksPerMinute * 60;
        private const long _ticksPerDay = _ticksPerHour * 24;
        private const int _daysPerYear = 365;
        private const int _daysPer4Years = _daysPerYear * 4 + 1;
        private const int _daysPer100Years = _daysPer4Years * 25 - 1;
        private const int _daysPer400Years = _daysPer100Years * 4 + 1;
        private const int _daysTo1601 = _daysPer400Years * 4;
        private const long _fileTimeOffset = _daysTo1601 * _ticksPerDay;

        public static readonly DateTime MinFileTime = DateTime.FromFileTimeUtc(0);
        public static readonly DateTimeOffset MinFileTimeOffset = new(MinFileTime);

        public static long ToPositiveFileTime(this DateTimeOffset dt)
        {
            var ft = ToFileTime(dt);
            return ft < 0 ? 0 : ft;
        }

        public static long ToPositiveFileTime(this DateTime dt)
        {
            var ft = ToFileTime(dt);
            return ft < 0 ? 0 : ft;
        }

        public static bool IsValidFileTime(this DateTimeOffset dt) => ToFileTime(dt) >= 0;
        public static bool IsValidFileTime(this DateTime dt) => ToFileTime(dt) >= 0;
        public static DateTime RemoveMilliseconds(this DateTime dateTime) => new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
        public static DateTimeOffset RemoveMilliseconds(this DateTimeOffset dateTime) => new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Offset);

        public static void AddIfNotNull(IDictionary<string, string?> dictionary, object? value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        {
            if (dictionary == null || parameterName == null)
                return;

            if (value == null || Convert.IsDBNull(value))
                return;

            if (value is not string s)
            {
                s = ChangeType<string>(value)!;
            }
            dictionary[parameterName] = s;
        }

        public static bool AreDefaultValues(params object[] values)
        {
            ArgumentNullException.ThrowIfNull(values);
            if (values.Length == 0)
                throw new ArgumentException(null, nameof(values));

            foreach (var value in values)
            {
                if (IsDefaultValue(value))
                    return true;
            }
            return true;
        }

        public static bool IsDefaultValue(object value)
        {
            if (value == null || Convert.IsDBNull(value))
                return true;

            var type = value.GetType();
            if (!type.IsValueType)
                return false;

            var tc = Type.GetTypeCode(type);
            switch (tc)
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    return true;

                case TypeCode.Boolean:
                    return !(bool)value;

                case TypeCode.Char:
                    return 0 == (char)value;

                case TypeCode.SByte:
                    return 0 == (sbyte)value;

                case TypeCode.Byte:
                    return 0 == (byte)value;

                case TypeCode.Int16:
                    return 0 == (short)value;

                case TypeCode.UInt16:
                    return 0 == (ushort)value;

                case TypeCode.Int32:
                    return 0 == (int)value;

                case TypeCode.UInt32:
                    return 0 == (uint)value;

                case TypeCode.Int64:
                    return 0 == (long)value;

                case TypeCode.UInt64:
                    return 0 == (ulong)value;

                case TypeCode.Single:
                    return 0 == (float)value;

                case TypeCode.Double:
                    return 0 == (double)value;

                case TypeCode.Decimal:
                    return 0 == (decimal)value;

                case TypeCode.DateTime:
                    return DateTime.MinValue == (DateTime)value;

                default:
                    if (value is TimeSpan ts)
                        return ts == TimeSpan.Zero;

                    if (value is Guid guid)
                        return guid == Guid.Empty;

                    if (value is DateTimeOffset dto)
                        return dto == DateTimeOffset.MinValue;

                    return value.Equals(Activator.CreateInstance(type));
            }
        }

        public static Type? GetAsyncEnumerableType(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            if (!type.IsGenericType || type.GenericTypeArguments.Length != 1 || type.GetGenericTypeDefinition() != typeof(IAsyncEnumerable<>))
                return null;

            return type.GetGenericArguments()[0];
        }

        public static Type? GetEnumeratedType(Type collectionType)
        {
            ArgumentNullException.ThrowIfNull(collectionType);

            var etype = GetEnumeratedItemType(collectionType);
            if (etype != null)
                return etype;

            foreach (var type in collectionType.GetInterfaces())
            {
                etype = GetEnumeratedItemType(type);
                if (etype != null)
                    return etype;
            }
            return null;
        }

        private static Type? GetEnumeratedItemType(Type type)
        {
            if (!type.IsGenericType)
                return null;

            if (type.GetGenericArguments().Length != 1)
                return null;

            if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            if (type.GetGenericTypeDefinition() == typeof(ICollection<>))
                return type.GetGenericArguments()[0];

            if (type.GetGenericTypeDefinition() == typeof(IList<>))
                return type.GetGenericArguments()[0];

            if (type.GetGenericTypeDefinition() == typeof(ISet<>))
                return type.GetGenericArguments()[0];

            if (type.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>))
                return type.GetGenericArguments()[0];

            if (type.GetGenericTypeDefinition() == typeof(IReadOnlyList<>))
                return type.GetGenericArguments()[0];

            if (type.GetGenericTypeDefinition() == typeof(IReadOnlySet<>))
                return type.GetGenericArguments()[0];

            if (type.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
                return type.GetGenericArguments()[0];

            return null;
        }

        public static bool IsNullable(this Type type)
        {
            ArgumentNullException.ThrowIfNull(type);
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type? GetNullableTypeArgument(this Type type)
        {
            ArgumentNullException.ThrowIfNull(type);
            if (!type.IsGenericType)
                return null;

            var def = type.GetGenericTypeDefinition();
            if (def != typeof(Nullable<>))
                return null;

            return type.GetGenericArguments()[0];
        }

        public static bool EqualsIgnoreCase(this string? thisString, string? text, bool trim = true)
        {
            if (trim)
            {
                thisString = thisString.Nullify();
                text = text.Nullify();
            }

            if (thisString == null)
                return text == null;

            if (text == null)
                return false;

            if (thisString.Length != text.Length)
                return false;

            return string.Compare(thisString, text, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static string? Nullify(this string? text)
        {
            if (text == null)
                return null;

            if (string.IsNullOrWhiteSpace(text))
                return null;

            var t = text.Trim();
            return t.Length == 0 ? null : t;
        }

        public static string? ToHexa(this byte[] bytes) => bytes.ToHexa(0, (bytes?.Length).GetValueOrDefault());
        public static string? ToHexa(this byte[] bytes, int count) => bytes.ToHexa(0, count);
        public static string? ToHexa(this byte[] bytes, int offset, int count)
        {
            if (bytes == null)
                return null;

            if (offset < 0)
                throw new ArgumentException(null, nameof(offset));

            if (count < 0)
                throw new ArgumentException(null, nameof(count));

            if (offset > bytes.Length)
                throw new ArgumentException(null, nameof(offset));

            count = Math.Min(count, bytes.Length - offset);
            var sb = new StringBuilder(count * 2);
            for (int i = offset; i < offset + count; i++)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", bytes[i]);
            }
            return sb.ToString();
        }

        private static readonly char[] _enumSeparators = new char[] { ',', ';', '+', '|', ' ' };

        public static bool TryParseEnum(Type type, object input, out object? value)
        {
            ArgumentNullException.ThrowIfNull(type);

            if (!type.IsEnum)
                throw new ArgumentException(null, nameof(type));

            if (input == null || Convert.IsDBNull(input))
            {
                value = Activator.CreateInstance(type);
                return false;
            }

            var stringInput = string.Format(CultureInfo.InvariantCulture, "{0}", input);
            stringInput = stringInput.Nullify();
            if (stringInput == null)
            {
                value = Activator.CreateInstance(type);
                return false;
            }

            if (stringInput.StartsWith("0x", StringComparison.OrdinalIgnoreCase) && ulong.TryParse(stringInput.AsSpan(2), NumberStyles.HexNumber, null, out ulong ulx))
            {
                value = ToEnum(ulx.ToString(CultureInfo.InvariantCulture), type);
                return true;
            }

            var names = Enum.GetNames(type);
            if (names.Length == 0)
            {
                value = Activator.CreateInstance(type);
                return false;
            }

            var values = Enum.GetValues(type);
            // some enums like System.CodeDom.MemberAttributes *are* flags but are not declared with Flags...
            if (!type.IsDefined(typeof(FlagsAttribute), true) && stringInput.IndexOfAny(_enumSeparators) < 0)
                return StringToEnum(type, names, values, stringInput, out value);

            // multi value enum
            var tokens = stringInput.Split(_enumSeparators, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
            {
                value = Activator.CreateInstance(type);
                return false;
            }

            ulong ul = 0;
            foreach (var tok in tokens)
            {
                var token = tok.Nullify(); // NOTE: we don't consider empty tokens as errors
                if (token == null)
                    continue;

                if (!StringToEnum(type, names, values, token, out var tokenValue))
                {
                    value = Activator.CreateInstance(type);
                    return false;
                }

                ulong tokenUl;
                switch (Convert.GetTypeCode(tokenValue))
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                        tokenUl = (ulong)Convert.ToInt64(tokenValue, CultureInfo.InvariantCulture);
                        break;

                    default:
                        tokenUl = Convert.ToUInt64(tokenValue, CultureInfo.InvariantCulture);
                        break;
                }

                ul |= tokenUl;
            }
            value = Enum.ToObject(type, ul);
            return true;
        }

        public static object? ToEnum(string text, Type enumType)
        {
            ArgumentNullException.ThrowIfNull(enumType);

            TryParseEnum(enumType, text, out var value);
            return value;
        }

        private static bool StringToEnum(Type type, string[] names, Array values, string input, out object? value)
        {
            for (var i = 0; i < names.Length; i++)
            {
                if (names[i].EqualsIgnoreCase(input))
                {
                    value = values.GetValue(i);
                    return true;
                }
            }

            for (var i = 0; i < values.GetLength(0); i++)
            {
                var valuei = values.GetValue(i)!;
                if (input.Length > 0 && input[0] == '-')
                {
                    var ul = (long)EnumToUInt64(valuei);
                    if (ul.ToString().EqualsIgnoreCase(input))
                    {
                        value = valuei;
                        return true;
                    }
                }
                else
                {
                    var ul = EnumToUInt64(valuei);
                    if (ul.ToString().EqualsIgnoreCase(input))
                    {
                        value = valuei;
                        return true;
                    }
                }
            }

            if (char.IsDigit(input[0]) || input[0] == '-' || input[0] == '+')
            {
                var obj = EnumToObject(type, input);
                if (obj == null)
                {
                    value = Activator.CreateInstance(type);
                    return false;
                }
                value = obj;
                return true;
            }

            value = Activator.CreateInstance(type);
            return false;
        }

        public static object EnumToObject(Type enumType, object value)
        {
            ArgumentNullException.ThrowIfNull(enumType);
            ArgumentNullException.ThrowIfNull(value);

            if (!enumType.IsEnum)
                throw new ArgumentException(null, nameof(enumType));

            var underlyingType = Enum.GetUnderlyingType(enumType);
            if (underlyingType == typeof(long))
                return Enum.ToObject(enumType, ChangeType<long>(value));

            if (underlyingType == typeof(ulong))
                return Enum.ToObject(enumType, ChangeType<ulong>(value));

            if (underlyingType == typeof(int))
                return Enum.ToObject(enumType, ChangeType<int>(value));

            if (underlyingType == typeof(uint))
                return Enum.ToObject(enumType, ChangeType<uint>(value));

            if (underlyingType == typeof(short))
                return Enum.ToObject(enumType, ChangeType<short>(value));

            if (underlyingType == typeof(ushort))
                return Enum.ToObject(enumType, ChangeType<ushort>(value));

            if (underlyingType == typeof(byte))
                return Enum.ToObject(enumType, ChangeType<byte>(value));

            if (underlyingType == typeof(sbyte))
                return Enum.ToObject(enumType, ChangeType<sbyte>(value));

            throw new ArgumentException(null, nameof(enumType));
        }

        public static ulong EnumToUInt64(object value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var typeCode = Convert.GetTypeCode(value);
            switch (typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);

                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return Convert.ToUInt64(value, CultureInfo.InvariantCulture);

                case TypeCode.String:
                default:
                    return ChangeType<ulong>(value, 0, CultureInfo.InvariantCulture);
            }
        }

        public static object? ChangeType(object? input, Type conversionType, object? defaultValue = null, IFormatProvider? provider = null)
        {
            if (!TryChangeType(input, conversionType, provider, out object? value))
                return defaultValue;

            return value;
        }

        public static T? ChangeType<T>(object? input, T? defaultValue = default, IFormatProvider? provider = null)
        {
            if (!TryChangeType(input, provider, out T? value))
                return defaultValue;

            return value;
        }

        public static bool TryChangeType<T>(object? input, out T? value) => TryChangeType(input, null, out value);
        public static bool TryChangeType<T>(object? input, IFormatProvider? provider, out T? value)
        {
            if (!TryChangeType(input, typeof(T), provider, out object? tvalue))
            {
                value = default;
                return false;
            }

            value = (T)tvalue!;
            return true;
        }

        public static bool TryChangeType(object? input, Type conversionType, out object? value) => TryChangeType(input, conversionType, null, out value);
        public static bool TryChangeType(object? input, Type conversionType, IFormatProvider? provider, out object? value)
        {
            ArgumentNullException.ThrowIfNull(conversionType);

            if (conversionType == typeof(object))
            {
                value = input;
                return true;
            }

            value = conversionType.IsValueType ? Activator.CreateInstance(conversionType) : null;
            if (input == null || Convert.IsDBNull(input))
                return !conversionType.IsValueType;

            var inputType = input.GetType();
            if (conversionType.IsAssignableFrom(inputType))
            {
                value = input;
                return true;
            }

            if (input is JsonElement element)
            {
                if (element.TryConvertToObject(out var jvalue))
                    return TryChangeType(jvalue, conversionType, provider, out value);

                return false;
            }

            if (conversionType.IsEnum)
                return TryParseEnum(conversionType, input, out value);

            if (conversionType == typeof(Guid))
            {
                var svalue = string.Format(provider, "{0}", input).Nullify();
                if (svalue != null && Guid.TryParse(svalue, out Guid guid))
                {
                    value = guid;
                    return true;
                }
                return false;
            }

            if (conversionType == typeof(Type))
            {
                var typeName = string.Format(provider, "{0}", input).Nullify();
                if (typeName == null)
                    return false;

                var type = Type.GetType(typeName, false);
                if (type == null)
                    return false;

                value = type;
                return true;
            }

            if (conversionType == typeof(nint))
            {
                if (nint.Size == 8 && TryChangeType(input, provider, out long l))
                {
                    value = new nint(l);
                    return true;
                }
                return false;
            }

            if (conversionType == typeof(int))
            {
                if (inputType == typeof(uint))
                {
                    value = unchecked((int)(uint)input);
                    return true;
                }

                if (inputType == typeof(ulong))
                {
                    value = unchecked((int)(ulong)input);
                    return true;
                }

                if (inputType == typeof(ushort))
                {
                    value = unchecked((int)(ushort)input);
                    return true;
                }

                if (inputType == typeof(byte))
                {
                    value = unchecked((int)(byte)input);
                    return true;
                }
            }

            if (conversionType == typeof(long))
            {
                if (inputType == typeof(uint))
                {
                    value = unchecked((long)(uint)input);
                    return true;
                }

                if (inputType == typeof(ulong))
                {
                    value = unchecked((long)(ulong)input);
                    return true;
                }

                if (inputType == typeof(ushort))
                {
                    value = unchecked((long)(ushort)input);
                    return true;
                }

                if (inputType == typeof(byte))
                {
                    value = unchecked((long)(byte)input);
                    return true;
                }
            }

            if (conversionType == typeof(short))
            {
                if (inputType == typeof(uint))
                {
                    value = unchecked((short)(uint)input);
                    return true;
                }

                if (inputType == typeof(ulong))
                {
                    value = unchecked((short)(ulong)input);
                    return true;
                }

                if (inputType == typeof(ushort))
                {
                    value = unchecked((short)(ushort)input);
                    return true;
                }

                if (inputType == typeof(byte))
                {
                    value = unchecked((short)(byte)input);
                    return true;
                }
            }

            if (conversionType == typeof(sbyte))
            {
                if (inputType == typeof(uint))
                {
                    value = unchecked((sbyte)(uint)input);
                    return true;
                }

                if (inputType == typeof(ulong))
                {
                    value = unchecked((sbyte)(ulong)input);
                    return true;
                }

                if (inputType == typeof(ushort))
                {
                    value = unchecked((sbyte)(ushort)input);
                    return true;
                }

                if (inputType == typeof(byte))
                {
                    value = unchecked((sbyte)(byte)input);
                    return true;
                }
            }

            if (conversionType == typeof(uint))
            {
                if (inputType == typeof(int))
                {
                    value = unchecked((uint)(int)input);
                    return true;
                }

                if (inputType == typeof(long))
                {
                    value = unchecked((uint)(long)input);
                    return true;
                }

                if (inputType == typeof(short))
                {
                    value = unchecked((uint)(short)input);
                    return true;
                }

                if (inputType == typeof(sbyte))
                {
                    value = unchecked((uint)(sbyte)input);
                    return true;
                }
            }

            if (conversionType == typeof(ulong))
            {
                if (inputType == typeof(int))
                {
                    value = unchecked((ulong)(int)input);
                    return true;
                }

                if (inputType == typeof(long))
                {
                    value = unchecked((ulong)(long)input);
                    return true;
                }

                if (inputType == typeof(short))
                {
                    value = unchecked((ulong)(short)input);
                    return true;
                }

                if (inputType == typeof(sbyte))
                {
                    value = unchecked((ulong)(sbyte)input);
                    return true;
                }
            }

            if (conversionType == typeof(ushort))
            {
                if (inputType == typeof(int))
                {
                    value = unchecked((ushort)(int)input);
                    return true;
                }

                if (inputType == typeof(long))
                {
                    value = unchecked((ushort)(long)input);
                    return true;
                }

                if (inputType == typeof(short))
                {
                    value = unchecked((ushort)(short)input);
                    return true;
                }

                if (inputType == typeof(sbyte))
                {
                    value = unchecked((ushort)(sbyte)input);
                    return true;
                }
            }

            if (conversionType == typeof(byte))
            {
                if (inputType == typeof(int))
                {
                    value = unchecked((byte)(int)input);
                    return true;
                }

                if (inputType == typeof(long))
                {
                    value = unchecked((byte)(long)input);
                    return true;
                }

                if (inputType == typeof(short))
                {
                    value = unchecked((byte)(short)input);
                    return true;
                }

                if (inputType == typeof(sbyte))
                {
                    value = unchecked((byte)(sbyte)input);
                    return true;
                }
            }

            if (conversionType == typeof(bool))
            {
                if (inputType == typeof(string))
                {
                    var sinput = (string)input;
                    if (string.IsNullOrWhiteSpace(sinput) || sinput.EqualsIgnoreCase("false"))
                    {
                        value = false;
                        return true;
                    }

                    if (sinput.EqualsIgnoreCase("true"))
                    {
                        value = true;
                        return true;
                    }

                    if (TryChangeType<long>(input, out var l))
                    {
                        value = l != 0;
                        return true;
                    }
                }
            }

            if (conversionType == typeof(DateTime))
            {
                if (input is DateTimeOffset dto)
                {
                    value = dto.DateTime;
                    return true;
                }

                if (input is double dbl)
                {
                    try
                    {
                        value = DateTime.FromOADate(dbl);
                        return true;
                    }
                    catch
                    {
                        value = DateTime.MinValue;
                        return false;
                    }
                }
            }

            if (conversionType == typeof(DateTimeOffset))
            {
                if (input is DateTime dta)
                {
                    value = new DateTimeOffset(dta);
                    return true;
                }

                if (input is double dbl2)
                {
                    try
                    {
                        value = new DateTimeOffset(DateTime.FromOADate(dbl2));
                        return true;
                    }
                    catch
                    {
                        value = DateTimeOffset.MinValue;
                        return false;
                    }
                }
            }

            if (conversionType == typeof(TimeSpan))
            {
                if (TryChangeType<long>(input, out var l))
                {
                    try
                    {
                        value = TimeSpan.FromTicks(l);
                        return true;
                    }
                    catch
                    {
                        // do nothing
                    }
                }
                else if (TryChangeType<string>(input, out var str) && !string.IsNullOrEmpty(str) && TimeSpan.TryParse(str, provider, out var ts))
                {
                    value = ts;
                    return true;
                }
                value = TimeSpan.Zero;
                return false;
            }

            var nullable = conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>);
            if (nullable)
            {
                if (input == null || Convert.IsDBNull(input) || string.Empty.Equals(input))
                {
                    value = null;
                    return true;
                }

                var type = conversionType.GetGenericArguments()[0];
                if (TryChangeType(input, type, provider, out var vtValue))
                {
                    var nullableType = typeof(Nullable<>).MakeGenericType(type);
                    value = Activator.CreateInstance(nullableType, vtValue);
                    return true;
                }

                value = null;
                return false;
            }

            if (input is IConvertible convertible)
            {
                try
                {
                    value = convertible.ToType(conversionType, provider);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            if (conversionType == typeof(byte[]))
            {
                if (input is int i)
                {
                    value = BitConverter.GetBytes(i);
                    return true;
                }

                if (input is long l)
                {
                    value = BitConverter.GetBytes(l);
                    return true;
                }

                if (input is short s)
                {
                    value = BitConverter.GetBytes(s);
                    return true;
                }

                if (input is uint ui)
                {
                    value = BitConverter.GetBytes(ui);
                    return true;
                }

                if (input is ulong ul)
                {
                    value = BitConverter.GetBytes(ul);
                    return true;
                }

                if (input is ushort us)
                {
                    value = BitConverter.GetBytes(us);
                    return true;
                }

                if (input is bool b)
                {
                    value = BitConverter.GetBytes(b);
                    return true;
                }

                if (input is string str)
                {
                    try
                    {
                        value = Convert.FromBase64String(str);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                if (input is Guid g)
                {
                    value = g.ToByteArray();
                    return true;
                }

                if (input is byte bb)
                {
                    value = new byte[] { bb };
                    return true;
                }

                if (input is sbyte sb)
                {
                    value = new byte[] { (byte)sb };
                    return true;
                }

                if (input is char c)
                {
                    value = BitConverter.GetBytes(c);
                    return true;
                }

                if (input is double dbl)
                {
                    value = BitConverter.GetBytes(dbl);
                    return true;
                }

                if (input is float flt)
                {
                    value = BitConverter.GetBytes(flt);
                    return true;
                }

                return false;
            }

            if (conversionType == typeof(string))
            {
                if (input is byte[] bytes)
                {
                    value = bytes.ToHexa();
                    return true;
                }

                value = string.Format(provider, "{0}", input);
                return true;
            }

            var inputEnumeratedType = GetEnumeratedType(inputType);
            var conversionEnumeratedType = GetEnumeratedType(conversionType);
            if (inputEnumeratedType != null && conversionEnumeratedType != null)
            {
                var containerType = typeof(List<>).MakeGenericType(conversionEnumeratedType);
                if (!conversionType.IsAssignableFrom(containerType))
                    return false;

                var containerAdd = containerType.GetMethod("Add");
                if (containerAdd == null)
                    return false;

                var convertedValues = Activator.CreateInstance(containerType);
                var defaultValue = conversionEnumeratedType.IsValueType ? Activator.CreateInstance(conversionEnumeratedType) : null;
                var enumerableInput = (IEnumerable<object>)input;
                foreach (var inputItem in enumerableInput)
                {
                    var convertedValue = ChangeType(inputItem, conversionEnumeratedType, defaultValue);
                    containerAdd!.Invoke(convertedValues, new[] { convertedValue });
                }
                value = convertedValues;
                return true;
            }
            return false;
        }

        public static T? ConvertToObject<T>(this JsonElement element, T? defaultValue = default)
        {
            if (!element.TryConvertToObject<T>(out var value))
                return defaultValue;

            return value;
        }

        public static object? ConvertToObject(this JsonElement element, object? defaultValue)
        {
            if (!element.TryConvertToObject(out var value))
                return defaultValue;

            return value;
        }

        public static bool TryConvertToObject<T>(this JsonElement element, out T? value)
        {
            if (!element.TryConvertToObject(out var cvalue))
            {
                value = default;
                return false;
            }

            return TryChangeType(cvalue, out value);
        }

        public static bool TryConvertToObject(this JsonElement element, out object? value)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                    value = null;
                    return true;

                case JsonValueKind.Object:
                    var dic = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                    foreach (var child in element.EnumerateObject())
                    {
                        if (!child.Value.TryConvertToObject(out var childValue))
                        {
                            value = null;
                            return false;
                        }

                        dic[child.Name] = childValue;
                    }

                    // empty dic => null
                    if (dic.Count == 0)
                    {
                        value = null;
                        return true;
                    }

                    value = dic;
                    return true;

                case JsonValueKind.Array:
                    var objects = new object?[element.GetArrayLength()];
                    var i = 0;
                    foreach (var child in element.EnumerateArray())
                    {
                        if (!child.TryConvertToObject(out var childValue))
                        {
                            value = null;
                            return false;
                        }

                        objects[i++] = childValue;
                    }

                    value = objects;
                    return true;

                case JsonValueKind.String:
                    var str = element.ToString();
                    if (DateTime.TryParseExact(str, new string[] { "o", "r", "s" }, null, DateTimeStyles.None, out var dt))
                    {
                        value = dt;
                        return true;
                    }

                    value = str;
                    return true;

                case JsonValueKind.Number:
                    if (element.TryGetInt32(out var i32))
                    {
                        value = i32;
                        return true;
                    }

                    if (element.TryGetInt32(out var i64))
                    {
                        value = i64;
                        return true;
                    }

                    if (element.TryGetDecimal(out var dec))
                    {
                        value = dec;
                        return true;
                    }

                    if (element.TryGetDouble(out var dbl))
                    {
                        value = dbl;
                        return true;
                    }
                    break;

                case JsonValueKind.True:
                    value = true;
                    return true;

                case JsonValueKind.False:
                    value = false;
                    return true;
            }

            value = null;
            return false;
        }
    }
}

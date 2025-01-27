﻿using System;
using System.Text;

namespace ExifLibrary
{
    /// <summary>
    /// Converts between exif data types and array of bytes.
    /// </summary>
    public class ExifBitConverter : BitConverterEx
    {
        #region Constructors
        /// <summary>
        /// Intializes a new instance of the <see cref="ExifBitConverter"/> class.
        /// </summary>
        /// <param name="from">The byte order to convert from.</param>
        /// <param name="to">The byte order to convert to.</param>
        public ExifBitConverter(ByteOrder from, ByteOrder to)
            : base(from, to)
        {
            ;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Returns an ASCII string converted from the given byte array.
        /// </summary>
        public static string ToAscii(byte[] data, bool endatfirstnull, Encoding encoding)
        {
            int len = data.Length;
            if (endatfirstnull)
            {
                len = Array.IndexOf(data, (byte)0);
                if (len == -1) len = data.Length;
            }
            return encoding.GetString(data, 0, len);
        }

        /// <summary>
        /// Returns an ASCII string converted from the given byte array.
        /// </summary>
        public static string ToAscii(byte[] data, Encoding encoding)
        {
            return ToAscii(data, true, encoding);
        }

        /// <summary>
        /// Returns a string converted from the given byte array.
        /// from the numeric value of each byte.
        /// </summary>
        public static string ToString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
                sb.Append(b);
            return sb.ToString();
        }

        /// <summary>
        /// Returns a DateTime object converted from the given byte array.
        /// </summary>
        public static DateTime ToDateTime(byte[] data, bool hastime)
        {
            string str = ToAscii(data, Encoding.ASCII);
            string[] parts = str.Split(new char[] { ':', ' ' });
            try
            {
                if (hastime && parts.Length >= 6)
                {
                    // yyyy:MM:dd HH:mm:ss
                    // This is the expected format though some cameras
                    // can use single digits. See Issue 21.
                    if (!int.TryParse(parts[0], out int year)) year = 1;
                    if (!int.TryParse(parts[1], out int month)) month = 1;
                    if (!int.TryParse(parts[2], out int day)) day = 1;
                    if (!int.TryParse(parts[3], out int hour)) hour = 0;
                    if (!int.TryParse(parts[4], out int minute)) minute = 0;
                    if (!int.TryParse(parts[5], out int second)) second = 0;
                    return new DateTime(year, month, day, hour, minute, second);
                }
                else if (!hastime && parts.Length >= 3)
                {
                    // yyyy:MM:dd
                    if (!int.TryParse(parts[0], out int year)) year = 1;
                    if (!int.TryParse(parts[1], out int month)) month = 1;
                    if (!int.TryParse(parts[2], out int day)) day = 1;
                    return new DateTime(year, month, day);
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                return DateTime.MinValue;
            }
            catch (ArgumentException)
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Returns a DateTime object converted from the given byte array.
        /// </summary>
        public static DateTime ToDateTime(byte[] data)
        {
            return ToDateTime(data, true);
        }

        /// <summary>
        /// Returns an unsigned rational number converted from the first 
        /// eight bytes of the given byte array. The first four bytes are
        /// assumed to be the numerator and the next four bytes are the
        /// denumerator.
        /// Numbers are converted from the given byte-order to platform byte-order.
        /// </summary>
        public static MathEx.UFraction32 ToURational(byte[] data, ByteOrder frombyteorder)
        {
            byte[] num = new byte[4];
            byte[] den = new byte[4];
            Array.Copy(data, 0, num, 0, 4);
            Array.Copy(data, 4, den, 0, 4);
            return new MathEx.UFraction32(ToUInt32(num, 0, frombyteorder, BitConverterEx.SystemByteOrder), ToUInt32(den, 0, frombyteorder, BitConverterEx.SystemByteOrder));
        }

        /// <summary>
        /// Returns a signed rational number converted from the first 
        /// eight bytes of the given byte array. The first four bytes are
        /// assumed to be the numerator and the next four bytes are the
        /// denumerator.
        /// Numbers are converted from the given byte-order to platform byte-order.
        /// </summary>
        public static MathEx.Fraction32 ToSRational(byte[] data, ByteOrder frombyteorder)
        {
            byte[] num = new byte[4];
            byte[] den = new byte[4];
            Array.Copy(data, 0, num, 0, 4);
            Array.Copy(data, 4, den, 0, 4);
            return new MathEx.Fraction32(ToInt32(num, 0, frombyteorder, BitConverterEx.SystemByteOrder), ToInt32(den, 0, frombyteorder, BitConverterEx.SystemByteOrder));
        }

        /// <summary>
        /// Returns an array of 16-bit unsigned integers converted from 
        /// the given byte array.
        /// Numbers are converted from the given byte-order to platform byte-order.
        /// </summary>
        public static ushort[] ToUShortArray(byte[] data, int count, ByteOrder frombyteorder)
        {
            ushort[] numbers = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                byte[] num = new byte[2];
                Array.Copy(data, i * 2, num, 0, 2);
                numbers[i] = ToUInt16(num, 0, frombyteorder, BitConverterEx.SystemByteOrder);
            }
            return numbers;
        }

        /// <summary>
        /// Returns an array of 32-bit unsigned integers converted from 
        /// the given byte array.
        /// Numbers are converted from the given byte-order to platform byte-order.
        /// </summary>
        public static uint[] ToUIntArray(byte[] data, int count, ByteOrder frombyteorder)
        {
            uint[] numbers = new uint[count];
            for (int i = 0; i < count; i++)
            {
                byte[] num = new byte[4];
                Array.Copy(data, i * 4, num, 0, 4);
                numbers[i] = ToUInt32(num, 0, frombyteorder, BitConverterEx.SystemByteOrder);
            }
            return numbers;
        }

        /// <summary>
        /// Returns an array of 32-bit signed integers converted from 
        /// the given byte array.
        /// Numbers are converted from the given byte-order to platform byte-order.
        /// </summary>
        public static int[] ToSIntArray(byte[] data, int count, ByteOrder byteorder)
        {
            int[] numbers = new int[count];
            for (int i = 0; i < count; i++)
            {
                byte[] num = new byte[4];
                Array.Copy(data, i * 4, num, 0, 4);
                numbers[i] = ToInt32(num, 0, byteorder, BitConverterEx.SystemByteOrder);
            }
            return numbers;
        }

        /// <summary>
        /// Returns an array of unsigned rational numbers converted from 
        /// the given byte array.
        /// Numbers are converted from the given byte-order to platform byte-order.
        /// </summary>
        public static MathEx.UFraction32[] ToURationalArray(byte[] data, int count, ByteOrder frombyteorder)
        {
            MathEx.UFraction32[] numbers = new MathEx.UFraction32[count];
            for (int i = 0; i < count; i++)
            {
                byte[] num = new byte[4];
                byte[] den = new byte[4];
                Array.Copy(data, i * 8, num, 0, 4);
                Array.Copy(data, i * 8 + 4, den, 0, 4);
                numbers[i].Set(ToUInt32(num, 0, frombyteorder, BitConverterEx.SystemByteOrder), ToUInt32(den, 0, frombyteorder, BitConverterEx.SystemByteOrder));
            }
            return numbers;
        }

        /// <summary>
        /// Returns an array of signed rational numbers converted from 
        /// the given byte array.
        /// Numbers are converted from the given byte-order to platform byte-order.
        /// </summary>
        public static MathEx.Fraction32[] ToSRationalArray(byte[] data, int count, ByteOrder frombyteorder)
        {
            MathEx.Fraction32[] numbers = new MathEx.Fraction32[count];
            for (int i = 0; i < count; i++)
            {
                byte[] num = new byte[4];
                byte[] den = new byte[4];
                Array.Copy(data, i * 8, num, 0, 4);
                Array.Copy(data, i * 8 + 4, den, 0, 4);
                numbers[i].Set(ToInt32(num, 0, frombyteorder, BitConverterEx.SystemByteOrder), ToInt32(den, 0, frombyteorder, BitConverterEx.SystemByteOrder));
            }
            return numbers;
        }


        /// <summary>
        /// Returns an array of 16-bit signed integers converted from 
        /// the given byte array.
        /// Numbers are converted from the given byte-order to platform byte-order.
        /// </summary>
        public static short[] ToSShortArray(byte[] data, int count, ByteOrder frombyteorder)
        {
            short[] numbers = new short[count];
            for (int i = 0; i < count; i++)
            {
                byte[] num = new byte[2];
                Array.Copy(data, i * 2, num, 0, 2);
                numbers[i] = ToInt16(num, 0, frombyteorder, BitConverterEx.SystemByteOrder);
            }
            return numbers;
        }

        /// <summary>
        /// Returns an array of 32-bit floating numbers converted from 
        /// the given byte array.
        /// Numbers are converted from the given byte-order to platform byte-order.
        /// </summary>
        public static float[] ToSingleArray(byte[] data, int count, ByteOrder frombyteorder)
        {
            float[] numbers = new float[count];
            for (int i = 0; i < count; i++)
            {
                byte[] num = new byte[4];
                Array.Copy(data, i * 4, num, 0, 4);
                numbers[i] = ToSingle(num, 0, frombyteorder, BitConverterEx.SystemByteOrder);
            }
            return numbers;
        }

        /// <summary>
        /// Returns an array of 64-bit floating numbers converted from 
        /// the given byte array.
        /// Numbers are converted from the given byte-order to platform byte-order.
        /// </summary>
        public static double[] ToDoubleArray(byte[] data, int count, ByteOrder frombyteorder)
        {
            double[] numbers = new double[count];
            for (int i = 0; i < count; i++)
            {
                byte[] num = new byte[8];
                Array.Copy(data, i * 8, num, 0, 8);
                numbers[i] = ToDouble(num, 0, frombyteorder, BitConverterEx.SystemByteOrder);
            }
            return numbers;
        }

        /// <summary>
        /// Converts the given ascii string to an array of bytes optionally adding a null terminator.
        /// </summary>
        public static byte[] GetBytes(string value, bool addnull, Encoding encoding)
        {
            if (addnull) value += '\0';
            return encoding.GetBytes(value);
        }

        /// <summary>
        /// Converts the given ascii string to an array of bytes without adding a null terminator.
        /// </summary>
        public static byte[] GetBytes(string value, Encoding encoding)
        {
            return GetBytes(value, false, encoding);
        }

        /// <summary>
        /// Converts the given datetime to an array of bytes with a null terminator.
        /// </summary>
        public static byte[] GetBytes(DateTime value, bool hastime)
        {
            string str = "";
            if (hastime)
                str = value.ToString("yyyy:MM:dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            else
                str = value.ToString("yyyy:MM:dd", System.Globalization.CultureInfo.InvariantCulture);
            return GetBytes(str, true, Encoding.ASCII);
        }

        /// <summary>
        /// Converts the given unsigned rational number to an array of bytes.
        /// Numbers are converted from the platform byte-order to the given byte-order.
        /// </summary>
        public static byte[] GetBytes(MathEx.UFraction32 value, ByteOrder tobyteorder)
        {
            byte[] num = GetBytes(value.Numerator, BitConverterEx.SystemByteOrder, tobyteorder);
            byte[] den = GetBytes(value.Denominator, BitConverterEx.SystemByteOrder, tobyteorder);
            byte[] data = new byte[8];
            Array.Copy(num, 0, data, 0, 4);
            Array.Copy(den, 0, data, 4, 4);
            return data;
        }

        /// <summary>
        /// Converts the given signed rational number to an array of bytes.
        /// Numbers are converted from the platform byte-order to the given byte-order.
        /// </summary>
        public static byte[] GetBytes(MathEx.Fraction32 value, ByteOrder tobyteorder)
        {
            byte[] num = GetBytes(value.Numerator, BitConverterEx.SystemByteOrder, tobyteorder);
            byte[] den = GetBytes(value.Denominator, BitConverterEx.SystemByteOrder, tobyteorder);
            byte[] data = new byte[8];
            Array.Copy(num, 0, data, 0, 4);
            Array.Copy(den, 0, data, 4, 4);
            return data;
        }

        /// <summary>
        /// Converts the given array of 16-bit unsigned integers to an array of bytes.
        /// Numbers are converted from the platform byte-order to the given byte-order.
        /// </summary>
        public static byte[] GetBytes(ushort[] value, ByteOrder tobyteorder)
        {
            byte[] data = new byte[2 * value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] num = GetBytes(value[i], BitConverterEx.SystemByteOrder, tobyteorder);
                Array.Copy(num, 0, data, i * 2, 2);
            }
            return data;
        }

        /// <summary>
        /// Converts the given array of 32-bit unsigned integers to an array of bytes.
        /// Numbers are converted from the platform byte-order to the given byte-order.
        /// </summary>
        public static byte[] GetBytes(uint[] value, ByteOrder tobyteorder)
        {
            byte[] data = new byte[4 * value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] num = GetBytes(value[i], BitConverterEx.SystemByteOrder, tobyteorder);
                Array.Copy(num, 0, data, i * 4, 4);
            }
            return data;
        }

        /// <summary>
        /// Converts the given array of 32-bit signed integers to an array of bytes.
        /// Numbers are converted from the platform byte-order to the given byte-order.
        /// </summary>
        public static byte[] GetBytes(int[] value, ByteOrder tobyteorder)
        {
            byte[] data = new byte[4 * value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] num = GetBytes(value[i], BitConverterEx.SystemByteOrder, tobyteorder);
                Array.Copy(num, 0, data, i * 4, 4);
            }
            return data;
        }

        /// <summary>
        /// Converts the given array of unsigned rationals to an array of bytes.
        /// Numbers are converted from the platform byte-order to the given byte-order.
        /// </summary>
        public static byte[] GetBytes(MathEx.UFraction32[] value, ByteOrder tobyteorder)
        {
            byte[] data = new byte[8 * value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] num = GetBytes(value[i].Numerator, BitConverterEx.SystemByteOrder, tobyteorder);
                byte[] den = GetBytes(value[i].Denominator, BitConverterEx.SystemByteOrder, tobyteorder);
                Array.Copy(num, 0, data, i * 8, 4);
                Array.Copy(den, 0, data, i * 8 + 4, 4);
            }
            return data;
        }

        /// <summary>
        /// Converts the given array of signed rationals to an array of bytes.
        /// Numbers are converted from the platform byte-order to the given byte-order.
        /// </summary>
        public static byte[] GetBytes(MathEx.Fraction32[] value, ByteOrder tobyteorder)
        {
            byte[] data = new byte[8 * value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] num = GetBytes(value[i].Numerator, BitConverterEx.SystemByteOrder, tobyteorder);
                byte[] den = GetBytes(value[i].Denominator, BitConverterEx.SystemByteOrder, tobyteorder);
                Array.Copy(num, 0, data, i * 8, 4);
                Array.Copy(den, 0, data, i * 8 + 4, 4);
            }
            return data;
        }

        /// <summary>
        /// Converts the given array of 16-bit signed integers to an array of bytes.
        /// Numbers are converted from the platform byte-order to the given byte-order.
        /// </summary>
        public static byte[] GetBytes(short[] value, ByteOrder tobyteorder)
        {
            byte[] data = new byte[2 * value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] num = GetBytes(value[i], BitConverterEx.SystemByteOrder, tobyteorder);
                Array.Copy(num, 0, data, i * 2, 2);
            }
            return data;
        }

        /// <summary>
        /// Converts the given array of 32-bit floating numbers to an array of bytes.
        /// Numbers are converted from the platform byte-order to the given byte-order.
        /// </summary>
        public static byte[] GetBytes(float[] value, ByteOrder tobyteorder)
        {
            byte[] data = new byte[4 * value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] num = GetBytes(value[i], BitConverterEx.SystemByteOrder, tobyteorder);
                Array.Copy(num, 0, data, i * 4, 4);
            }
            return data;
        }

        /// <summary>
        /// Converts the given array of 64-bit floating numbers to an array of bytes.
        /// Numbers are converted from the platform byte-order to the given byte-order.
        /// </summary>
        public static byte[] GetBytes(double[] value, ByteOrder tobyteorder)
        {
            byte[] data = new byte[8 * value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] num = GetBytes(value[i], BitConverterEx.SystemByteOrder, tobyteorder);
                Array.Copy(num, 0, data, i * 8, 8);
            }
            return data;
        }
        #endregion
    }
}
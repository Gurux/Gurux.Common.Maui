//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

namespace Gurux.Common
{
    /// <summary>
    /// Common Gurux helpers.
    /// </summary>
    public static class GXCommon
    {
        /// <summary>
        /// Convert byte array to hex string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        [DebuggerStepThrough]
        public static string ToHex(byte[] bytes)
        {
            return ToHex(bytes, true, 0, bytes == null ? 0 : bytes.Length);
        }

        /// <summary>
        /// Convert byte array to hex string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <param name="addSpace">Is space added between bytes.</param>
        [DebuggerStepThrough]
        public static string ToHex(byte[] bytes, bool addSpace)
        {
            return ToHex(bytes, addSpace, 0, bytes == null ? 0 : bytes.Length);
        }

        /// <summary>
        /// Convert byte array to hex string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <param name="addSpace">Is space added between bytes.</param>
        /// <param name="index">Byte index.</param>
        /// <param name="count">Byte count.</param>
        /// <returns>Byte array as hex string.</returns>
        [DebuggerStepThrough]
        public static string ToHex(byte[] bytes, bool addSpace, int index, int count)
        {
            if (bytes == null || bytes.Length == 0 || count == 0)
            {
                return string.Empty;
            }
            char[] str = new char[count * (addSpace ? 3 : 2)];
            int tmp;
            int len = 0;
            for (int pos = 0; pos != count; ++pos)
            {
                tmp = (bytes[index + pos] >> 4);
                str[len] = (char)(tmp > 9 ? tmp + 0x37 : tmp + 0x30);
                ++len;
                tmp = (bytes[index + pos] & 0x0F);
                str[len] = (char)(tmp > 9 ? tmp + 0x37 : tmp + 0x30);
                ++len;
                if (addSpace)
                {
                    str[len] = ' ';
                    ++len;
                }
            }
            if (addSpace)
            {
                --len;
            }
            return new string(str, 0, len);
        }

        /// <summary>
        /// Convert char hex value to byte value.
        /// </summary>
        /// <param name="c">Character to convert hex.</param>
        /// <returns>Byte value of hex char value.</returns>
        private static byte GetValue(byte c)
        {
            byte value = 0xFF;
            // If number
            if (c > 0x2F && c < 0x3A)
            {
                value = (byte)(c - '0');
            }
            else if (c > 0x40 && c < 'G')
            {
                // If upper case.
                value = (byte)(c - 'A' + 10);
            }
            else if (c > 0x60 && c < 'g')
            {
                // If lower case.
                value = (byte)(c - 'a' + 10);
            }
            return value;
        }

        private static bool IsHex(byte c)
        {
            return GetValue(c) != 0xFF;
        }

        /// <summary>
        /// Convert string to byte array.
        /// </summary>
        /// <param name="value">Hex string</param>
        /// <returns>Byte array.</returns>
        public static byte[] HexToBytes(string value)
        {
            if (value == null || value.Length == 0)
            {
                return new byte[0];
            }
            int len = value.Length / 2;
            if (value.Length % 2 != 0)
            {
                ++len;
            }

            byte[] buffer = new byte[len];
            int lastValue = -1;
            int index = 0;
            foreach (byte ch in value)
            {
                if (IsHex(ch))
                {
                    if (lastValue == -1)
                    {
                        lastValue = GetValue(ch);
                    }
                    else if (lastValue != -1)
                    {
                        buffer[index] = (byte)(lastValue << 4 | GetValue(ch));
                        lastValue = -1;
                        ++index;
                    }
                }
                else if (lastValue != -1 && ch == ' ')
                {
                    buffer[index] = GetValue(ch);
                    lastValue = -1;
                    ++index;
                }
                else
                {
                    lastValue = -1;
                }
            }
            if (lastValue != -1)
            {
                buffer[index] = (byte)lastValue;
                ++index;
            }
            // If there are no spaces in the hex string.
            if (buffer.Length == index)
            {
                return buffer;
            }
            byte[] tmp = new byte[index];
            Buffer.BlockCopy(buffer, 0, tmp, 0, index);
            return tmp;
        }

        /// <summary>
        /// Convert object to byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Object as byte array.</returns>
        public static byte[] GetAsByteArray(object value)
        {
            if (value == null)
            {
                return [];
            }
            if (value is byte[] v)
            {
                return v;
            }
            if (value is string)
            {
                return Encoding.UTF8.GetBytes((string)value);
            }
            int rawsize;
            byte[] rawdata;
            GCHandle handle;
            if (value is Array arr)
            {
                if (arr.Length != 0)
                {
                    int valueSize = Marshal.SizeOf(arr.GetType().GetElementType());
                    rawsize = valueSize * arr.Length;
                    rawdata = new byte[rawsize];
                    handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
                    long pos = handle.AddrOfPinnedObject().ToInt64();
                    foreach (object it in arr)
                    {
                        Marshal.StructureToPtr(it, new IntPtr(pos), false);
                        pos += valueSize;
                    }
                    handle.Free();
                    return rawdata;
                }
                return [];
            }

            rawsize = Marshal.SizeOf(value);
            rawdata = new byte[rawsize];
            handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            handle.Free();
            return rawdata;
        }

        /// <summary>
        /// Convert byte array to object.
        /// </summary>
        /// <param name="byteArray">Byte array where Object uis created.</param>
        /// <param name="type">Object type.</param>
        /// <param name="index">Byte array index.</param>
        /// <param name="count">Byte count.</param>
        /// <param name="reverse">Is value reversed.</param>
        /// <param name="readBytes">Count of read bytes.</param>
        /// <returns>Return object of given type.</returns>
        public static object? ByteArrayToObject(byte[] byteArray, Type type, int index, int count, bool reverse, out int readBytes)
        {
            if (byteArray == null)
            {
                throw new ArgumentException("byteArray");
            }
            if (count <= 0)
            {
                count = byteArray.Length - index;
            }
            //If count is higher than one and type is not array.
            if (count != 1 && !type.IsArray && type != typeof(string))
            {
                throw new ArgumentException("count");
            }
            if (index < 0 || index > byteArray.Length)
            {
                throw new ArgumentException("index");
            }
            if (type == typeof(byte[]) && index == 0 && count == byteArray.Length)
            {
                readBytes = byteArray.Length;
                return byteArray;
            }
            readBytes = 0;
            Type valueType = null;
            int valueSize = 0;
            if (index != 0 || reverse)
            {
                if (type == typeof(string))
                {
                    readBytes = count;
                }
                else if (type.IsArray)
                {
                    valueType = type.GetElementType();
                    valueSize = Marshal.SizeOf(valueType);
                    readBytes = (valueSize * count);
                }
                else if (type == typeof(bool) || type == typeof(Boolean))
                {
                    readBytes = 1;
                }
                else
                {
                    readBytes = Marshal.SizeOf(type);
                }
                byte[] tmp = byteArray;
                byteArray = new byte[readBytes];
                Array.Copy(tmp, index, byteArray, 0, readBytes);
            }
            object? value = null;
            if (type == typeof(string))
            {
                return Encoding.UTF8.GetString(byteArray);
            }
            else if (reverse)
            {
                byteArray = byteArray.Reverse().ToArray();
            }
            GCHandle handle;
            if (type.IsArray)
            {
                if (count == -1)
                {
                    count = byteArray.Length / Marshal.SizeOf(valueType);
                }
                Array arr = (Array)Activator.CreateInstance(type, count);
                handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
                long start = handle.AddrOfPinnedObject().ToInt64();
                for (int pos = 0; pos != count; ++pos)
                {
                    arr.SetValue(Marshal.PtrToStructure(new IntPtr(start), valueType), pos);
                    start += valueSize;
                    readBytes += valueSize;
                }
                handle.Free();
                return arr;
            }
            handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
            value = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type);
            readBytes = Marshal.SizeOf(type);
            handle.Free();
            return value;
        }

        /// <summary>
        /// Convert received byte stream to wanted object.
        /// </summary>
        /// <param name="byteArray">Bytes to parse.</param>
        /// <param name="type">Object type.</param>
        /// <param name="readBytes">Read byte count.</param>
        /// <returns></returns>
        public static object? ByteArrayToObject(byte[] byteArray, Type type, out int readBytes)
        {
            return ByteArrayToObject(byteArray, type, 0, byteArray.Length, false, out readBytes);
        }

        /// <summary>
        /// Searches for the specified pattern and returns the index of the first occurrence
        /// within the range of elements in the byte buffer that starts at the specified
        /// index and contains the specified number of elements.
        /// </summary>
        /// <param name="input">Input byte buffer</param>
        /// <param name="pattern"></param>
        /// <param name="index">Index where search is started.</param>
        /// <param name="count">Maximum search buffer size.</param>
        /// <returns></returns>
        public static int IndexOf(byte[] input, byte[] pattern, int index, int count)
        {
            //If not enough data available.
            if ((count - index) < pattern.Length)
            {
                return -1;
            }
            byte firstByte = pattern[0];
            int pos = -1;
            if ((pos = Array.IndexOf(input, firstByte, index, count - index)) >= 0)
            {
                if (count - pos < pattern.Length)
                {
                    pos = -1;
                }
                else
                {
                    for (int i = 0; i < pattern.Length; i++)
                    {
                        if (pos + i >= input.Length || pattern[i] != input[pos + i])
                        {
                            return IndexOf(input, pattern, pos + 1, count);
                        }
                    }
                }
            }
            return pos;
        }

        /// <summary>
        /// Compares two byte or byte array values.
        /// </summary>
        public static bool EqualBytes(byte[] a, byte[] b)
        {
            if (a == null)
            {
                return b == null;
            }
            if (b == null)
            {
                return a == null;
            }
            if (a is Array && b is Array)
            {
                int pos = 0;
                if (((Array)a).Length != ((Array)b).Length)
                {
                    return false;
                }
                foreach (byte mIt in (byte[])a)
                {
                    if ((((byte)((byte[])b).GetValue(pos++)) & mIt) != mIt)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return BitConverter.Equals(a, b);
            }
            return true;
        }
    }
}

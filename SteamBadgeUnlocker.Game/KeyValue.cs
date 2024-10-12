﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SteamBadgeUnlocker.Game
{
    internal class KeyValue
    {
        private static readonly KeyValue _Invalid = new();
        public string Name = "<root>";
        public KeyValueType Type = KeyValueType.None;
        public object Value;
        public bool Valid;

        public List<KeyValue> Children = null;

        public KeyValue this[string key]
        {
            get
            {
                if (this.Children == null)
                {
                    return _Invalid;
                }

                var child = this.Children.SingleOrDefault(
                    c => string.Compare(c.Name, key, StringComparison.InvariantCultureIgnoreCase) == 0);

                if (child == null)
                {
                    return _Invalid;
                }

                return child;
            }
        }

        public string AsString(string defaultValue)
        {
            if (this.Valid == false)
            {
                return defaultValue;
            }

            if (this.Value == null)
            {
                return defaultValue;
            }

            return this.Value.ToString();
        }

        public int AsInteger(int defaultValue)
        {
            if (this.Valid == false)
            {
                return defaultValue;
            }

            switch (this.Type)
            {
                case KeyValueType.String:
                case KeyValueType.WideString:
                {
                    return int.TryParse((string)this.Value, out int value) == false
                        ? defaultValue
                        : value;
                }

                case KeyValueType.Int32:
                {
                    return (int)this.Value;
                }

                case KeyValueType.Float32:
                {
                    return (int)((float)this.Value);
                }

                case KeyValueType.UInt64:
                {
                    return (int)((ulong)this.Value & 0xFFFFFFFF);
                }
            }

            return defaultValue;
        }

        public float AsFloat(float defaultValue)
        {
            if (this.Valid == false)
            {
                return defaultValue;
            }

            switch (this.Type)
            {
                case KeyValueType.String:
                case KeyValueType.WideString:
                {
                    return float.TryParse((string)this.Value, out float value) == false
                        ? defaultValue
                        : value;
                }

                case KeyValueType.Int32:
                {
                    return (int)this.Value;
                }

                case KeyValueType.Float32:
                {
                    return (float)this.Value;
                }

                case KeyValueType.UInt64:
                {
                    return (ulong)this.Value & 0xFFFFFFFF;
                }
            }

            return defaultValue;
        }

        public bool AsBoolean(bool defaultValue)
        {
            if (this.Valid == false)
            {
                return defaultValue;
            }

            switch (this.Type)
            {
                case KeyValueType.String:
                case KeyValueType.WideString:
                {
                    return int.TryParse((string)this.Value, out int value) == false
                        ? defaultValue
                        : value != 0;
                }

                case KeyValueType.Int32:
                {
                    return ((int)this.Value) != 0;
                }

                case KeyValueType.Float32:
                {
                    return ((int)((float)this.Value)) != 0;
                }

                case KeyValueType.UInt64:
                {
                    return ((ulong)this.Value) != 0;
                }
            }

            return defaultValue;
        }

        public override string ToString()
        {
            if (this.Valid == false)
            {
                return "<invalid>";
            }

            if (this.Type == KeyValueType.None)
            {
                return this.Name;
            }

            return $"{this.Name} = {this.Value}";
        }

        public static KeyValue LoadAsBinary(string path)
        {
            if (File.Exists(path) == false)
            {
                return null;
            }

            try
            {
                using (var input = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    KeyValue kv = new();
                    if (kv.ReadAsBinary(input) == false)
                    {
                        return null;
                    }
                    return kv;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool ReadAsBinary(Stream input)
        {
            this.Children = new();
            try
            {
                while (true)
                {
                    var type = (KeyValueType)input.ReadValueU8();

                    if (type == KeyValueType.End)
                    {
                        break;
                    }

                    KeyValue current = new()
                    {
                        Type = type,
                        Name = input.ReadStringUnicode(),
                    };

                    switch (type)
                    {
                        case KeyValueType.None:
                        {
                            current.ReadAsBinary(input);
                            break;
                        }

                        case KeyValueType.String:
                        {
                            current.Valid = true;
                            current.Value = input.ReadStringUnicode();
                            break;
                        }

                        case KeyValueType.WideString:
                        {
                            throw new FormatException("wstring is unsupported");
                        }

                        case KeyValueType.Int32:
                        {
                            current.Valid = true;
                            current.Value = input.ReadValueS32();
                            break;
                        }

                        case KeyValueType.UInt64:
                        {
                            current.Valid = true;
                            current.Value = input.ReadValueU64();
                            break;
                        }

                        case KeyValueType.Float32:
                        {
                            current.Valid = true;
                            current.Value = input.ReadValueF32();
                            break;
                        }

                        case KeyValueType.Color:
                        {
                            current.Valid = true;
                            current.Value = input.ReadValueU32();
                            break;
                        }

                        case KeyValueType.Pointer:
                        {
                            current.Valid = true;
                            current.Value = input.ReadValueU32();
                            break;
                        }

                        default:
                        {
                            throw new FormatException();
                        }
                    }

                    if (input.Position >= input.Length)
                    {
                        throw new FormatException();
                    }

                    this.Children.Add(current);
                }

                this.Valid = true;
                return input.Position == input.Length;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

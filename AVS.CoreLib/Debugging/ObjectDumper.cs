using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.Debugging
{
    public class ObjectDumper
    {
        private int _level;
        private readonly int _indentSize;
        private readonly StringBuilder _stringBuilder;
        private readonly List<int> _hashListOfFoundElements;

        private ObjectDumper(int indentSize)
        {
            _indentSize = indentSize;
            _stringBuilder = new StringBuilder();
            _hashListOfFoundElements = new List<int>();
        }

        public static string Dump(object element)
        {
            return Dump(element, 2);
        }

        public static string Dump(object element, int indentSize)
        {
            var instance = new ObjectDumper(indentSize);
            return instance.DumpElement(element);
        }

        private string DumpElement(object? element)
        {
            if (element == null || element is ValueType || element is string)
            {
                var str = FormatValue(element);
                if (str.Length < 12)
                {
                    WriteInline(str + ",");
                }
                else
                {
                    Write(str + ",");
                }
            }
            else
            {
                var objectType = element.GetType();
                if (!typeof(IEnumerable).IsAssignableFrom(objectType))
                {
                    Write("`{0}`", objectType.GetReadableName());
                    _hashListOfFoundElements.Add(element.GetHashCode());
                    _level++;
                }

                var enumerableElement = element as IEnumerable;
                if (enumerableElement != null)
                {
                    Write("[");
                    _level++;
                    foreach (object item in enumerableElement)
                    {
                        if (item is IEnumerable && !(item is string))
                        {
                            _level++;
                            DumpElement(item);
                            _level--;
                        }
                        else
                        {
                            if (!AlreadyTouched(item))
                                DumpElement(item);
                            else
                                Write("{{{0}}} <-- bidirectional reference found", item.GetType().FullName!);
                        }
                    }
                    _level--;
                    if (_stringBuilder[^1] == ',')
                    {
                        _stringBuilder.Length--;
                        WriteInline("],");
                        _stringBuilder.AppendLine();
                    }
                    else
                    {
                        Write("],");
                    }
                }
                else
                {
                    Write("{");
                    _level++;
                    var members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var memberInfo in members)
                    {
                        var fieldInfo = memberInfo as FieldInfo;
                        var propertyInfo = memberInfo as PropertyInfo;

                        if (fieldInfo == null && propertyInfo == null)
                            continue;

                        var type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo!.PropertyType;
                        var value = fieldInfo != null
                                           ? fieldInfo.GetValue(element)
                                           : propertyInfo!.GetValue(element, null);

                        if (type.IsValueType || type == typeof(string))
                        {
                            Write("{0}: {1},", memberInfo.Name, FormatValue(value));
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
                            Write("{0}:", memberInfo.Name);

                            var alreadyTouched = !isEnumerable && AlreadyTouched(value);
                            _level++;
                            if (!alreadyTouched)
                            {
                                DumpElement(value);
                            }
                            else
                                Write("`{0}` <-- bidirectional reference found", value.GetType().GetReadableName());
                            _level--;
                        }
                    }
                    _level--;
                    Write("},");
                }

                if (!typeof(IEnumerable).IsAssignableFrom(objectType))
                {
                    _level--;
                }

                if (_stringBuilder[^1] == ',')
                    _stringBuilder.Length--;
            }

            return _stringBuilder.ToString();
        }

        private bool AlreadyTouched(object? value)
        {
            if (value == null)
                return false;

            var hash = value.GetHashCode();
            for (var i = 0; i < _hashListOfFoundElements.Count; i++)
            {
                if (_hashListOfFoundElements[i] == hash)
                    return true;
            }
            return false;
        }

        private void WriteInline(string value, params object[]? args)
        {
            if (_stringBuilder.Length > 3 && (_stringBuilder[^3] == '[' || _stringBuilder[^3] == '{'))
            {
                _stringBuilder.Length -= 2;
            }

            if (args is { Length: > 0 })
                value = string.Format(value, args);

            _stringBuilder.Append(' ' + value);
        }

        private void Write(string value, params object[] args)
        {
            var space = new string(' ', _level * _indentSize);

            if (args is { Length: > 0 })
                value = string.Format(value, args);

            _stringBuilder.AppendLine(space + value);
        }

        private string FormatValue(object? o)
        {
            return o switch
            {
                null => ("null"),
                DateTime time => (time.ToShortDateString()),
                string => string.Format("\"{0}\"", o),
                char c when c == '\0' => string.Empty,
                ValueType => (o.ToString()!),
                IEnumerable => ("..."),
                _ => ("{ }")
            };
        }
    }
}
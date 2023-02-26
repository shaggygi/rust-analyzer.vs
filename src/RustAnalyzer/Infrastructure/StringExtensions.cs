using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace KS.RustAnalyzer.Infrastructure;

public static class StringExtensions
{
    // Environment variables should be passed as a null-terminated block of null-terminated strings. Each string is in the following form:name=value\0.
    // NOTE: This is here because TextFieldParser is not in .NET Standard.
    public static string GetEnvironmentBlock(this string @this)
    {
        using var sr = new StringReader(@this);
        using var tfp = new TextFieldParser(sr) { Delimiters = new[] { " " }, HasFieldsEnclosedInQuotes = true, TextFieldType = FieldType.Delimited };

        var kvSep = new[] { "=" };
        return (tfp.ReadFields() ?? Array.Empty<string>())
            .Select(s => s.Split(kvSep, StringSplitOptions.RemoveEmptyEntries))
            .Where(s => s.Length == 2)
            .Aggregate(new StringBuilder(), (acc, e) => acc.AppendFormat("{0}={1}\0", e[0], e[1]))
            .Append('\0')
            .ToString();
    }
}

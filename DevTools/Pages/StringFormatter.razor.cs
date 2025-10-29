using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NSQLFormatter;

namespace DevTools.Pages;

public partial class StringFormatter : ComponentBase
{
    private delegate string TokenTransformer(string[] tokens);

    private sealed class TextStatistics
    {
        public TextStatistics(int characters, int words, int lines, int bytes, double? readingTimeMinutes)
        {
            Characters = characters;
            Words = words;
            Lines = lines;
            Bytes = bytes;
            ReadingTimeMinutes = readingTimeMinutes;
        }

        public int Characters { get; }
        public int Words { get; }
        public int Lines { get; }
        public int Bytes { get; }
        public double? ReadingTimeMinutes { get; }
    }

    private const string PillButton = "px-3 py-1 text-xs font-medium text-white rounded-md hover:brightness-95 transition-colors";
    private string? Input { get; set; }
    private string? Output { get; set; }
    private string? OperationMessage { get; set; }
    private TextStatistics? Statistics { get; set; }

    private bool EnsureInput()
    {
        if (string.IsNullOrWhiteSpace(Input))
        {
            OperationMessage = "Paste or type text to transform.";
            Statistics = null;
            return false;
        }

        return true;
    }

    private void SetOutput(string value, string? message = null, bool calculateStats = true)
    {
        Output = value;
        OperationMessage = message;
        Statistics = calculateStats ? CalculateStatistics(value) : null;
    }

    private void ClearFeedback()
    {
        OperationMessage = null;
        Statistics = null;
    }

    private void FormatJson()
    {
        if (!EnsureInput())
        {
            return;
        }

        try
        {
            dynamic? parsedJson = JsonConvert.DeserializeObject(Input!);
            var formatted = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            SetOutput(formatted, "Formatted JSON with indentation.");
        }
        catch (JsonException ex)
        {
            SetOutput(string.Empty, $"JSON parse error: {ex.Message}", calculateStats: false);
        }
    }

    private void MinifyJson()
    {
        if (!EnsureInput())
        {
            return;
        }

        try
        {
            dynamic? parsedJson = JsonConvert.DeserializeObject(Input!);
            var minified = JsonConvert.SerializeObject(parsedJson, Formatting.None);
            SetOutput(minified, "Removed whitespace and line breaks from JSON.");
        }
        catch (JsonException ex)
        {
            SetOutput(string.Empty, $"JSON parse error: {ex.Message}", calculateStats: false);
        }
    }

    private void FormatSQL()
    {
        if (!EnsureInput())
        {
            return;
        }

        try
        {
            var formatted = Formatter.Format(Input!);
            SetOutput(formatted, "Formatted SQL statement.");
        }
        catch (Exception ex)
        {
            SetOutput(string.Empty, $"SQL formatter error: {ex.Message}", calculateStats: false);
        }
    }

    private void EscapeJson()
    {
        if (!EnsureInput())
        {
            return;
        }

        var escaped = JsonConvert.ToString(Input!).Trim('"');
        SetOutput(escaped, "Escaped characters for embedding into JSON.");
    }

    private void UnescapeJson()
    {
        if (!EnsureInput())
        {
            return;
        }

        try
        {
            var unescaped = JsonConvert.DeserializeObject<string>($"\"{Input!.Replace("\"", "\\\"")}\"");
            SetOutput(unescaped ?? string.Empty, "Unescaped JSON string.");
        }
        catch (JsonException ex)
        {
            SetOutput(string.Empty, $"Unable to unescape JSON string: {ex.Message}", calculateStats: false);
        }
    }

    private void Upper()
    {
        if (!EnsureInput())
        {
            return;
        }

        SetOutput(Input!.ToUpper(CultureInfo.InvariantCulture), "Converted to uppercase.");
    }

    private void Lower()
    {
        if (!EnsureInput())
        {
            return;
        }

        SetOutput(Input!.ToLower(CultureInfo.InvariantCulture), "Converted to lowercase.");
    }

    private void TitleCase()
    {
        if (!EnsureInput())
        {
            return;
        }

        var textInfo = CultureInfo.InvariantCulture.TextInfo;
        SetOutput(textInfo.ToTitleCase(Input!.ToLower(CultureInfo.InvariantCulture)), "Converted to title case.");
    }

    private void PascalCase()
    {
        TransformTokens(tokens => string.Concat(tokens.Select(Capitalize)), "Converted to PascalCase.");
    }

    private void CamelCase()
    {
        TransformTokens(tokens =>
        {
            var first = tokens.FirstOrDefault() ?? string.Empty;
            var rest = tokens.Skip(1).Select(Capitalize);
            return string.Concat(first.ToLower(CultureInfo.InvariantCulture), string.Concat(rest));
        }, "Converted to camelCase.");
    }

    private void SnakeCase()
    {
        TransformTokens(tokens => string.Join("_", tokens).ToLower(CultureInfo.InvariantCulture), "Converted to snake_case.");
    }

    private void KebabCase()
    {
        TransformTokens(tokens => string.Join("-", tokens).ToLower(CultureInfo.InvariantCulture), "Converted to kebab-case.");
    }

    private void TrimWhitespace()
    {
        if (!EnsureInput())
        {
            return;
        }

        SetOutput(Input!.Trim(), "Trimmed leading and trailing whitespace.");
    }

    private void CollapseWhitespace()
    {
        if (!EnsureInput())
        {
            return;
        }

        var collapsed = Regex.Replace(Input!, "\\s+", " ");
        SetOutput(collapsed.Trim(), "Collapsed whitespace to single spaces.");
    }

    private void RemoveDuplicateLines()
    {
        if (!EnsureInput())
        {
            return;
        }

        var lines = GetLines(Input!);
        var distinct = lines.Distinct().ToArray();
        SetOutput(string.Join(Environment.NewLine, distinct), $"Removed {lines.Length - distinct.Length} duplicate line(s).");
    }

    private void SortLines()
    {
        if (!EnsureInput())
        {
            return;
        }

        var lines = GetLines(Input!);
        var sorted = lines.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();
        SetOutput(string.Join(Environment.NewLine, sorted), "Sorted lines alphabetically.");
    }

    private void ReverseLines()
    {
        if (!EnsureInput())
        {
            return;
        }

        var lines = GetLines(Input!);
        Array.Reverse(lines);
        SetOutput(string.Join(Environment.NewLine, lines), "Reversed line order.");
    }

    private void Base64EncodeText()
    {
        if (!EnsureInput())
        {
            return;
        }

        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(Input!));
        SetOutput(encoded, "Base64 encoded using UTF-8.");
    }

    private void Base64DecodeText()
    {
        if (!EnsureInput())
        {
            return;
        }

        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(Input!));
            SetOutput(decoded, "Decoded Base64 using UTF-8.");
        }
        catch (FormatException ex)
        {
            SetOutput(string.Empty, $"Invalid Base64 payload: {ex.Message}", calculateStats: false);
        }
    }

    private void URLEncodeText()
    {
        if (!EnsureInput())
        {
            return;
        }

        SetOutput(System.Net.WebUtility.UrlEncode(Input!), "URL encoded string.");
    }

    private void URLDecodeText()
    {
        if (!EnsureInput())
        {
            return;
        }

        SetOutput(System.Net.WebUtility.UrlDecode(Input!), "URL decoded string.");
    }

    private void ReverseText()
    {
        if (!EnsureInput())
        {
            return;
        }

        var chars = Input!.ToCharArray();
        Array.Reverse(chars);
        SetOutput(new string(chars), "Reversed characters.");
    }

    private void AnalyzeText()
    {
        if (!EnsureInput())
        {
            return;
        }

        Statistics = CalculateStatistics(Input!);
        OperationMessage = "Analysis complete (values based on UTF-8).";
    }

    private void SwapInputAndOutput()
    {
        (Input, Output) = (Output, Input);
        if (!string.IsNullOrEmpty(Input))
        {
            Statistics = CalculateStatistics(Input);
            OperationMessage = "Swapped input and output.";
        }
        else
        {
            ClearFeedback();
        }
    }

    private void PasteSampleJson()
    {
        Input = "{\n    \"id\": \"c63b9c62-5b9d-4fb1-8b0c-6f6b3a3def42\",\n    \"name\": \"Sample tenant\",\n    \"users\": [\n        { \"id\": 1, \"email\": \"admin@example.com\" },\n        { \"id\": 2, \"email\": \"owner@example.com\" }\n    ],\n    \"features\": { \"darkMode\": true, \"beta\": false }\n}";
        ClearFeedback();
    }

    private void ClearInput()
    {
        Input = string.Empty;
        Output = string.Empty;
        ClearFeedback();
    }

    private void TransformTokens(TokenTransformer transformer, string successMessage)
    {
        if (!EnsureInput())
        {
            return;
        }

        var tokens = Tokenize(Input!);
        if (tokens.Length == 0)
        {
            SetOutput(string.Empty, "No alphanumeric tokens were found.", calculateStats: false);
            return;
        }

        var result = transformer(tokens);
        SetOutput(result, successMessage);
    }

    private string Capitalize(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        if (value.Length == 1)
        {
            return value.ToUpperInvariant();
        }

        return char.ToUpperInvariant(value[0]) + value[1..].ToLowerInvariant();
    }

    private string[] Tokenize(string value)
    {
        var matches = Regex.Matches(value, "[A-Za-z0-9]+");
        var result = new string[matches.Count];
        var index = 0;
        foreach (Match match in matches)
        {
            result[index++] = match.Value;
        }

        return result;
    }

    private string[] GetLines(string value)
    {
        return value.Replace("\r\n", "\n").Split('\n', StringSplitOptions.None);
    }

    private TextStatistics CalculateStatistics(string value)
    {
        var lines = GetLines(value);
        var words = Regex.Matches(value, "\\b\\w+\\b").Count;
        var bytes = Encoding.UTF8.GetByteCount(value);
        var readingTime = words > 0 ? words / 200.0 : (double?)null;
        return new TextStatistics(value.Length, words, lines.Length, bytes, readingTime);
    }

    private async Task CopyOutputToClipboard()
    {
        if (!string.IsNullOrEmpty(Output))
        {
            await JSRuntime.InvokeVoidAsync("copyToClipboard", Output, "copy-format-output-btn");
            OperationMessage = "Output copied to clipboard.";
        }
        else
        {
            OperationMessage = "Nothing to copy yet.";
        }
    }
}

namespace ErpApi.Configuration;

internal static class EnvironmentVariableLoader
{
    public static void LoadDotEnv(string rootDirectory)
    {
        if (string.IsNullOrWhiteSpace(rootDirectory))
        {
            return;
        }

        var envPath = Path.Combine(rootDirectory, ".env");
        if (!File.Exists(envPath))
        {
            return;
        }

        foreach (var line in File.ReadLines(envPath))
        {
            var trimmed = line.Trim();

            if (IsCommentOrEmpty(trimmed))
            {
                continue;
            }

            var separatorIndex = trimmed.IndexOf('=', StringComparison.Ordinal);
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = trimmed[..separatorIndex].Trim();
            if (key.Length == 0)
            {
                continue;
            }

            // Prefer externally provided values (e.g., CI environment variables).
            var existingValue = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrWhiteSpace(existingValue))
            {
                continue;
            }

            var value = trimmed[(separatorIndex + 1)..].Trim();
            if (value.Length >= 2 && value.StartsWith('"') && value.EndsWith('"'))
            {
                value = value[1..^1];
            }

            Environment.SetEnvironmentVariable(key, value);
        }
    }

    private static bool IsCommentOrEmpty(string line)
    {
        return string.IsNullOrEmpty(line) || line.StartsWith('#');
    }
}

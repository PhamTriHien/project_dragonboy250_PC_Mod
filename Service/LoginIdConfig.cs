using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class LoginIdConfig
{
	private const string LOGIN_ID_RESOURCE_PATH = "login_codes_generated";

	private static readonly object configLock = new object();

	private static bool loaded;

	private static bool valid;

	private static string loginCode;

	public static string getLoginCode()
	{
		lock (configLock)
		{
			if (!loaded)
			{
				loaded = true;
				string rawText = LoadRawConfigText();
				if (string.IsNullOrEmpty(rawText))
				{
					Debug.LogError("[LOGIN] Missing or empty Resources/login_codes_generated.txt.");
					return null;
				}
				string[] lines = rawText.Split('\n');
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i].Trim();
					if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal))
					{
						continue;
					}
					int colonIdx = line.IndexOf(':');
					if (colonIdx <= 0)
					{
						return LogInvalidLoginIdConfig();
					}
					string candidate = line.Substring(colonIdx + 1).Trim();
					if (!IsValidLoginId(candidate) || loginCode != null)
					{
						return LogInvalidLoginIdConfig();
					}
					loginCode = candidate;
				}
				if (loginCode == null)
				{
					return LogInvalidLoginIdConfig();
				}
				valid = true;
				return loginCode;
			}
			return valid ? loginCode : null;
		}
	}

	private static string LoadRawConfigText()
	{
		TextAsset textAsset = Resources.Load(LOGIN_ID_RESOURCE_PATH, typeof(TextAsset)) as TextAsset;
		if (textAsset != null && !string.IsNullOrEmpty(textAsset.text))
		{
			return textAsset.text;
		}
		try
		{
			using (Stream stream = typeof(LoginIdConfig).Assembly.GetManifestResourceStream("login_codes_generated.txt"))
			{
				if (stream != null)
				{
					using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
					{
						return reader.ReadToEnd();
					}
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private static bool IsValidLoginId(string value)
	{
		if (value == null || value.Length != 32)
		{
			return false;
		}
		for (int i = 0; i < value.Length; i++)
		{
			char c = value[i];
			bool isHex = (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
			if (!isHex)
			{
				return false;
			}
		}
		return true;
	}

	private static string LogInvalidLoginIdConfig()
	{
		Debug.LogError("[LOGIN] Invalid Resources/login_codes_generated.txt. Expected one entry with a 32-character hexadecimal value.");
		return null;
	}
}

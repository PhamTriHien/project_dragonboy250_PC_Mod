using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class Rms
{
	public static int status;

	public static sbyte[] data;

	public static string filename;

	private const int INTERVAL = 5;

	private const int MAXTIME = 500;

	public static string RMS_ResVersion = "ResVersion";

	public static string RMS_clienttype = "clienttype";

	public static string RMS_lastZoomlevel = "lastZoomlevel";

	public static string RMS_userAo = "userAo";

	public static string RMS_acc = "acc";

	public static string RMS_pass = "pass";

	public static string RMS_check = "check";

	private static readonly object rmsLock = new object();

	public static System.Collections.Generic.Dictionary<string, sbyte[]> memCache = new System.Collections.Generic.Dictionary<string, sbyte[]>();

	public static void saveRMS(string filename, sbyte[] data)
	{
		__saveRMS(filename, data);
	}

	public static sbyte[] loadRMS(string filename)
	{
		return __loadRMS(filename);
	}

	public static string loadRMSString(string fileName)
	{
		sbyte[] array = loadRMS(fileName);
		if (array == null)
		{
			return null;
		}
		DataInputStream dataInputStream = new DataInputStream(array);
		try
		{
			string result = dataInputStream.readUTF();
			dataInputStream.close();
			if (fileName == RMS_pass && result != null && result.StartsWith("ENC_V1:"))
			{
				result = DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.DeobfuscatePassword(result);
			}
			return result;
		}
		catch (Exception ex)
		{
			Cout.println(ex.StackTrace);
		}
		return null;
	}

	public static byte[] convertSbyteToByte(sbyte[] var)
	{
		byte[] array = new byte[var.Length];
		for (int i = 0; i < var.Length; i++)
		{
			if (var[i] > 0)
			{
				array[i] = (byte)var[i];
			}
			else
			{
				array[i] = (byte)(var[i] + 256);
			}
		}
		return array;
	}

	public static void saveRMSString(string filename, string data)
	{
		DataOutputStream dataOutputStream = new DataOutputStream();
		try
		{
			dataOutputStream.writeUTF(data);
			saveRMS(filename, dataOutputStream.toByteArray());
			dataOutputStream.close();
		}
		catch (Exception ex)
		{
			Cout.println(ex.StackTrace);
		}
	}

	private static void _saveRMS(string filename, sbyte[] data)
	{
		__saveRMS(filename, data);
	}

	private static sbyte[] _loadRMS(string filename)
	{
		return __loadRMS(filename);
	}

	public static void update()
	{
		if (status == 2)
		{
			status = 1;
			__saveRMS(filename, data);
			status = 0;
		}
		else if (status == 3)
		{
			status = 1;
			data = __loadRMS(filename);
			status = 0;
		}
	}

	public static int loadRMSInt(string file)
	{
		sbyte[] array = loadRMS(file);
		return (array != null && array.Length > 0) ? array[0] : (-1);
	}

	public static void saveRMSInt(string file, int x)
	{
		try
		{
			saveRMS(file, new sbyte[1] { (sbyte)x });
			if (file == ServerListScreen.RMS_svselect)
			{
				Debug.LogError(">>>>>>>>Save saveRMSInt: " + file + "  index:" + x);
			}
		}
		catch (Exception)
		{
		}
	}

	public static string GetiPhoneDocumentsPath()
	{
		return Application.persistentDataPath;
	}

	private static void __saveRMS(string filename, sbyte[] data)
	{
		if (string.IsNullOrEmpty(filename) || data == null) return;
		lock (rmsLock)
		{
			try
			{
				string text = GetiPhoneDocumentsPath() + "/" + filename;
				sbyte[] cachedCopy = new sbyte[data.Length];
				Buffer.BlockCopy(data, 0, cachedCopy, 0, data.Length);
				memCache[text] = cachedCopy;
				string dir = Path.GetDirectoryName(text);
				if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
				byte[] rawBytes = ArrayCast.cast(data);
				for (int attempt = 0; attempt < 3; attempt++)
				{
					try
					{
						using (FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
						{
							fileStream.Write(rawBytes, 0, rawBytes.Length);
							fileStream.Flush();
						}
						break;
					}
					catch (IOException)
					{
						if (attempt == 2) throw;
						Thread.Sleep(20);
					}
				}
				Main.setBackupIcloud(text);
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi __saveRMS " + filename + ": " + ex.Message);
			}
		}
	}

	private static sbyte[] __loadRMS(string filename)
	{
		if (string.IsNullOrEmpty(filename)) return null;
		lock (rmsLock)
		{
			string fullPath = GetiPhoneDocumentsPath() + "/" + filename;
			if (memCache.TryGetValue(fullPath, out sbyte[] cached) && cached != null)
			{
				sbyte[] copy = new sbyte[cached.Length];
				Buffer.BlockCopy(cached, 0, copy, 0, cached.Length);
				return copy;
			}
			try
			{
				if (File.Exists(fullPath))
				{
					for (int attempt = 0; attempt < 3; attempt++)
					{
						try
						{
							using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
							{
								byte[] array = new byte[fileStream.Length];
								int offset = 0;
								while (offset < array.Length)
								{
									int n = fileStream.Read(array, offset, array.Length - offset);
									if (n <= 0) break;
									offset += n;
								}
								sbyte[] sbytes = ArrayCast.cast(array);
								sbyte[] toCache = new sbyte[sbytes.Length];
								Buffer.BlockCopy(sbytes, 0, toCache, 0, sbytes.Length);
								memCache[fullPath] = toCache;
								return sbytes;
							}
						}
						catch (IOException)
						{
							if (attempt == 2) throw;
							Thread.Sleep(20);
						}
					}
				}
			}
			catch (Exception)
			{
			}

			// Fallback to bundled data templates (e.g. NR_image, NR_part, NR_dart, etc.)
			try
			{
				var textAsset = Resources.Load("data/" + filename) as TextAsset;
				if (textAsset != null && textAsset.bytes != null && textAsset.bytes.Length > 0)
				{
					sbyte[] sbytes = ArrayCast.cast(textAsset.bytes);
					sbyte[] toCache = new sbyte[sbytes.Length];
					Buffer.BlockCopy(sbytes, 0, toCache, 0, sbytes.Length);
					memCache[fullPath] = toCache;
					return sbytes;
				}
			}
			catch (Exception)
			{
			}

			try
			{
				string baseDir = AppDomain.CurrentDomain.BaseDirectory;
				string[] paths = new string[]
				{
					Path.Combine(Path.Combine(baseDir, "Assets"), Path.Combine("data", filename)),
					Path.Combine(Path.Combine(baseDir, "data"), filename),
					Path.Combine(@"C:\ModNRO\DragonBoy_Net8_Native\Assets\data", filename)
				};
				foreach (string p in paths)
				{
					if (File.Exists(p))
					{
						byte[] bytes = File.ReadAllBytes(p);
						sbyte[] sbytes = ArrayCast.cast(bytes);
						sbyte[] toCache = new sbyte[sbytes.Length];
						Buffer.BlockCopy(sbytes, 0, toCache, 0, sbytes.Length);
						memCache[fullPath] = toCache;
						return sbytes;
					}
				}
			}
			catch (Exception)
			{
			}

			return null;
		}
	}

	public static void clearAll()
	{
		lock (rmsLock)
		{
			Cout.LogError3("clean rms");
			string dirPath = GetiPhoneDocumentsPath();
			FileInfo[] files = new DirectoryInfo(dirPath + "/").GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				if (!(fileInfo.Name == GameMidlet.VERSION) && !(fileInfo.Name == ServerListScreen.RMS_svselect) && !(fileInfo.Name == RMS_acc) && !(fileInfo.Name == RMS_pass) && !(fileInfo.Name == RMS_check))
				{
					memCache.Remove(dirPath + "/" + fileInfo.Name);
					fileInfo.Delete();
				}
			}
		}
	}

	public static void DeleteStorage(string path)
	{
		lock (rmsLock)
		{
			try
			{
				string fullPath = GetiPhoneDocumentsPath() + "/" + path;
				memCache.Remove(fullPath);
				File.Delete(fullPath);
			}
			catch (Exception)
			{
			}
		}
	}

	public static string ByteArrayToString(byte[] ba)
	{
		string text = BitConverter.ToString(ba);
		return text.Replace("-", string.Empty);
	}

	public static byte[] StringToByteArray(string hex)
	{
		int length = hex.Length;
		byte[] array = new byte[length / 2];
		for (int i = 0; i < length; i += 2)
		{
			array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
		}
		return array;
	}

	public static void deleteRecord(string name)
	{
		if (string.IsNullOrEmpty(name)) return;
		lock (rmsLock)
		{
			try
			{
				PlayerPrefs.DeleteKey(name);
				string text = GetiPhoneDocumentsPath() + "/" + name;
				memCache.Remove(text);
				if (File.Exists(text))
				{
					File.Delete(text);
				}
			}
			catch (Exception ex)
			{
				Cout.println("loi xoa RMS --------------------------" + ex.ToString());
			}
		}
	}

	public static void clearRMS()
	{
		deleteRecord("data");
		deleteRecord("dataVersion");
		deleteRecord("map");
		deleteRecord("mapVersion");
		deleteRecord("skill");
		deleteRecord("killVersion");
		deleteRecord("item");
		deleteRecord("itemVersion");
	}

	public static void saveIP(string strID)
	{
		saveRMSString("NRIPlink", strID);
	}

	public static string loadIP()
	{
		string text = loadRMSString("NRIPlink");
		if (text == null)
		{
			return null;
		}
		return text;
	}
}

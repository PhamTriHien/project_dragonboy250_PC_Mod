using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
#if NET8_0_OR_GREATER
using System.Net.Http;
#else
using System.Net;
#endif
using UnityEngine;

public class BackgroundItem
{
	public string id;
	public string name;
	public string filename;
	public string url;
	public string size;
	public string desc;
	public int topColor;

	public bool isDownloaded;
	public bool isDownloading;
	public int downloadPercent;

	public BackgroundItem(string id, string name, string filename, int topColor, string size, string desc)
	{
		this.id = id;
		this.name = name;
		this.filename = filename;
		this.topColor = topColor;
		this.size = size;
		this.desc = desc;
		this.url = "https://raw.githubusercontent.com/PhamTriHien/project_dragonboy250_PC_Mod/main/Backgrounds/" + filename;
		this.isDownloaded = false;
		this.isDownloading = false;
		this.downloadPercent = 0;
	}
}

public static class ModBackground
{
	public static bool isCustomBGActive = false;
	public static string selectedBgId = string.Empty;
	public static Image currentBGImage = null;
	public static int currentTopColor = 0;
	public static string pendingApplyId = null;

	public static readonly List<BackgroundItem> bgList = new List<BackgroundItem>()
	{
		new BackgroundItem("bg_galaxy", "Thiên Hà Galaxy", "bg_galaxy.png", 0x060718, "58 KB", "Bầu trời vũ trụ với dải ngân hà và tinh vân huyền ảo."),
		new BackgroundItem("bg_sunset", "Hoàng Hôn Sunset", "bg_sunset.png", 0x1E1035, "54 KB", "Chiều tà rực rỡ với vầng thái dương lặn sau rặng núi."),
		new BackgroundItem("bg_sakura", "Anh Đào Sakura", "bg_sakura.png", 0x3A2352, "62 KB", "Đỉnh núi phủ tuyết cùng những cánh hoa anh đào mùa xuân."),
		new BackgroundItem("bg_cyberpunk", "Đêm Cyberpunk", "bg_cyberpunk.png", 0x0D0C1D, "60 KB", "Thành phố công nghệ tương lai với ánh đèn neon huyền ảo."),
		new BackgroundItem("bg_snow_mountain", "Tuyết Sơn Bắc Cực", "bg_snow_mountain.png", 0x0A1E38, "56 KB", "Núi tuyết lạnh giá hùng vĩ dưới dải cực quang lung linh."),
		new BackgroundItem("bg_namek_fantasy", "Huyền Ảo Namek", "bg_namek_fantasy.png", 0x09252A, "55 KB", "Bầu trời hành tinh Namek với tam nguyệt và đảo nổi kỳ bí.")
	};

	public static string GetBackgroundsDirectory()
	{
		string bgDir;
		try
		{
			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || mSystem.clientType == 2 || mSystem.clientType == 3)
			{
				bgDir = Path.Combine(Application.persistentDataPath, "Backgrounds");
			}
			else
			{
				string baseDir = AppDomain.CurrentDomain.BaseDirectory;
				if (string.IsNullOrEmpty(baseDir))
				{
					baseDir = Application.dataPath;
				}
				bgDir = Path.Combine(baseDir, "Backgrounds");
			}
			if (!Directory.Exists(bgDir))
			{
				Directory.CreateDirectory(bgDir);
			}
		}
		catch
		{
			bgDir = "Backgrounds";
		}
		return bgDir;
	}

	public static string GetLocalImagePath(string filename)
	{
		string targetDir = GetBackgroundsDirectory();
		string primaryPath = Path.Combine(targetDir, filename);
		if (File.Exists(primaryPath))
		{
			return primaryPath;
		}

		string[] candidates = new string[]
		{
			Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Backgrounds"), filename),
			Path.Combine(Path.Combine(Application.dataPath, "Backgrounds"), filename),
			Path.Combine(Path.Combine(Application.dataPath, "../Backgrounds"), filename),
			Path.Combine(@"C:\ModNRO\DragonBoy_Net8_Native\Backgrounds", filename),
			Path.Combine(@"C:\ModNRO\ModNRO_Tools\Decompiled\Dragonboy250_PC_projectbuild\Backgrounds", filename)
		};

		for (int i = 0; i < candidates.Length; i++)
		{
			try
			{
				if (File.Exists(candidates[i]))
				{
					return candidates[i];
				}
			}
			catch
			{
			}
		}

		return null;
	}

	public static void RefreshDownloadedStatus()
	{
		for (int i = 0; i < bgList.Count; i++)
		{
			string path = GetLocalImagePath(bgList[i].filename);
			bgList[i].isDownloaded = (!string.IsNullOrEmpty(path) && File.Exists(path));
		}
	}

	public static void Init()
	{
		RefreshDownloadedStatus();
		if (isCustomBGActive && !string.IsNullOrEmpty(selectedBgId))
		{
			ApplyBackground(selectedBgId, saveConfig: false);
		}
	}

	public static void Update()
	{
		if (!string.IsNullOrEmpty(pendingApplyId))
		{
			string id = pendingApplyId;
			pendingApplyId = null;
			ApplyBackground(id, saveConfig: true);
		}
	}

	public static bool ApplyBackground(string id, bool saveConfig = true)
	{
		BackgroundItem item = bgList.Find(b => b.id.Equals(id, StringComparison.OrdinalIgnoreCase));
		if (item == null)
		{
			return false;
		}

		string filePath = GetLocalImagePath(item.filename);
		if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
		{
			GameScr.info1?.addInfo("Chưa tải hình nền này về máy!", 0);
			return false;
		}

		try
		{
			byte[] bytes = File.ReadAllBytes(filePath);
			if (bytes == null || bytes.Length == 0)
			{
				return false;
			}

			Image img = Image.createImage(bytes);
			if (img != null && img.texture != null)
			{
				currentBGImage = img;
				selectedBgId = item.id;
				isCustomBGActive = true;
				currentTopColor = item.topColor;
				if (saveConfig)
				{
					ModConfig.SaveConfig();
				}
				GameScr.info1?.addInfo("Đã áp dụng: " + item.name, 0);
				return true;
			}
		}
		catch (Exception ex)
		{
			GameScr.info1?.addInfo("Lỗi nạp ảnh: " + ex.Message, 0);
		}

		return false;
	}

	public static void ResetToDefault(bool saveConfig = true)
	{
		isCustomBGActive = false;
		selectedBgId = string.Empty;
		currentBGImage = null;
		if (saveConfig)
		{
			ModConfig.SaveConfig();
		}
		GameScr.info1?.addInfo("Đã khôi phục phong cảnh mặc định!", 0);
	}

	public static void DeleteBackground(string id)
	{
		BackgroundItem item = bgList.Find(b => b.id.Equals(id, StringComparison.OrdinalIgnoreCase));
		if (item == null)
		{
			return;
		}

		if (isCustomBGActive && selectedBgId.Equals(id, StringComparison.OrdinalIgnoreCase))
		{
			ResetToDefault(saveConfig: true);
		}

		string targetDir = GetBackgroundsDirectory();
		string filePath = Path.Combine(targetDir, item.filename);
		try
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}
		catch
		{
		}

		item.isDownloaded = false;
		item.isDownloading = false;
		item.downloadPercent = 0;
		RefreshDownloadedStatus();
		GameScr.info1?.addInfo("Đã xóa tệp: " + item.name, 0);
	}

	public static void DownloadBackgroundAsync(BackgroundItem item)
	{
		if (item == null || item.isDownloading)
		{
			return;
		}

		item.isDownloading = true;
		item.downloadPercent = 0;
		GameScr.info1?.addInfo("Bắt đầu tải từ Git: " + item.name, 0);

		Thread thread = new Thread(() =>
		{
			string targetDir = GetBackgroundsDirectory();
			string targetFile = Path.Combine(targetDir, item.filename);
			string tempFile = targetFile + ".download.tmp";

			try
			{
				if (File.Exists(tempFile))
				{
					File.Delete(tempFile);
				}

#if NET8_0_OR_GREATER
				using (HttpClient client = new HttpClient())
				{
					client.Timeout = TimeSpan.FromSeconds(30);
					using (HttpResponseMessage response = client.GetAsync(item.url, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult())
					{
						response.EnsureSuccessStatusCode();
						long total = response.Content.Headers.ContentLength ?? 0;
						using (Stream contentStream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
						using (FileStream fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
						{
							byte[] buffer = new byte[8192];
							int read;
							long downloaded = 0;
							while ((read = contentStream.Read(buffer, 0, buffer.Length)) > 0)
							{
								fs.Write(buffer, 0, read);
								downloaded += read;
								if (total > 0)
								{
									item.downloadPercent = (int)((downloaded * 100) / total);
									if (item.downloadPercent > 100) item.downloadPercent = 100;
								}
							}
						}
					}
				}
#else
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(item.url);
				request.Method = "GET";
				request.Timeout = 30000;
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				using (Stream contentStream = response.GetResponseStream())
				using (FileStream fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					long total = response.ContentLength;
					byte[] buffer = new byte[8192];
					int read;
					long downloaded = 0;
					while ((read = contentStream.Read(buffer, 0, buffer.Length)) > 0)
					{
						fs.Write(buffer, 0, read);
						downloaded += read;
						if (total > 0)
						{
							item.downloadPercent = (int)((downloaded * 100) / total);
							if (item.downloadPercent > 100) item.downloadPercent = 100;
						}
					}
				}
#endif

				if (File.Exists(targetFile))
				{
					File.Delete(targetFile);
				}
				File.Move(tempFile, targetFile);

				item.isDownloading = false;
				item.isDownloaded = true;
				item.downloadPercent = 100;
				GameScr.info1?.addInfo("Tải xong: " + item.name + "! Bấm [ÁP DỤNG] để bật.", 0);
			}
			catch (Exception ex)
			{
				item.isDownloading = false;
				item.downloadPercent = 0;
				try { if (File.Exists(tempFile)) File.Delete(tempFile); } catch { }
				GameScr.info1?.addInfo("Tải thất bại: " + ex.Message, 0);
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	public static void PaintCustomBG(mGraphics g)
	{
		if (!isCustomBGActive || currentBGImage == null || currentBGImage.texture == null)
		{
			return;
		}

		try
		{
			int screenW = GameCanvas.w;
			int screenH = GameCanvas.h;

			int imgW = mGraphics.getImageWidth(currentBGImage);
			int imgH = mGraphics.getImageHeight(currentBGImage);
			if (imgW <= 0) imgW = 512;
			if (imgH <= 0) imgH = 256;

			int cameraX = (GameScr.cmx != 0) ? (GameScr.cmx / 4) : 0;
			int startX = -(cameraX % imgW);
			if (startX > 0)
			{
				startX -= imgW;
			}

			int posY = screenH - imgH;
			if (posY < 0)
			{
				int offY = (GameScr.cmy != 0) ? (GameScr.cmy / 10) : 0;
				posY = -offY;
				if (posY > 0) posY = 0;
				if (posY + imgH < screenH) posY = screenH - imgH;
			}
			else if (posY > 0)
			{
				g.setColor(currentTopColor);
				g.fillRect(0, 0, screenW, posY + 1);
			}

			for (int x = startX; x < screenW; x += imgW)
			{
				g.drawImage(currentBGImage, x, posY);
			}
		}
		catch
		{
		}
	}
}

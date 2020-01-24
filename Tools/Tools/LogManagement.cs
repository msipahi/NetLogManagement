using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tools
{
	public class LogManagement
	{
		public string LogUrl = "";
		public string LogFileName = "Varsayilan.Log";
		public int dosyakonum = 3;//1 programfiles 2 logurl, 3 programın olduğu konum

		public static void Kaydet(string mesaj, string dosyaadi = "")
		{
			LogManagement log = new LogManagement();
			if (dosyaadi.Length > 0)
				log.LogFileName = dosyaadi;
			log.DosyaLog(mesaj);
		}

		public LogManagement()
		{
		}
		public LogManagement(int dosyakonum = 3, string LogUrl = "")
		{
			this.dosyakonum = dosyakonum;
			this.LogUrl = LogUrl;
		}


		public void DosyaLog(string mesaj)
		{
			if (dosyakonum == 1)
				ProgramFilesYaz(mesaj);
			else if (dosyakonum == 2)
				LogUrlYaz(mesaj);
			else if (dosyakonum == 3)
				DosyayaYaz(mesaj);
		}
		private void ProgramFilesYaz(string Mesaj)
		{
			try
			{
				string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
				try
				{
					long length = new System.IO.FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\" + LogFileName).Length;
					if (length > 200000) File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\" + LogFileName); // dosya boyutu büyükse sil
				}
				catch
				{

				}


				DosyaYaz(Mesaj, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\" + LogFileName);
			}
			catch (Exception)
			{

			}

		}
		private void LogUrlYaz(string Mesaj)
		{
			DosyaYaz(Mesaj, LogUrl);
		}
		private void DosyayaYaz(string Mesaj)
		{

			string Url = AppDomain.CurrentDomain.BaseDirectory + LogFileName;

			DosyaYaz(Mesaj, Url);

		}
		private void DosyaYaz(string Mesaj, string dosya)
		{

			DosyaYedekleSil(dosya);
			try
			{
				var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
				StreamWriter str = new StreamWriter(dosya, true);
				str.WriteLine("{" + DateTime.Now.ToString() + "}  : [" + Process.GetCurrentProcess().Id.ToString() + "]" + fvi.FileVersion + " :" + Mesaj);
				str.Close();
			}
			catch (Exception)
			{ }
		}

		private void DosyaYedekleSil(string dosya)
		{
			

			try
			{
				long length = new System.IO.FileInfo(dosya).Length;
				long LogFileMaxSize = 102400;
				string a = "102400"; //todo: log dosyalarının maksimum boyutunu reg/ini file vb. oku
				bool LogSakla = false;
				string b = "1";//todo: Silinen logların başka bir dizinde saklanıp saklanmayacaığını reg/ini file vb. oku  
				if (a != "-1")
				{
					try
					{
						LogFileMaxSize = Convert.ToInt64(a);
					}
					catch (Exception)
					{
					}
				}
				if (b != "-1")
				{
					try
					{
						if ((b == "1") || (b == "true"))
							LogSakla = true;
					}
					catch (Exception)
					{
					}
				}

				if (length > LogFileMaxSize)
				{

					if (!LogSakla)
						System.IO.File.Delete(dosya);
					else
					{
						string yedekklasoru = Path.GetDirectoryName(dosya) + @"\LogEski";
						try
						{
							if (!Directory.Exists(yedekklasoru))
							{
								Directory.CreateDirectory(yedekklasoru);
							}
							if (!File.Exists(dosya + "-" + DateTime.Today))
								File.Move(dosya, dosya + "-" + DateTime.Today);
							else
								File.Move(dosya, dosya + "-" + DateTime.Now + "-" + DateTime.Now.Second);
						}
						catch (Exception)
						{
						}
					}
				}
			}
			catch (Exception)
			{ }






		}
	}

}

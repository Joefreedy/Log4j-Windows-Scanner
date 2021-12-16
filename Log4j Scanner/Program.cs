using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Log4j
{
	// Token: 0x02000002 RID: 2
	internal class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static void Main(string[] args)
		{
			Program._willNotBeReviewedFolders = new List<string>
			{
				"C:\\Windows".ToLower(Program.CultureInfo)
			};
			Program._tasks = new List<Task>();
			Program._log4jFiles = new List<string>();
			Task item = Task.Factory.StartNew(delegate()
			{
				Program.Examine("C:\\");
			});
			Program._tasks.Add(item);
			do
			{
			}
			while (!Program._tasks.TrueForAll((Task t) => t != null && t.IsCompleted));
			Console.WriteLine("Found Files:");
			Program.GetFiles(Program._log4jFiles);
			Console.WriteLine("------");
			Console.ReadLine();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002124 File Offset: 0x00000324
		private static void GetFiles(IEnumerable<string> paths)
		{
			foreach (string str in from path in paths
			where Program.GetVersion(Path.GetFileName(path)) < Program._version
			select path)
			{
				Console.WriteLine("Found : " + str);
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000021A0 File Offset: 0x000003A0
		private static void LogFilesPath(string path)
		{
			object lockObject = Program._lockObject;
			lock (lockObject)
			{
				File.AppendAllText(Program._desktopPath + "\\vulnerability.txt", path + "\n");
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002200 File Offset: 0x00000400
		private static void LogJarPath(string path)
		{
			object lockObject = Program._lockObject1;
			lock (lockObject)
			{
				File.AppendAllText(Program._desktopPath + "\\vulnerability2.txt", path + "\n");
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002260 File Offset: 0x00000460
		private static void Log(string log)
		{
			object lockObject = Program._lockObject2;
			lock (lockObject)
			{
				File.AppendAllText(Program._desktopPath + "\\log.txt", log + "\n");
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000022C0 File Offset: 0x000004C0
		private static void LogScannedDir(string path, bool status)
		{
			object lockObject = Program._lockObject3;
			lock (lockObject)
			{
				if (status)
				{
					File.AppendAllText(Program._desktopPath + "\\scannedDirectorys.txt", path + "\n");
				}
				else
				{
					File.AppendAllText(Program._desktopPath + "\\scannedDirectorys2.txt", path + "\n");
				}
				File.AppendAllText(Program._desktopPath + "\\scannedDirectorysAll.txt", path + "\n");
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000236C File Offset: 0x0000056C
		private static Version GetVersion(string fileName)
		{
			Match match = Regex.Match(fileName, "\\d+(\\.\\d+)+");
			bool flag = !match.Success;
			Version result;
			if (flag)
			{
				result = new Version();
			}
			else
			{
				Version version;
				try
				{
					version = Version.Parse(match.Value);
				}
				catch (Exception ex)
				{
					version = new Version();
				}
				result = version;
			}
			return result;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000023D0 File Offset: 0x000005D0
		private static void FileExamine(string path)
		{
			try
			{
				string[] files = Directory.GetFiles(path, "*.jar");
				for (int i = 0; i < files.Length; i++)
				{
					string file = files[i];
					bool flag = !Path.GetFileName(file).StartsWith("log4j");
					if (!flag)
					{
						Program._log4jFiles.Add(file);
						Console.WriteLine(file);
						Version version = Program.GetVersion(Path.GetFileName(file));
						bool flag2 = !(Program.GetVersion(Path.GetFileName(file)) < Program._version);
						if (!flag2)
						{
							Task task = Task.Factory.StartNew(delegate()
							{
								Program.LogFilesPath(file);
							});
							Task task2 = Task.Factory.StartNew(delegate()
							{
								Program.JarExamine(file);
							});
							Program.TasksAdd(new Task[]
							{
								task,
								task2
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				Program.Log(ex.Message);
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000024F4 File Offset: 0x000006F4
		private static void JarExamine(string path)
		{
			try
			{
				using (ZipArchive zipArchive = ZipFile.Open(path, ZipArchiveMode.Update))
				{
					bool flag = zipArchive.Entries.Any((ZipArchiveEntry a) => a.FullName.Contains("JndiLookup.class"));
					if (flag)
					{
						Program.LogJarPath(path);
					}
				}
			}
			catch (Exception ex)
			{
				Program.Log(ex.Message);
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002584 File Offset: 0x00000784
		private static void DirectoryExamine(string path)
		{
			bool status = true;
			try
			{
				string[] directories = Directory.GetDirectories(path);
				for (int i = 0; i < directories.Length; i++)
				{
					string directory = directories[i];
					Task task = Task.Factory.StartNew(delegate()
					{
						Program.Examine(directory);
					});
					Program.TasksAdd(task);
				}
			}
			catch (Exception ex)
			{
				Program.Log(ex.Message);
				status = false;
			}
			finally
			{
				Task task2 = Task.Factory.StartNew(delegate()
				{
					Program.LogScannedDir(path, status);
				});
				Program.TasksAdd(task2);
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002654 File Offset: 0x00000854
		private static void Examine(string path)
		{
			bool flag = Program._willNotBeReviewedFolders.Contains(path.ToLower(Program.CultureInfo));
			if (!flag)
			{
				Task task = Task.Factory.StartNew(delegate()
				{
					Program.FileExamine(path);
				});
				Task task2 = Task.Factory.StartNew(delegate()
				{
					Program.DirectoryExamine(path);
				});
				Program.TasksAdd(new Task[]
				{
					task,
					task2
				});
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000026D4 File Offset: 0x000008D4
		private static void TasksAdd(Task task)
		{
			object lockObjectTask = Program._lockObjectTask;
			lock (lockObjectTask)
			{
				Program._tasks.Add(task);
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002720 File Offset: 0x00000920
		private static void TasksAdd(params Task[] tasks)
		{
			object lockObjectTask = Program._lockObjectTask;
			lock (lockObjectTask)
			{
				Program._tasks.AddRange(tasks);
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x0000276C File Offset: 0x0000096C
		public Program()
		{
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002778 File Offset: 0x00000978
		// Note: this type is marked as 'beforefieldinit'.
		static Program()
		{
		}

		// Token: 0x04000001 RID: 1
		private static List<string> _log4jFiles;

		// Token: 0x04000002 RID: 2
		private static List<Task> _tasks;

		// Token: 0x04000003 RID: 3
		private static List<string> _willNotBeReviewedFolders;

		// Token: 0x04000004 RID: 4
		private static readonly Version _version = new Version("2.15");

		// Token: 0x04000005 RID: 5
		private static readonly string _desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

		// Token: 0x04000006 RID: 6
		private static readonly object _lockObject = new object();

		// Token: 0x04000007 RID: 7
		private static readonly object _lockObject1 = new object();

		// Token: 0x04000008 RID: 8
		private static readonly object _lockObject2 = new object();

		// Token: 0x04000009 RID: 9
		private static readonly object _lockObject3 = new object();

		// Token: 0x0400000A RID: 10
		private static readonly object _lockObjectTask = new object();

		// Token: 0x0400000B RID: 11
		private static readonly CultureInfo CultureInfo = new CultureInfo("en-us");
	}
}
/*
 * リソース生成 ResGen
 * メインクラス
 */

using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Resources;


namespace ResxGener {

	class Program {

		private static string _errmsg = string.Empty;

		static void Main(string[] args) {

			string asmName = GetAssemblyName();
			string asmVersion = GetAssemblyVersion();
			Console.WriteLine(asmName + " " + asmVersion);

			if (args.Length == 0) {
				ShowUsage(asmName);
			} else {
				// 指定されている.rcファイルごとに.resourcesを作成する
				foreach (string arg in args) {
                    string filepath = arg;
					if (!ArgumentValidity(filepath)) {
						Console.WriteLine(_errmsg);
						continue;
					}
					ResourceGenerator rgen = new ResourceGenerator(filepath);
				}
			}
		}

		/// <summary>
		/// Usageをコマンドラインに表示する
		/// </summary>
		/// <param name="asmName"></param>
		private static void ShowUsage(string asmName) {
			string[] messages = {
				"C#プログラムに埋め込み可能なリソースを生成します。",
				asmName + " [ドライブ:][パス]ファイル名.rc",
				"<書式>",
				"STRING  resid  \"文字列\"  String型",
				"TEXT    resid  \"パス\"    String型",
				"BITMAP  resid  \"パス\"    Bitmap型",
				"CURSOR  resid  \"パス\"    Bitmap型",
				"ICON    resid  \"パス\"      Icon型",
				"any     resid  \"パス\"      Byte型",
				"# コメント"
			};
			foreach (string message in messages) {
				Console.WriteLine(message);
			}
		}

		/// <summary>
		/// アッセンブリー情報から名称を返却する
		/// </summary>
		/// <returns>名称を返却する</returns>
		private static string GetAssemblyName() {
			Assembly assembly = Assembly.GetExecutingAssembly();
			return assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
		}

		/// <summary>
		/// アッセンブリー情報からバージョン情報を返却する
		/// </summary>
		/// <returns>バージョン情報を返却する</returns>
		private static string GetAssemblyVersion() {
			AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
			return assemblyName.Version.ToString();
		}

		/// <summary>
		/// 引数の妥当性チェック
		/// </summary>
		/// <param name="arg"></param>
		/// <returns>エラーなし:true、エラーあり:false</returns>
		private static bool ArgumentValidity(string arg) {

			// ファイルが存在していること
			if (!File.Exists(arg)) {
				_errmsg = "ファイルが存在しません。\"" + arg + "\"";
				return false;
			}
			// 拡張子が".rc"であること
			if (!Path.GetExtension(arg).ToLower().Equals(".rc")) {
				_errmsg = "拡張子が\".rc\"になっていません。";
				return false;
			}
			return true;
		}
	}

    /// <summary>
    /// リソース生成クラス
    /// </summary>
    public class ResourceGenerator {

        private string _filePath = string.Empty;
		private string _outputFilePath = string.Empty;

		public struct ResType {
			public string Type;
			public string Name;
			public string Value;
		}
		private ArrayList res_list = new ArrayList();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="filePath"></param>
		public ResourceGenerator(string filePath) {

            this._filePath = filePath;

            // 出力ファイルパスの編集
			string fullpath = Path.GetFullPath(this._filePath);
			string directotyName = Path.GetDirectoryName(fullpath);
			string filename = Path.GetFileNameWithoutExtension(this._filePath) + ".resx";
			_outputFilePath = Path.Combine(directotyName, filename);

			PreValidation();
			if (res_list.Count == 0) {
				Console.WriteLine("No resouces.");
				return;
			}
			Generate();
	    }

		/// <summary>
		/// 事前検証
		/// </summary>
		private void PreValidation() {

			string[] lines = File.ReadAllLines(this._filePath, Encoding.GetEncoding("utf-8"));

			Console.WriteLine("------------------------------");
			foreach(string line in lines) {
				Console.WriteLine(line);
				if (line.Length == 0 || line[0] == '#' || line[0] == ';' || line[0] == ' ')
					continue;
				ResType token = LexicalAnalysis(line);
				if (token.Type.Equals(string.Empty)) {
					Console.WriteLine("error");
					continue;
				}
				res_list.Add(token);
			}
		}
		
		/// <summary>
		/// 字句解析
		/// </summary>
		/// <param name="line"></param>
		/// <returns>登録リソースのタイプ、ID、値を返却</returns>
		public ResType LexicalAnalysis(string line) {
			
			string pattern = @"([A-Za-z][A-Za-z0-9]{0,})[ \t]{1,}([A-Za-z][A-Za-z0-9]{1,})[ \t]{1,}""([^""]*)""";

			ResType restype = new ResType();
			restype.Type = string.Empty;
			restype.Name = string.Empty;
			restype.Value = string.Empty;

			MatchCollection matches = Regex.Matches(line, pattern);
            foreach(Match match in matches) {
				restype.Type = match.Groups[1].ToString().ToUpper();
				restype.Name = match.Groups[2].ToString();
				restype.Value = match.Groups[3].ToString();
			}

			if (!restype.Type.Equals("STRING")) {
				string fullpath = Path.GetFullPath(restype.Value);
				if (!File.Exists(fullpath)) {
					Console.WriteLine("error");
					restype.Type = string.Empty;
					restype.Name = string.Empty;
					restype.Value = string.Empty;
				}
			}
			return restype;
		}

		/// <summary>
		/// リソースファイル生成
		/// </summary>
		public void Generate() {

			using (ResXResourceWriter writer = new ResXResourceWriter(this._outputFilePath)) {
				foreach (ResType restype in res_list) {
					if (restype.Type.Equals("STRING")) {
						writer.AddResource(restype.Name, restype.Value);
					} else if (restype.Type.Equals("BITMAP") || restype.Type.Equals("CURSOR")) {
						Bitmap bmp = new Bitmap(restype.Value);
						writer.AddResource(restype.Name, bmp);
					} else if (restype.Type.Equals("ICON")) {
						Icon icon = new Icon(restype.Value);
						writer.AddResource(restype.Name, icon);
					} else if (restype.Type.Equals("TEXT")) {
						string text = File.ReadAllText(restype.Value);
						writer.AddResource(restype.Name, text);
					} else {
						Byte[] bin = File.ReadAllBytes(restype.Value);
						writer.AddResource(restype.Name, bin);
					}
				}
			}
			Console.WriteLine("------------------------------");
			using (ResXResourceReader reader = new ResXResourceReader(this._outputFilePath)) {
				IDictionaryEnumerator dict = reader.GetEnumerator();
				while (dict.MoveNext())
					Console.WriteLine(" {0}: '{1}' (Type {2})", dict.Key, dict.Value, dict.Value.GetType().Name);
			}
		}

	}

}

﻿using System.IO;

namespace MonoDesign.Core.File {
	public class FileService : IFileService {
		public void SaveToFile(byte[] bytes, string path) {
			System.IO.File.WriteAllBytes(path, bytes);
		}
		public byte[] ReadWithFile(string path) {
			return System.IO.File.ReadAllBytes(path);
		}
		public bool Exists(string path) {
			return Directory.Exists(path) || System.IO.File.Exists(path);
		}
		public string GetDirectory(string path) {
			return new FileInfo(path).Directory?.FullName;
		}
		public void CreateDirectory(string directory) {
			Directory.CreateDirectory(directory);
		}
		public void RemoveDirectory(string directory) {
			Directory.Delete(directory, true);
		}
		public string CombinePath(params string[] paths) {
			return Path.Combine(paths);
		}
		public void CopyFile(string source, string path) {
			var directory = GetDirectory(path);
			if (!Exists(directory)) {
				CreateDirectory(directory);
			}
			System.IO.File.Copy(source, path, true);
		}
		public string GetFileName(string path) {
			return Path.GetFileName(path);
		}
		public string GetExtension(string path) {
			return Path.GetExtension(path);
		}
	}
}
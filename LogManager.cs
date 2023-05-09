using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LogManager
{
#if RELEASE
	public static bool IGNORE_LOG = true; // 로그 출력 무시 옵션
	public static bool IGNORE_SAVEOPTION = true; // 로그 저장 강제 무시 옵션
#else
	public static bool IGNORE_LOG = false; // 로그 출력 무시 옵션
	public static bool IGNORE_SAVEOPTION = false; // 로그 저장 강제 무시 옵션
#endif
	private static int log_Index = 0;
	#region [ ENUM ]
	public enum LogType
	{
		Normal,
		Warning,
		Error,
	}
	public enum SaveOption
	{
		None,
		Save,
	}
	#endregion
	/// <summary>
	/// 커스텀 로그 메세지 출력 및 저장
	/// </summary>
	/// <param name="argMessage">로그 메세지</param>
	/// <param name="logType">로그 타입</param>
	/// <param name="saveOption">저장 여부</param>
	/// <param name="_lineNumber">호출 라인</param>
	/// <param name="_callerName">호출 멤버 이름</param>
	public static void Print(string argMessage, LogType logType = LogType.Normal, SaveOption saveOption = SaveOption.None, [CallerLineNumber] int _lineNumber = 0, [CallerMemberName] string _callerName = "EMPTY", [CallerFilePath] string _filePath = "EMPTY")
	{
		if (IGNORE_LOG == true) return;
		// 로그 타입에 따라 메세지 출력
		switch(logType)
		{
			case LogType.Normal: Debug.Log($"<{log_Index}> : {argMessage}"); break;
			case LogType.Warning: Debug.LogWarning($"<{log_Index}> : {argMessage}"); break;
			case LogType.Error: Debug.LogError($"<{log_Index}> : {argMessage}"); break;
		}
		log_Index++;
		// 로그 저장 강제 무시 옵션
		if (IGNORE_SAVEOPTION == true) return;
		// 로그 저장 여부
		switch(saveOption)
		{
			case SaveOption.None: break;
			case SaveOption.Save:
				{
					SaveLog(argMessage, _lineNumber, _callerName, _filePath);
					break;
				}
		}
	}
	private static StreamWriter _writer = null;
	//private static string _savePath = $"{Application.dataPath}\\CustomLog"; // 로그 파일 저장 경로
	private static string _savePath = "C:\\DragonGate\\Log"; // 로그 파일 저장 경로
	private static int _logNumber = 0; // 로그 인덱싱
	/// <summary>
	/// 로그 저장 함수
	/// </summary>
	/// <param name="argMessage">로그 메세지</param>
	/// <param name="argLineNumber">호출 라인 넘버</param>
	/// <param name="argMemberName">호출 멤버 이름</param>
	private static void SaveLog(string argMessage, int argLineNumber, string argMemberName, string argFilePath)
	{
		if (_writer == null) return;
		_writer.WriteLine($"[{_logNumber}] : {argMessage} - [{System.DateTime.Now:HH:mm:ss}]");
		_writer.WriteLine($"\t+ Caller : {argFilePath} >> {argMemberName}() >> {argLineNumber} line.");
		_writer.WriteLine();
		_logNumber++;
	}
	/// <summary>
	/// 로그 파일 생성 및 열기
	/// </summary>
	public static void OpenLogFile()
	{
		if (_writer == null)
		{
			if (_savePath == null || _savePath.Length == 0)
			{
				Debug.LogError($"[LogManager] : Save path is NULL. Cannot save the log.");
				return;
			}
			if (Directory.Exists(_savePath) == false)
			{
				Directory.CreateDirectory(_savePath);
			}
			string fileName = $"CustomLogManager_{DateTime.Now:yyyy/MM/dd_HH-mm-ss}.txt";
			_savePath = $"{_savePath}\\{fileName}";

			_writer = new StreamWriter(_savePath);
			if (_writer == null)
			{
				Debug.LogError($"[LogManager] : Stream Writer Open failed. Cannot save the log.");
				return;
			}

			_writer.WriteLine($"** CREATED BY LOG MANAGER **\t\t<< {System.DateTime.Now:yyyy-MM/dd ddd HH:mm:ss} >>");
			_writer.WriteLine();
			_writer.WriteLine("---------------------------------------------------------------------------");
			_writer.WriteLine();

			Debug.Log("[LogManager] : Log file writing is started.");
		}
	}
	/// <summary>
	/// 로그 파일 작성 종료
	/// </summary>
	public static void CloseLogFile()
	{
		if (_writer != null)
		{
			_writer.WriteLine();
			_writer.WriteLine("---------------------------------------------------------------------------");
			_writer.WriteLine();
			_writer.WriteLine($"** END LOG **\t\tPlayTime : [{Time.time}]");

			_writer.Close();
			Debug.Log("[LogManager] : Log file writer is closed.");
			_writer = null;
		}
	}
}

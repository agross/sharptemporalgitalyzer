﻿using System.IO;

namespace Seabites.SharpTemporalGitalyzer {
  public class CSharpMethod {
    readonly string _fullmethodname;
    readonly CSharpMethodBodyStatistics _bodyStatistics;
    readonly string _file;
    readonly Commit _commit;

    public CSharpMethod(string fullmethodname, CSharpMethodBodyStatistics bodyStatistics, string file, Commit commit) {
      _fullmethodname = fullmethodname;
      _bodyStatistics = bodyStatistics;
      _file = file;
      _commit = commit;
    }

    public void WriteAsCsv(TextWriter writer) {
      writer.WriteLine(
        "{0},{1},{2},{3},{4},{5},{6},{7}",
        _fullmethodname, _bodyStatistics.Length, _bodyStatistics.LineCount, _bodyStatistics.Hash, _file, _commit.Hash, _commit.Committer, _commit.CommitDate);
    }

    public static void WriteCsvHeader(TextWriter writer) {
      writer.WriteLine(
        "FullMethodName,MethodBodyLength,MethodBodyLineCount,MethodBodyHash,FilePath,CommitHash,Committer,CommitDate");
    }
  }
}
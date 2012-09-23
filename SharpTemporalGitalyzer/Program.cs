﻿using System;
using System.Configuration;
using System.IO;

using ICSharpCode.NRefactory.CSharp;

namespace Seabites.SharpTemporalGitalyzer {
  class Program {
    static void Main(string[] args) {
      var gitPath = ConfigurationManager.AppSettings["GitPath"];
      var gitCommitIterator = new GitCommitIterator(gitPath);
      var gitWorkingCopyCheckout = new GitWorkingCopyCheckout(gitPath);
      var workingCopyPath = args[0];
      var analysisOutputPath = Path.Combine(Environment.CurrentDirectory, "analysis");
      EnsureCleanAnalysisOutput(analysisOutputPath);
      var parser = new CSharpParser();
      Commit commitToRestoreAfterAnalysis = null;
      try
      {
        commitToRestoreAfterAnalysis = gitCommitIterator.GetHeadCommit(workingCopyPath);
        foreach (var commit in gitCommitIterator.GetCommits(workingCopyPath))
        {
          Console.WriteLine(commit);

          gitWorkingCopyCheckout.CheckoutCommit(workingCopyPath, commit);
        
          using (var output = File.OpenWrite(
            Path.Combine(
              analysisOutputPath,
              commit.AnalysisOutputFileName))) {
            using (var csvWriter = new StreamWriter(output)) {
              CSharpMethod.WriteCsvHeader(csvWriter);
              foreach (var file in CSharpFile.Enumerate(workingCopyPath, commit)) {
                foreach (var method in file.AnalyzeMethods(parser)) {
                  method.WriteAsCsv(csvWriter);
                }
              }
              csvWriter.Flush();
            }
          }
        }
      }
      finally
      {
        if(commitToRestoreAfterAnalysis != null)
          gitWorkingCopyCheckout.CheckoutCommit(workingCopyPath, commitToRestoreAfterAnalysis);
      }
      Console.WriteLine("Yeah, I'm done.");
      Console.ReadLine();
    }

    static void EnsureCleanAnalysisOutput(string analysisOutputPath) {
      if(Directory.Exists(analysisOutputPath)) {
        Directory.Delete(analysisOutputPath, true);
      }
      Directory.CreateDirectory(analysisOutputPath);
    }
  }
}

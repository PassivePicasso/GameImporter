﻿using System;
using UnityEditor;
using uTinyRipper;

namespace PassivePicasso.GameImporter
{
    public class ProgressBarLogger : ILogger, IDisposable
    {
        public ProgressBarLogger()
        {
            EditorUtility.DisplayProgressBar("Analyzing Game", $"", 0);
        }

        public void Dispose()
        {
            EditorUtility.ClearProgressBar();
        }

        public void Log(LogType type, LogCategory category, string message, float progress = 0)
        {
            switch (type)
            {
                case LogType.Error:
                    throw new Exception(message);
                default:
                    EditorUtility.DisplayProgressBar("Analyzing Game", message, progress);
                    break;
            }
        }
    }
}
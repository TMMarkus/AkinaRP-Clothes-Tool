﻿namespace AkinaRPTool
{
    class StatusController
    {

        public static void SetStatus(string status)
        {
            MainWindow.SetStatus(status);
        }

        public static void SetProgress(double progress)
        {
            MainWindow.SetProgress(progress);
        }
    }
}

﻿using System;
using System.Text;
using ProjectConfiguration;
using System.Windows.Forms;
using System.Drawing;

namespace Logging
{
    /// <summary>
    /// Manages where to write logs.
    /// </summary>
    public class LogManager
    {
        #region Logging Colors
        static Color errorColor = Color.FromArgb(232, 76, 78);
        static Color warninfColor = Color.FromArgb(243, 238, 119);
        static Color wrongDataColor = Color.FromArgb(230, 134, 117);
        static Color appearanceColor = Color.FromArgb(103, 220, 103);

        #endregion
        public static DataGridView LogSource = null;
        public static Form form = null;
        public static PictureBox loading = null;
        private static int count = 0;

        public static object block = new object();

        /// <summary>
        /// Manages writing of logs in Text file or in Windows Event Log.
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public static void DoLogging(LogType logType, Exception ex = null, string message = null)
        {
            AppConfiguration appConfig = AppConfiguration.GetInstance;

            bool isTxt;
            ILogger log = null;

            try
            {
                isTxt = appConfig.IsStoreTxtFile;
            }
            catch
            {
                isTxt = false;
            }

            if (isTxt)
            {
                log = new TxtFileLogger();
            }
            else
            {
                log = new WindowsEventLogger();
            }

            if (LogSource != null)
            {
                form.Invoke(
                    new Action(
                        () =>
                        {
                            LogVisibleInWindow(logType, ex, message);
                        }));
            }

            log.Log(logType, ex, message);
        }

        /// <summary>
        /// RealTime logging in UIMainForm.
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        private static void LogVisibleInWindow(LogType logType, Exception exception = null, string message = null)
        {
            StringBuilder builder = new StringBuilder();

            lock (block)
            {
                int index = LogSource.Rows.Add();

                Color color = Color.White;
                switch (logType)
                {
                    case LogType.Error:
                        {
                            color = errorColor;
                            break;
                        }
                    case LogType.Warning:
                        {
                            color = warninfColor;
                            break;
                        }
                    case LogType.Appearance:
                        {
                            count++;
                            color = appearanceColor;
                            break;
                        }
                    case LogType.WrongData:
                        {

                            color = wrongDataColor;
                            break;
                        }
                    case LogType.Delete:
                        {
                            count--;
                            break;
                        }
                    case LogType.Success:
                        {
                           
                            break;
                        }
                    default:
                        break;
                }

                if (count > 0)
                    loading.Visible = true;
                else
                    loading.Visible = false;

                LogSource.Rows[index].DefaultCellStyle.BackColor = color;

                LogSource.Rows[index].Cells[0].ValueType = typeof(LogType);
                LogSource.Rows[index].Cells[0].Value = logType.ToString();

                LogSource.Rows[index].Cells[1].ValueType = typeof(DateTime);
                LogSource.Rows[index].Cells[1].Value = DateTime.Now;

                LogSource.Rows[index].Cells[2].ValueType = typeof(Exception);
                if (exception != null)
                {
                    builder.AppendLine("Namespace: " + exception.Source);
                    builder.AppendLine("Class Name: " + exception.TargetSite.ReflectedType.Name);
                    builder.AppendLine("Line: " + exception.StackTrace.Substring(exception.StackTrace.LastIndexOf(":line") + 5));
                    builder.AppendLine("Discription: " + exception.Message);
                }
                LogSource.Rows[index].Cells[2].Value = builder.ToString();

                LogSource.Rows[index].Cells[3].ValueType = typeof(string);
                LogSource.Rows[index].Cells[3].Value = message;
                if (index > 0)
                {
                    LogSource.Rows[index - 1].Selected = false;
                }
                    LogSource.Rows[index].Selected = true;
                LogSource.FirstDisplayedScrollingRowIndex = index;
            }

        }
    }
}

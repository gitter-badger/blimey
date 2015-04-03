// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey Engine (http://www.blimey.io)         │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors:                                                               │ \\
// │ ~ Ash Pook (http://www.ajpook.com)                                     │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining  │ \\
// │ a copy of this software and associated documentation files (the        │ \\
// │ "Software"), to deal in the Software without restriction, including    │ \\
// │ without limitation the rights to use, copy, modify, merge, publish,    │ \\
// │ distribute, sublicense, and/or sellcopies of the Software, and to      │ \\
// │ permit persons to whom the Software is furnished to do so, subject to  │ \\
// │ the following conditions:                                              │ \\
// │                                                                        │ \\
// │ The above copyright notice and this permission notice shall be         │ \\
// │ included in all copies or substantial portions of the Software.        │ \\
// │                                                                        │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        │ \\
// │ EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     │ \\
// │ MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. │ \\
// │ IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY   │ \\
// │ CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   │ \\
// │ TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE       │ \\
// │ SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 │ \\
// └────────────────────────────────────────────────────────────────────────┘ \\

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.IO;
    using System.Reflection;
    using Abacus.SinglePrecision;
    using Fudge;
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class LogManager
    {
        public delegate void WriteLogDelegate (
            String assembly,
            String tag,
            String channel,
            String type,
            String time,
            String[] lines);

        public void Debug (String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Debug, assembly, line);
        }

        public void Debug (String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Debug, assembly, line, args);
        }

        public void Debug (String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Debug, assembly, channel, line, args);
        }

        public void Debug (String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Debug, assembly, channel, line);
        }

        public void Info (String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Info, assembly, line);
        }

        public void Info (String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Info, assembly, line, args);
        }

        public void Info (String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Info, assembly, channel, line, args);
        }

        public void Info (String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Info, assembly, channel, line);
        }

        public void Warning (String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Warning, assembly, line);
        }

        public void Warning (String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Warning, assembly, line, args);
        }

        public void Warning (String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Warning, assembly, channel, line, args);
        }

        public void Warning (String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Warning, assembly, channel, line);
        }

        public void Error (String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Error, assembly, line);
        }

        public void Error (String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Error, assembly, line, args);
        }

        public void Error (String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Error, assembly, channel, line, args);
        }

        public void Error (String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Error, assembly, channel, line);
        }


        enum LogType
        {
            Debug,
            Info,
            Warning,
            Error,
        }

        readonly LogManagerSettings settings;

        internal LogManager (LogManagerSettings settings)
        {
            this.settings = settings;
        }

        // This should be user customisable
        void DoWriteLog (
            String assembly,
            String tag,
            String channel,
            String type,
            String time,
            String[] lines)
        {
            foreach (var writeLogFn in settings.LogWriters)
            {
                writeLogFn (assembly, tag, channel, type, time, lines);
            }
        }

        void WriteLine (LogType type, Assembly callingAssembly, String line)
        {
            WriteLine (type, callingAssembly, "Default", line);
        }


        void WriteLine (LogType type, Assembly callingAssembly, String line, params object[] args)
        {
            WriteLine (type, callingAssembly, "Default", line, args);
        }

        void WriteLine (LogType type, Assembly callingAssembly, String channel, String line, params object[] args)
        {
            String main = String.Format (line, args);

            WriteLine (type, callingAssembly, channel, main);
        }

        void WriteLine (LogType type, Assembly callingAssembly, String channel, String line)
        {
            if (settings.UseLogChannels &&
                !settings.EnabledLogChannels.Contains (channel))
            {
                return;
            }

            if (String.IsNullOrWhiteSpace (line))
            {
                return;
            }

            String assembyStr = Path.GetFileNameWithoutExtension (callingAssembly.Location);
            String typeStr = type.ToString ().ToUpper ();
            String timeStr = DateTime.Now.ToString ("HH:mm:ss.ffffff");
            String[] lines = line.Split (Environment.NewLine.ToCharArray ())
                .Where (x => !String.IsNullOrWhiteSpace (x))
                .ToArray ();

            DoWriteLog (assembyStr, settings.Tag, channel, typeStr, timeStr, lines);
        }
    }
}
// Copyright 2025 Takuto Nakamura
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using Microsoft.Win32;

namespace RunCat365
{
    internal sealed class LaunchAtStartupManager
    {
        private const string KEY_PATH = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string VALUE_NAME = "RunCat Local";

        internal bool GetStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(KEY_PATH);
            return string.Equals(key?.GetValue(VALUE_NAME) as string, GetCommand(), StringComparison.OrdinalIgnoreCase);
        }

        internal bool ToggleStartup(bool currentlyEnabled)
        {
            using var key = Registry.CurrentUser.CreateSubKey(KEY_PATH, true);
            if (currentlyEnabled)
            {
                key.DeleteValue(VALUE_NAME, false);
            }
            else
            {
                key.SetValue(VALUE_NAME, GetCommand());
            }
            return true;
        }

        private static string GetCommand()
        {
            var path = Environment.ProcessPath ?? throw new InvalidOperationException("Executable path is unavailable.");
            return "\"" + path + "\"";
        }
    }
}

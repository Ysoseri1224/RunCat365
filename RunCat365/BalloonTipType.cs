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

using RunCat365.Properties;

namespace RunCat365
{
    internal readonly struct BalloonTipInfo
    {
        internal string Title { get; }
        internal string Text { get; }
        internal ToolTipIcon Icon { get; }

        internal BalloonTipInfo(string title, string text, ToolTipIcon icon)
        {
            Title = title;
            Text = text;
            Icon = icon;
        }
    }

    internal enum BalloonTipType
    {
        AppLaunched,
        CPUInfoUnavailable,
    }

    internal static class BalloonTipTypeExtension
    {
        internal static BalloonTipInfo GetInfo(this BalloonTipType balloonTipType)
        {
            return balloonTipType switch
            {
                BalloonTipType.AppLaunched => new("RunCat 365", Strings.Message_AppLaunched, ToolTipIcon.Info),
                BalloonTipType.CPUInfoUnavailable => new(Strings.Message_Warning, Strings.Message_CPUUsageUnavailable, ToolTipIcon.Warning),
                _ => new("RunCat 365", string.Empty, ToolTipIcon.None),
            };
        }
    }
}

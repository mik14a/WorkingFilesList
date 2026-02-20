// Working Files List
// Visual Studio extension tool window that shows a selectable list of files
// that are open in the editor
// Copyright © 2016 Anthony Fung

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace WorkingFilesList.Core.Model
{
    /// <summary>
    /// Type of metric indicator to display for each file in the list
    /// </summary>
    public enum MetricIndicatorType
    {
        /// <summary>
        /// No metric indicator (empty bar)
        /// </summary>
        None,

        /// <summary>
        /// Full bar for all items
        /// </summary>
        Full,

        /// <summary>
        /// Display usage order metric based on when files were activated
        /// </summary>
        UsageOrder,

        /// <summary>
        /// Display line count metric based on file line count
        /// </summary>
        LineCount
    }
}

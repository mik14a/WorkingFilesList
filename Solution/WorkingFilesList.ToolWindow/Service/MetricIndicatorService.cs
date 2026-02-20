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

using System.Collections.Generic;
using System.Linq;
using WorkingFilesList.Core.Interface;
using WorkingFilesList.Core.Model;
using WorkingFilesList.ToolWindow.Interface;

namespace WorkingFilesList.ToolWindow.Service
{
    public class MetricIndicatorService : IMetricIndicatorService
    {
        public MetricIndicatorService()
        {
        }

        /// <summary>
        /// Sets <see cref="DocumentMetadata.MetricIndicator"/> according to the
        /// selected <see cref="MetricIndicatorType"/> in user preferences
        /// </summary>
        /// <param name="metadataCollection">
        /// Collection over which to establish metric indicator values
        /// </param>
        /// <param name="userPreferences">
        /// <see cref="IUserPreferences"/> instance that contains the selected
        /// metric indicator type
        /// </param>
        public void SetMetricIndicator(
            IList<DocumentMetadata> metadataCollection,
            IUserPreferences userPreferences)
        {
            switch (userPreferences.MetricIndicatorType)
            {
                case MetricIndicatorType.None:
                    foreach (var metadata in metadataCollection)
                    {
                        metadata.MetricIndicator = 0;
                    }
                    break;

                case MetricIndicatorType.Full:
                    foreach (var metadata in metadataCollection)
                    {
                        metadata.MetricIndicator = 1;
                    }
                    break;

                case MetricIndicatorType.UsageOrder:
                    var interval = 1 / (double)metadataCollection.Count;
                    var sortedCollection = metadataCollection.OrderBy(m => m.ActivatedAt);

                    var counter = 0;
                    foreach (var metadata in sortedCollection)
                    {
                        counter++;
                        metadata.MetricIndicator = counter * interval;
                    }
                    break;

                case MetricIndicatorType.LineCount:
                    if (metadataCollection.Count == 0)
                    {
                        break;
                    }

                    var maxLines = metadataCollection.Max(m => m.Lines);
                    if (maxLines == 0)
                    {
                        // If all files have 0 lines, set all indicators to 1
                        foreach (var metadata in metadataCollection)
                        {
                            metadata.MetricIndicator = 1;
                        }
                    }
                    else
                    {
                        foreach (var metadata in metadataCollection)
                        {
                            var normalizedValue = (double)metadata.Lines / maxLines;
                            metadata.MetricIndicator = normalizedValue;
                        }
                    }
                    break;
            }
        }
    }
}

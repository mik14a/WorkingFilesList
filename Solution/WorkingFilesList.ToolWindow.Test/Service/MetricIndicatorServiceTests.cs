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

using Moq;
using NUnit.Framework;
using System;
using WorkingFilesList.Core.Interface;
using WorkingFilesList.Core.Model;
using WorkingFilesList.ToolWindow.Service;
using WorkingFilesList.ToolWindow.Test.TestingInfrastructure;

namespace WorkingFilesList.ToolWindow.Test.Service
{
    [TestFixture]
    public class MetricIndicatorServiceTests
    {
        private static DocumentMetadata CreateDocumentMetadata(DateTime activatedAt, int lines = 0)
        {
            var info = new DocumentMetadataInfo
            {
                LineCount = lines
            };
            var metadata = new DocumentMetadata(info, string.Empty, null)
            {
                ActivatedAt = activatedAt
            };

            return metadata;
        }

        [Test]
        public void UsageOrderIsCalculatedFromActivatedAtTime()
        {
            // Arrange

            var utcNow = DateTime.UtcNow;

            var first = CreateDocumentMetadata(utcNow);
            var second = CreateDocumentMetadata(utcNow - TimeSpan.FromSeconds(1));
            var third = CreateDocumentMetadata(utcNow - TimeSpan.FromSeconds(2));

            var metadata = new[]
            {
                second,
                first,
                third
            };

            var builder = new UserPreferencesBuilder();
            var preferences = builder.CreateUserPreferences();
            preferences.MetricIndicatorType = MetricIndicatorType.UsageOrder;

            var service = new MetricIndicatorService();

            // Act

            service.SetMetricIndicator(metadata, preferences);

            // Assert

            var interval = 1 / (double)metadata.Length;

            var expectedFirst = interval * 3;
            var expectedSecond = interval * 2;
            var expectedThird = interval * 1;

            Assert.That(first.MetricIndicator, Is.EqualTo(expectedFirst));
            Assert.That(second.MetricIndicator, Is.EqualTo(expectedSecond));
            Assert.That(third.MetricIndicator, Is.EqualTo(expectedThird));
        }

        [Test]
        public void MaximumUsageOrderValueIs1()
        {
            // Arrange

            var utcNow = DateTime.UtcNow;
            var first = CreateDocumentMetadata(utcNow);
            var metadata = new[] { first };

            var builder = new UserPreferencesBuilder();
            var preferences = builder.CreateUserPreferences();
            preferences.MetricIndicatorType = MetricIndicatorType.UsageOrder;

            var service = new MetricIndicatorService();

            // Act

            service.SetMetricIndicator(metadata, preferences);

            // Assert

            Assert.That(first.MetricIndicator, Is.Not.GreaterThan(1));
        }

        [Test]
        public void NoneSetsAllIndicatorsToZero()
        {
            // Arrange

            var utcNow = DateTime.UtcNow;

            var first = CreateDocumentMetadata(utcNow);
            var second = CreateDocumentMetadata(utcNow - TimeSpan.FromSeconds(1));
            var third = CreateDocumentMetadata(utcNow - TimeSpan.FromSeconds(2));

            var metadata = new[]
            {
                second,
                first,
                third
            };

            var builder = new UserPreferencesBuilder();
            var preferences = builder.CreateUserPreferences();
            preferences.MetricIndicatorType = MetricIndicatorType.None;

            var service = new MetricIndicatorService();

            // Act

            service.SetMetricIndicator(metadata, preferences);

            // Assert

            Assert.That(first.MetricIndicator, Is.EqualTo(0));
            Assert.That(second.MetricIndicator, Is.EqualTo(0));
            Assert.That(third.MetricIndicator, Is.EqualTo(0));
        }

        [Test]
        public void FullSetsAllIndicatorsToOne()
        {
            // Arrange

            var utcNow = DateTime.UtcNow;

            var first = CreateDocumentMetadata(utcNow);
            var second = CreateDocumentMetadata(utcNow - TimeSpan.FromSeconds(1));
            var third = CreateDocumentMetadata(utcNow - TimeSpan.FromSeconds(2));

            var metadata = new[]
            {
                second,
                first,
                third
            };

            var builder = new UserPreferencesBuilder();
            var preferences = builder.CreateUserPreferences();
            preferences.MetricIndicatorType = MetricIndicatorType.Full;

            var service = new MetricIndicatorService();

            // Act

            service.SetMetricIndicator(metadata, preferences);

            // Assert

            const double desiredValue = 1;

            Assert.That(first.MetricIndicator, Is.EqualTo(desiredValue));
            Assert.That(second.MetricIndicator, Is.EqualTo(desiredValue));
            Assert.That(third.MetricIndicator, Is.EqualTo(desiredValue));
        }

        [Test]
        public void LineCountIsCalculatedFromLinesProperty()
        {
            // Arrange

            var first = CreateDocumentMetadata(DateTime.UtcNow, lines: 100);
            var second = CreateDocumentMetadata(DateTime.UtcNow, lines: 50);
            var third = CreateDocumentMetadata(DateTime.UtcNow, lines: 200);

            var metadata = new[]
            {
                first,
                second,
                third
            };

            var builder = new UserPreferencesBuilder();
            var preferences = builder.CreateUserPreferences();
            preferences.MetricIndicatorType = MetricIndicatorType.LineCount;

            var service = new MetricIndicatorService();

            // Act

            service.SetMetricIndicator(metadata, preferences);

            // Assert

            // Max lines is 200, so normalized values should be:
            // first: 100/200 = 0.5
            // second: 50/200 = 0.25
            // third: 200/200 = 1.0

            Assert.That(first.MetricIndicator, Is.EqualTo(0.5));
            Assert.That(second.MetricIndicator, Is.EqualTo(0.25));
            Assert.That(third.MetricIndicator, Is.EqualTo(1.0));
        }

        [Test]
        public void LineCountSetsAllIndicatorsToOneWhenAllFilesHaveZeroLines()
        {
            // Arrange

            var first = CreateDocumentMetadata(DateTime.UtcNow, lines: 0);
            var second = CreateDocumentMetadata(DateTime.UtcNow, lines: 0);
            var third = CreateDocumentMetadata(DateTime.UtcNow, lines: 0);

            var metadata = new[]
            {
                first,
                second,
                third
            };

            var builder = new UserPreferencesBuilder();
            var preferences = builder.CreateUserPreferences();
            preferences.MetricIndicatorType = MetricIndicatorType.LineCount;

            var service = new MetricIndicatorService();

            // Act

            service.SetMetricIndicator(metadata, preferences);

            // Assert

            const double desiredValue = 1;

            Assert.That(first.MetricIndicator, Is.EqualTo(desiredValue));
            Assert.That(second.MetricIndicator, Is.EqualTo(desiredValue));
            Assert.That(third.MetricIndicator, Is.EqualTo(desiredValue));
        }

        [Test]
        public void LineCountHandlesEmptyCollection()
        {
            // Arrange

            var metadata = new DocumentMetadata[0];

            var builder = new UserPreferencesBuilder();
            var preferences = builder.CreateUserPreferences();
            preferences.MetricIndicatorType = MetricIndicatorType.LineCount;

            var service = new MetricIndicatorService();

            // Act & Assert

            Assert.DoesNotThrow(() => service.SetMetricIndicator(metadata, preferences));
        }
    }
}

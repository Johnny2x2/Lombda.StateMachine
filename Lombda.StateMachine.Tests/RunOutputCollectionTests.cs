using NUnit.Framework;
using Lombda.StateMachine;

namespace Lombda.StateMachine.Tests
{
    [TestFixture]
    public class RunOutputCollectionTests
    {
        [Test]
        public void Constructor_WithNoParameters_ShouldCreateEmptyCollection()
        {
            // Act
            var collection = new RunOutputCollection<string>();

            // Assert
            Assert.That(collection.Index, Is.EqualTo(0));
            Assert.That(collection.Results, Is.Null);
        }

        [Test]
        public void Constructor_WithParameters_ShouldSetProperties()
        {
            // Arrange
            var index = 5;
            var results = new List<string> { "result1", "result2", "result3" };

            // Act
            var collection = new RunOutputCollection<string>(index, results);

            // Assert
            Assert.That(collection.Index, Is.EqualTo(index));
            Assert.That(collection.Results, Is.EqualTo(results));
            Assert.That(collection.Results.Count, Is.EqualTo(3));
        }

        [Test]
        public void Index_Property_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var collection = new RunOutputCollection<string>();
            var newIndex = 10;

            // Act
            collection.Index = newIndex;

            // Assert
            Assert.That(collection.Index, Is.EqualTo(newIndex));
        }

        [Test]
        public void Results_Property_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var collection = new RunOutputCollection<string>();
            var newResults = new List<string> { "new1", "new2" };

            // Act
            collection.Results = newResults;

            // Assert
            Assert.That(collection.Results, Is.EqualTo(newResults));
            Assert.That(collection.Results.Count, Is.EqualTo(2));
        }

        [Test]
        public void Collection_WithComplexType_ShouldWork()
        {
            // Arrange
            var complexResults = new List<List<int>>
            {
                new List<int> { 1, 2, 3 },
                new List<int> { 4, 5, 6 }
            };
            var index = 1;

            // Act
            var collection = new RunOutputCollection<List<int>>(index, complexResults);

            // Assert
            Assert.That(collection.Index, Is.EqualTo(index));
            Assert.That(collection.Results, Is.EqualTo(complexResults));
            Assert.That(collection.Results[0], Is.EqualTo(new List<int> { 1, 2, 3 }));
            Assert.That(collection.Results[1], Is.EqualTo(new List<int> { 4, 5, 6 }));
        }

        [Test]
        public void Collection_WithNullResults_ShouldHandleCorrectly()
        {
            // Arrange
            var index = 3;
            List<string>? nullResults = null;

            // Act
            var collection = new RunOutputCollection<string>(index, nullResults);

            // Assert
            Assert.That(collection.Index, Is.EqualTo(index));
            Assert.That(collection.Results, Is.Null);
        }

        [Test]
        public void Collection_WithEmptyResults_ShouldWork()
        {
            // Arrange
            var index = 2;
            var emptyResults = new List<string>();

            // Act
            var collection = new RunOutputCollection<string>(index, emptyResults);

            // Assert
            Assert.That(collection.Index, Is.EqualTo(index));
            Assert.That(collection.Results, Is.EqualTo(emptyResults));
            Assert.That(collection.Results.Count, Is.EqualTo(0));
        }

        [Test]
        public void Collection_PropertiesAreIndependent()
        {
            // Arrange
            var collection1 = new RunOutputCollection<string>(1, new List<string> { "a" });
            var collection2 = new RunOutputCollection<string>(2, new List<string> { "b" });

            // Act & Assert
            Assert.That(collection1.Index, Is.Not.EqualTo(collection2.Index));
            Assert.That(collection1.Results, Is.Not.EqualTo(collection2.Results));
        }

        [Test]
        public void Collection_WithDifferentTypes_ShouldWork()
        {
            // Arrange & Act
            var stringCollection = new RunOutputCollection<string>(1, new List<string> { "test" });
            var intCollection = new RunOutputCollection<int>(2, new List<int> { 42 });
            var boolCollection = new RunOutputCollection<bool>(3, new List<bool> { true });

            // Assert
            Assert.That(stringCollection.Results[0], Is.EqualTo("test"));
            Assert.That(intCollection.Results[0], Is.EqualTo(42));
            Assert.That(boolCollection.Results[0], Is.EqualTo(true));
        }
    }
}

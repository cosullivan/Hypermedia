using Hypermedia.Sample.Client;
using Hypermedia.Sample.Resources;
using Xunit;

namespace Hypermedia.JsonApi.Tests
{
    public sealed class JsonApiSerializerFacts
    {
        [Fact]
        public void CanDeserialize()
        {
            // arrange
            var serializer = new JsonApiSerializer(HypermediaSampleClient.CreateResolver());

            // act
            var resource = (PostResource)serializer.Deserialize(JsonContent.GetObject(nameof(CanDeserialize)));

            // assert
            Assert.Equal(2, resource.Id);
            Assert.Equal("Did the Greeks build temples for all of the children of Cronus?", resource.Title);
            Assert.Equal("kuwaly", resource.OwnerUser.DisplayName);
        }

        [Fact]
        public void CanDeserializeNullHasManyRelationship()
        {
            // arrange
            var serializer = new JsonApiSerializer(HypermediaSampleClient.CreateResolver());

            // act
            var resource = (PostResource)serializer.Deserialize(JsonContent.GetObject(nameof(CanDeserializeNullHasManyRelationship)));

            // assert
            Assert.Equal(2, resource.Id);
            Assert.Null(resource.Comments);
        }

        [Fact]
        public void CanDeserializeNullBelongsRelationship()
        {
            // arrange
            var serializer = new JsonApiSerializer(HypermediaSampleClient.CreateResolver());

            // act
            var resource = (PostResource)serializer.Deserialize(JsonContent.GetObject(nameof(CanDeserializeNullBelongsRelationship)));

            // assert
            Assert.Equal(2, resource.Id);
            Assert.Null(resource.OwnerUser);
        }
    }
}
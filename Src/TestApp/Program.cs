using System;
using System.Diagnostics;
using System.Linq;
using Hypermedia.Configuration;
using Hypermedia.Json;
using Hypermedia.JsonApi;
using Hypermedia.JsonApi.WebApi;
using Hypermedia.Metadata;
using Hypermedia.Sample.Client;
using Hypermedia.Sample.Resources;
using Hypermedia.Sample.WebApi;
using JsonLite.Ast;
using TestApp.Test;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = (JsonObject)JsonLite.Json.CreateAst(System.IO.File.ReadAllText(@"C:\Temp\Hypermedia\ComponentActualsResponseContent\ComponentActualsResponseContent.txt"));
            Console.WriteLine(json.Members.Count);

            var array = (JsonArray) json.Members[0].Value;
            var data = new JsonObject(new JsonMember("data", new JsonArray(array.Take(500000).ToList())));

            var serializer = new JsonApiSerializer(CreateRtioResolver());
            var timer = new Stopwatch();
            timer.Start();

            var entities = serializer.DeserializeMany(data).ToList();

            timer.Stop();

            Console.WriteLine("Count={0} Time Taken={1}ms", entities.Count, timer.ElapsedMilliseconds);


            //json = new JsonObject(json.Members[1]);

            //var resolver = CreateResolver();

            //var patch = new JsonApiPatch<PostResource>(resolver, new DasherizedFieldNamingStrategy(), json);
            //foreach (var member in patch.Members)
            //{
            //    Console.WriteLine(member.Name);
            //}

            //resolver.TryResolve(typeof(PostResource), out IContract contract);
            //var relationship = contract.Relationship(nameof(PostResource.OwnerUser));

            //Console.WriteLine("{0} => {1}", relationship.Name, relationship.Inverse(resolver).Name);

        }

        static IContractResolver CreateRtioResolver()
        {
            var builder = new Builder();

            builder.With<TrackSection>("tracksections")
                .Id(nameof(TrackSection.Id));

            builder.With<RailModel>("railmodels")
                .Id(nameof(RailModel.Id))
                .HasMany<RailComponent>(nameof(RailModel.Components));

            builder.With<RailComponent>("railcomponents")
                .Id(nameof(RailComponent.Id))
                .BelongsTo<RailModel>(nameof(RailComponent.Model))
                .BackingField(nameof(RailComponent.ModelId))
                .HasMany<TrackSection>(nameof(RailComponent.Sections));

            builder.With<ComponentActual>("componentactuals")
                .Id(nameof(ComponentActual.Id))
                .BelongsTo<RailComponent>(nameof(ComponentActual.Component))
                .BackingField(nameof(ComponentActual.ComponentId))
                .BelongsTo<TrainJourney>(nameof(ComponentActual.TrainJourney))
                .BackingField(nameof(ComponentActual.TrainId));

            builder.With<TrainJourney>("trainjourneys")
                .Id(nameof(TrainJourney.Id))
                .HasMany<ComponentActual>(nameof(TrainJourney.ComponentActuals))
                .HasMany<TrackSectionActual>(nameof(TrainJourney.SectionActuals));

            builder.With<TrackSectionActual>("sectionactuals")
                .Id(nameof(TrackSectionActual.Id))
                .BelongsTo<TrackSection>(nameof(TrackSectionActual.TrackSection))
                .BackingField(nameof(TrackSectionActual.TrackSectionId))
                .BelongsTo<TrainJourney>(nameof(TrackSectionActual.TrainJourney))
                .BackingField(nameof(TrackSectionActual.TrainJourneyId));

            return builder.Build();
        }

        /// <summary>
        /// Creates an instance of the resource contract resolver for the configured model.
        /// </summary>
        /// <returns>The resource contract resolver for the configured model.</returns>
        public static IContractResolver CreateResolver()
        {
            var builder = new Builder();

            builder.With<UserResource>("users")
                .Id(nameof(UserResource.Id))
                .HasMany<PostResource>(nameof(UserResource.Posts))
                    .Inverse(nameof(PostResource.OwnerUser))
                    .Template("/v1/users/{id}/posts", "id", resource => resource.Id);

            builder.With<PostResource>("posts")
                .Id(nameof(PostResource.Id))
                .BelongsTo<UserResource>(nameof(PostResource.OwnerUser))
                    .BackingField(nameof(PostResource.OwnerUserId))
                    .Inverse(nameof(UserResource.Posts))
                    .Template("/v1/users/{id}", "id", resource => resource.OwnerUserId)
                //.BelongsTo<UserResource>(nameof(PostResource.ApproverUser), resource => resource.ApproverId.HasValue)
                //    .BackingField(nameof(PostResource.ApproverId))
                //    .Template("/v1/users/{id}", "id", resource => resource.ApproverId)
                //.HasMany<CommentResource>(nameof(PostResource.Comments))
                //    //.Embedded()
                //    .Template("/v1/posts/{id}/comments", "id", resource => resource.Id)
                    ;

            //builder.With<CommentResource>("comments")
            //    .Id(nameof(CommentResource.Id))
            //    .BelongsTo<UserResource>(nameof(CommentResource.User))
            //    .BackingField(nameof(CommentResource.UserId))
            //    .Template("/v1/users/{id}", "id", resource => resource.UserId)
            //    .BelongsTo<PostResource>(nameof(CommentResource.Post))
            //    .BackingField(nameof(CommentResource.PostId))
            //    .Template("/v1/posts/{id}", "id", resource => resource.PostId);

            //TODO: maybe at the point in which it is built is the best place to link the inversions?

            return builder.Build();
        }
    }
}
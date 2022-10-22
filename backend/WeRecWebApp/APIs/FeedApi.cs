/*
 * Swagger WeRec - OpenAPI 3.0
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: 1.0.3
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using WeRecWebApp.Models;
using WeRecWebApp.Repository;

namespace WeRecWebApp.Apis
{ 
    /// <summary>
    /// 
    /// </summary>
    [Route("api/v1/feeds")]
    public class FeedApiController : Controller
    { 
        private readonly IFeedRepository _repo;
        public FeedApiController(IFeedRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Add new feed
        /// </summary>
        /// <remarks>Add new feed</remarks>
        /// <param name="feed"></param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid input</response>
        [HttpPost]
        public async Task<IActionResult> AddFeed([FromBody] PostFeedModel feed)
        {
            var faker = new Faker<Review>()
                .RuleFor(o => o.Comments, f => f.Make(f.Random.Int(1, 10), () => f.Lorem.Sentence()).ToList())
                .RuleFor(o => o.Id, f => f.Random.Guid().ToString())
                .RuleFor(o => o.Raiting, f => f.Random.Int(1, 5));
                
            feed.Configurations.ForEach(c => c.Id = Guid.NewGuid().ToString());

            var newFeed = new Feed
            {
                Id = Guid.NewGuid().ToString(),
                Name = feed.Name,
                Description = feed.Description,
                CreatorId = "1",
                CreatorName = "test",
                Tags = feed.Tags,
                Visibility = feed.Visibility,
                Configurations = feed.Configurations,
                Review = faker.Generate(1).Single()
            };

            try
            {
                var savedFeed = await _repo.InsertFeed(newFeed);
                return Ok(savedFeed);
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete feed by ID
        /// </summary>
        /// <param name="feedId">ID of the feed that needs to be altered</param>
        /// <response code="200">Successfully deleted</response>
        /// <response code="404">Feed not found</response>
        [HttpDelete("{feedId}")]
        public async Task<IActionResult> DeleteFeed([FromRoute] [Required] string feedId)
        {
            var isFeedDeleted = await _repo.DeleteFeed(feedId);
            if (!isFeedDeleted) return NotFound();
            return Ok();
        }

        /// <summary>
        /// Get feed information by id
        /// </summary>
        /// <remarks>Returns feed information</remarks>
        /// <param name="feedId">ID of the feed that needs to be altered</param>
        /// <response code="200">Returns feed</response>
        /// <response code="404">Feed not found</response>
        [HttpGet("{feedId}")]
        public async Task<IActionResult> GetFeedById([FromRoute] [Required] string feedId)
        {
            var feed = await _repo.GetFeed(feedId);
            if (feed == null) return NotFound();
            return Ok(feed);
        }

        /// <summary>
        /// Get all feeds
        /// </summary>
        /// <remarks>Returns all feed information</remarks>
        /// <response code="200">successful operation</response>
        [HttpGet]
        public async Task<IActionResult> GetFeeds()
        {
            var test = await _repo.GetFeeds();
            return Ok(test);
        }

        /// <summary>
        /// Update an existing feed
        /// </summary>
        /// <remarks>Update an existing feed by Id</remarks>
        /// <param name="feed">Update an existent feed</param>
        /// <param name="feedId">ID of the feed that needs to be altered</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">Feed not found</response>
        [HttpPut("{feedId}")]
        public async Task<IActionResult> UpdateFeed([FromBody] PutFeedModel feed, [FromRoute] [Required] string feedId)
        {
            var existingFeed = await _repo.GetFeed(feedId);
            if (existingFeed == null) return NotFound();

            existingFeed.Configurations = feed.Configurations.Any() ? feed.Configurations : existingFeed.Configurations;
            existingFeed.Description = !string.IsNullOrEmpty(feed.Description) ? feed.Description : existingFeed.Description;
            existingFeed.Name = !string.IsNullOrEmpty(feed.Name) ? feed.Name : existingFeed.Name;
            existingFeed.Tags = feed.Tags.Any() ? feed.Tags : existingFeed.Tags;
            existingFeed.Visibility = feed.Visibility ?? existingFeed.Visibility;
            var result = await _repo.UpdateFeed(existingFeed);
            if (!result) return BadRequest();
            return Ok();
        }
    }
}

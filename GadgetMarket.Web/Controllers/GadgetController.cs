using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using GadgetMarket.Model;
using Microsoft.AspNetCore.Authorization;
using GadgetMarket.Repositories;
using GadgetMarket.Exceptions;

namespace GadgetMarket.Web.Controllers
{
    //TODO add logging to db
    [ApiController]
    [Route("gadgets")]
    public class GadgetController : ControllerBase
    {
        private IGadgetsRepository gadgetsRepository;

        public GadgetController(IGadgetsRepository gadgetsRepository)
        {
            this.gadgetsRepository = gadgetsRepository;
        }

        [HttpGet("")]
        public async Task<List<Gadget>> GetAllGadgets()
        {
            return await gadgetsRepository.GetAllGadgetsAsync();
        }

        [HttpGet("page/{page:int}")]
        public async Task<GadgetsPageModel> GetGadgets(int page, int perPage)
        {
            return await gadgetsRepository.GetGadgetsAsync(page, perPage);
        }

        //TODO check this
        [HttpGet("{gadgetId:int}")]
        public async Task<Gadget> GetGadgetById(int gadgetId)
        {
            var gadget = await gadgetsRepository.GetGadgetByIdAsync(gadgetId);

            if(gadget == null)
            {
                throw new GadgetNotFoundException(gadgetId);
            }

            return gadget;
        }

        [HttpGet("category/{categoryId:int}")]
        public async Task<List<Gadget>> GetGadgetsByCategoryId(int categoryId)
        {
            return await gadgetsRepository.GetGadgetsByCategoryIdAsync(categoryId);
        }

        [HttpGet("category/{categoryId:int}/page/{page:int}")]
        public async Task<GadgetsPageModel> GetGadgetsByCategoryId(int categoryId, int page, int perPage)
        {
            return await gadgetsRepository.GetGadgetsByCategoryIdAsync(categoryId, page, perPage);
        }

        [HttpGet("name/{name}")]
        public async Task<GadgetsPageModel> GetGadgetsByName(string name, int page, int perPage)
        {
            return await gadgetsRepository.GetGadgetsByNameAsync(name, page, perPage);
        }

        [HttpGet("user/{ownerId:int}")]
        public async Task<List<Gadget>> GetGadgetsByOwnerId(int ownerId)
        {
            return await gadgetsRepository.GetGadgetsByOwnerIdAsync(ownerId);
        }

        [HttpPost]
        [Authorize]
        public async Task<Gadget> AddGadget([FromBody]Gadget gadget)
        {
            if (this.User.Identity == null)
            {
                throw new ForbiddenException();
            }

            if (int.TryParse(this.User.Identity.Name, out int userId))
            {
                gadget.OwnerId = userId;

                return await gadgetsRepository.CreateGadgetAsync(gadget);
            }

            throw new BadRequestException();
        }

        //TODO check this
        [HttpPut]
        [Authorize]
        public async Task<Gadget> UpdateGadget([FromBody]Gadget gadget)
        {
            if (this.User.Identity == null)
            {
                throw new ForbiddenException();
            }

            if (int.TryParse(this.User.Identity.Name, out int userId))
            {
                var owner = await gadgetsRepository.GetGadgetOwnerByGadgetIdAsync(gadget.Id);
                if (owner == null || owner.Id != userId)
                {
                    throw new ForbiddenException();
                }

                return await gadgetsRepository.UpdateGadgetAsync(gadget);
            }

            throw new BadRequestException();
        }

        [HttpDelete("{gadgetId:int}")]
        [Authorize]
        public async Task<bool> RemoveGadgetById(int gadgetId)
        {
            if (this.User.Identity == null)
            {
                throw new ForbiddenException();
            }

            //TODO admin can remove gadget of other user

            if (int.TryParse(this.User.Identity.Name, out int userId))
            {
                var owner = await gadgetsRepository.GetGadgetOwnerByGadgetIdAsync(gadgetId);
                if (owner == null || owner.Id != userId)
                {
                    throw new ForbiddenException();
                }

                return await gadgetsRepository.RemoveGadgetByIdAsync(gadgetId);
            }

            throw new BadRequestException();
        }

        ////TODO check this
        //[HttpPut]
        //[Authorize]
        //public async Task<IResult> ChangeCategory(int gadgetId, int newCategoryId)
        //{
        //    if (this.User.Identity == null)
        //    {
        //        return Results.Forbid();
        //    }

        //    //TODO admin can change gadget's category (check role)

        //    if (int.TryParse(this.User.Identity.Name, out int userId))
        //    {
        //        using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
        //        {
        //            try
        //            {
        //                var ownerId = await connection.QueryFirstOrDefaultAsync<int>("select ownerId from Gadget where Id = @GadgetId;",
        //                    new { GadgetId = gadgetId });

        //                if (userId != ownerId)
        //                {
        //                    return Results.Forbid();
        //                }

        //                int rows = await connection.ExecuteAsync("update Gadget set CategoryId = @CategoryId where Id = @Id",
        //                    new { Id = gadgetId, CategoryId = newCategoryId });

        //                return rows != 0 ? Results.Ok() : Results.NotFound();
        //            }
        //            catch
        //            {
        //                return Results.StatusCode(500);
        //            }
        //        }
        //    }
        //    return Results.StatusCode(500);
        //}

        ////TODO check this
        //[HttpPost]
        //[Authorize]
        //public async Task<IResult> ChangeOwner(int gadgetId, int newOwnerId)
        //{
        //    if (this.User.Identity == null)
        //    {
        //        return Results.Forbid();
        //    }

        //    //TODO admin can change gadget's owner (check role)
        //    if (int.TryParse(this.User.Identity.Name, out int userId))
        //    {
        //        using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
        //        {
        //            try
        //            {
        //                var ownerId = (await connection.QueryFirstOrDefaultAsync<int>("select ownerId from Gadget where Id = @GadgetId;", 
        //                    new { GadgetId = gadgetId }));

        //                if (userId != ownerId)
        //                {
        //                    return Results.Forbid();
        //                }

        //                int rows = await connection.ExecuteAsync("update Gadget set OwnerId = @OwnerId where Id = @Id", new { Id = gadgetId, OwnerId = newOwnerId });
        //                return rows != 0 ? Results.Ok() : Results.NotFound();
        //            }
        //            catch
        //            {
        //                return Results.StatusCode(500);
        //            }
        //        }
        //    }
        //    return Results.StatusCode(500);
        //}

        //TODO check this
        [HttpPut("owner/{id:int}")]
        [Authorize]
        public async Task<bool> PassGadgetsToUser(int ownerId, int inheritorId)
        {
            if (this.User.Identity == null)
            {
                throw new ForbiddenException();
            }

            //TODO admin can pass other user's gadgets

            if (int.TryParse(this.User.Identity.Name, out int userId))
            {
                if (userId != ownerId)
                {
                    throw new ForbiddenException();
                }

                return await gadgetsRepository.PassGadgetsToUserAsync(ownerId, inheritorId);
            }

            throw new BadRequestException();
        }

        //TODO check this
        [HttpDelete("owner/{id:int}")]
        [Authorize]
        public async Task<bool> RemoveAllGadgetsByOwnerId(int ownerId)
        {
            if(this.User.Identity == null)
            {
                throw new ForbiddenException();
            }

            //TODO admin can delete other user's gadgets

            if(int.TryParse(this.User.Identity.Name, out int userId))
            {
                if (userId != ownerId)
                {
                    throw new ForbiddenException();
                }

                return await gadgetsRepository.RemoveAllGadgetsByOwnerIdAsync(ownerId);
            }

            throw new BadRequestException();       
        }
    }
}

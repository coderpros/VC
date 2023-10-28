using System.Collections.Concurrent;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace VC.Res.WebInterface.Controllers.Api.Website
{
    [Route("api/website/[controller]")]
    [ApiController]
    public class PremisesController : ControllerBase
    {
        [HttpGet]
        [Route("getbyid")]
        public async Task<IActionResult> GetById(int id)
        {
            var objReturn = await Core.Integrations.Website.ModelConverter.To_PremiseAsync(await Core.Premises.Premise.FindAsync(id));

            return Ok(objReturn);
        }

        [HttpPost]
        [Route("getbyids")]
        public async Task<IActionResult> GetByIds([FromBody] List<int> ids)
        {
            var lstReturn = new List<VC.Shared.Models.Premise>();

            // setup options to limit number of concurrent tasks
            var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 5 };
            var lstResults = new ConcurrentBag<VC.Shared.Models.Premise>();

            await Parallel.ForEachAsync(ids, parallelOptions, async (id, cancellationToken) =>
            {
                lstResults.Add(await Core.Integrations.Website.ModelConverter.To_PremiseAsync(await Core.Premises.Premise.FindAsync(id)));
            });

            parallelOptions = null;

            lstReturn = lstResults.ToList();

            lstResults = null;

            return Ok(lstReturn);
        }

        [HttpPost]
        [Route("updatewebsiteinfo")]
        public async Task<IActionResult> UpdateWebsiteInfo(VC.Shared.Models.Premise premise)
        {
            if (!premise.Loaded) { return BadRequest(); }

            if (premise.Umb_Id == 0 || string.IsNullOrWhiteSpace(premise.Umb_URL)) { return BadRequest(); }

            // find the premise
            var objPremise = await Core.Premises.Premise.FindAsync(premise.Res_Id);

            if (!objPremise.Loaded) { return NotFound(); }

            // update the premise
            if (!await Core.Premises.Premise.Update_WebsiteIntegration(objPremise.Id, premise, "Website API"))
            {
                return StatusCode(500);
            }

            _ = await objPremise.RefreshAsync();

            return Ok(await Core.Integrations.Website.ModelConverter.To_PremiseAsync(objPremise));
        }
    }
}

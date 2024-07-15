using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PostgresAPI.Data;
using PostgresAPI.Models;
using PostgresAPI.Models.DTOs;

namespace PostgresAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ProspectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProspectsController(AppDbContext context)
        {
            _context = context;
        }

        private bool ProspectExists(int DDID)
        {
            return _context.prospects.Any(e => e.ddid == DDID);
        }

        #region Get

        #region get all + search

        //Get all prospects
        [HttpGet("allProspects")]
        public async Task<ActionResult<IEnumerable<Prospect>>> GetProspects()
        {
            return await _context.prospects.ToListAsync();
        }


        // search for a prospect
        // GET: api/Prospects/5   
        [HttpGet("{DDID}")]
        public async Task<ActionResult<Prospect>> GetProspect(int DDID)
        {
            var prospect = await _context.prospects.FindAsync(DDID);

            if (prospect == null)
            {
                return NotFound();
            }

            return prospect;
        }

        //Get all rewards
        [HttpGet("rewards")]
        public async Task<ActionResult<IEnumerable<Reward>>> GetRewards()
        {
            return await _context.rewards.ToListAsync();
        }


        // get all redemptions
        [HttpGet("redemptions")]
        public async Task<ActionResult<IEnumerable<Redemption>>> GetRedemptions()
        {
            return await _context.redemptions.ToListAsync();
        }

        #endregion

        #region insights

        #region points

        #endregion

        #region graphs

        // The number of prospects who joined each month/year
        //Line chart or bar chart
        [HttpGet("api/insights/prospects/join-date")]
        public async Task<IActionResult> GetProspectsByJoinDate()
        {
            var joinDateCounts = await _context.prospects
                .GroupBy(p => new { p.join_date.Year, p.join_date.Month })
                .Select(g => new JoinDateCountDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync();
            return Ok(joinDateCounts);
        }

        //The percentage of prospects who converted
        //Pie chart
        [HttpGet("api/insights/prospects/conversion-rate")]
        public async Task<IActionResult> GetConversionRate()
        {
            var totalProspects = await _context.prospects.CountAsync();
            var convertedProspects = await _context.prospects.CountAsync(p => p.converted);

            var conversionRate = new ConversionRateDto
            {
                Converted = convertedProspects,
                NotConverted = totalProspects - convertedProspects
            };

            return Ok(conversionRate);
        }

        // The most redeemed reward categories
        // Bar chart
        [HttpGet("api/insights/redemptions/popular-categories")]
        public async Task<IActionResult> GetPopularRewardCategories()
        {
            var popularCategories = await _context.redemptions
                .GroupBy(r => r.category)
                .Select(g => new PopularCategoryDto
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .ToListAsync();

            return Ok(popularCategories);
        }

        // The number of redemptions over time
        // Line chart
        [HttpGet("api/insights/redemptions/over-time")]
        public async Task<IActionResult> GetRedemptionsOverTime()
        {
            var redemptionsOverTime = await _context.redemptions
                .GroupBy(r => new { r.date.Year, r.date.Month })
                .Select(g => new RedemptionOverTimeDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            return Ok(redemptionsOverTime);
        }

        // The distribution of last active dates among prospects
        // Bar chart or histogram
        [HttpGet("api/insights/prospects/last-active-date")]
        public async Task<IActionResult> GetLastActiveDateDistribution()
        {
            var lastActiveDateCounts = await _context.prospects
                .GroupBy(p => new { p.last_active_date.Year, p.last_active_date.Month })
                .Select(g => new LastActiveDateCountDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(lastActiveDateCounts);
        }

        // The number of redemptions per prospect
        // Bar chart
        [HttpGet("api/insights/redemptions/by-prospect")]
        public async Task<IActionResult> GetRedemptionsByProspect()
        {
            var redemptionsByProspect = await _context.redemptions
                .GroupBy(r => r.ddid)
                .Select(g => new RedemptionsByProspectDto
                {
                    DDID = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .ToListAsync();

            return Ok(redemptionsByProspect);
        }
        #endregion

        #endregion

        #endregion

        #region Post
        // POST: api/Prospects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Prospect>> PostProspect(Prospect prospect)
        {
            _context.prospects.Add(prospect);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProspectExists(prospect.ddid))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(PostProspect), new { ID = prospect.id }, prospect);
        }

        [HttpPost("redemptions")]
        public async Task<IActionResult> AddRedemption([FromBody] Redemption redemption)
        {
            _context.redemptions.Add(redemption);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRedemptions), new { id = redemption.r_id, ddid = redemption.ddid }, redemption);
        }

        #endregion

        #region Put (edit)

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProspect(int id, [FromBody] Prospect prospect)
        {
            if (id != prospect.ddid)
            {
                return BadRequest();
            }

            _context.Entry(prospect).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        #region Delete

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProspect(int id)
        {
            var prospect = await _context.prospects.FindAsync(id);
            if (prospect == null)
            {
                return NotFound();
            }

            _context.prospects.Remove(prospect);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

    }

}

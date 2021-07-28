using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeldaTestWebAPI.Models;
using Microsoft.Extensions.Logging;

namespace TeldaTestWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly RegionContext db; // контекст базы данных
        private IMemoryCache cache ; //кэш
        private ILogger _logger; // логгер

        public RegionsController(RegionContext dbcontext, IMemoryCache memcache, ILogger<RegionsController> logger)
        {
            db = dbcontext;
            cache = memcache;
            _logger = logger;
        }

        // GET: api/Regions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Region>>> GetAllRegion() // запрос всех регионов
        {
            try
            {
                _logger.LogInformation("Request GetAllRegion() ({0})",DateTime.Now);
                return await db.Region.ToListAsync(); // возвращаем все регионы
            }
            catch
            {
                return Problem("Internal Server Error", "Module: GetAllRegion", 500); // алерт ошибки с указанием модуля
            }
        }

        // GET: api/Regions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Region>> GetRegion(int id) // запрос конкретного региона
        {
            Region region = null;
            if (!cache.TryGetValue(id,out region)) //если его нет в кэше
            {
                region = await db.Region.FindAsync(id); // ищем в базе
                if (region != null) // если нашли - добавляем в кэш на 3 минуты
                {
                    cache.Set(region.Id, region,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(3)));
                }
            }
            if (region == null) // если не нашли сообщаем обэтом
            {
                _logger.LogInformation("NotFound[404] in GetRegion({0}) ({1})", DateTime.Now, Request.QueryString.Value);
                return NotFound();
            }
            else // иначе возвращаем найденное
            {
                _logger.LogInformation("OK[200] in GetRegion({0}) ({1})",id, DateTime.Now);
                return region;
            }
        }

        // PUT: api/Regions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRegion(int id, Region region) //запрос обновления региона
        {
            if (id != region.Id) //если ид региона не соответствует самому региону
            {
                _logger.LogInformation("BadRequest[400] in UpdateRegion({0}) ({1})", id, DateTime.Now);
                return BadRequest(); // просим перепроверить запрос
            }
            db.Entry(region).State = EntityState.Modified; //меняем регион в базе
            try
            {
                await db.SaveChangesAsync(); //сохраняем изменения
                _logger.LogInformation("Ok[200] UpdateRegion({0}): {id={1};name={2};fullname={3};} ({4})",id,id,region.Name,region.FullName, DateTime.Now);
                return Ok(); // уведомляем, что все хорошо 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegionExists(id)) // если такого региона нет
                {
                    _logger.LogInformation("NotFound[404] in UpdateRegion({0}) ({1})", id, DateTime.Now);
                    return NotFound(); // возвращаем 404
                }
                else
                {
                    _logger.LogCritical("ERROR[500] UpdateRegion({0}) ({1})", DateTime.Now, Request.QueryString.Value);
                    return Problem("Internal Server Error", "Module: UpdateRegion", 500); // иначе - неопознанная ошибка
                }
            }
        }

        // POST: api/Regions
        [HttpPost]
        public async Task<ActionResult<Region>> CreateRegion(Region region) // запрос создания региона
        {
            try
            {
                if (region != null) // если прислали не пустоту
                {
                    db.Region.Add(region); // добавляем регион
                    int count = await db.SaveChangesAsync();// сохраняем изменения
                    if (count > 0) // если сохранили хоть один регион
                    {
                        cache.Set(region.Id, region, new MemoryCacheEntryOptions //добавляем его в кэш на 3 минуты
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
                        });                        
                    }
                    _logger.LogInformation("Ok[200] in CreateRegion(name={0};fullname={1};) ({0})",region.Name,region.FullName, DateTime.Now);
                    return Ok(); // сообщаем что "усе готово,шеф"
                }
                else
                {
                    _logger.LogWarning("BadRequest[400] in CreateRegion({0}) ({1})", DateTime.Now, Request.QueryString.Value);
                    return BadRequest(); //если пустота - сообщаем пользователю,чтоб проверил запрос
                }
            }
            catch (Exception)
            {
                _logger.LogCritical("ERROR CreateRegion({0}) ({1})", DateTime.Now,Request.QueryString.Value);
                return Problem("Internal Server Error","Module: CreateRegion",500);// ошибка
            }
            
        }

        // DELETE: api/Regions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegion(int id)
        {
            try
            {
                Region region = await db.Region.FindAsync(id); //ищем регион по id
                if (region == null) //если не нашли
                {
                    _logger.LogInformation("NotFound[404] in DeleteRegion({0}) ({1})", id, DateTime.Now);
                    return NotFound(); //вернем 404
                }
                db.Region.Remove(region); //удаляем с базы найденное
                await db.SaveChangesAsync();//сохраняем изменения
                _logger.LogInformation("Ok[200] in DeleteRegion({0}) ({1})", id, DateTime.Now);
                return Ok();// шлем 200
            }
            catch
            {
                _logger.LogCritical("ERROR[500] in DeleteRegion({0}) ({1})", DateTime.Now, Request.QueryString.Value);
                return Problem("Internal Server Error", "Module: DeleteRegion", 500);// ошибка
            }
        }

        private bool RegionExists(int id)
        {
            return db.Region.Any(e => e.Id == id);
        }
    }
}

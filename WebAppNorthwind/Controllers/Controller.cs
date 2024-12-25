using Microsoft.AspNetCore.Mvc; // Подключение пространства имен для работы с контроллерами MVC.
using Microsoft.EntityFrameworkCore; // Подключение EF Core для работы с базой данных.
using ClassLibrary1; // Подключение пользовательской библиотеки, содержащей модели данных.

namespace lab11sem3api.Controllers
{
    // Атрибут указывает, что данный контроллер обрабатывает HTTP-запросы по пути "api/Region".
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private readonly AppDbContext _context; // Контекст базы данных для взаимодействия с данными.

        // Конструктор контроллера, принимающий контекст базы данных через внедрение зависимостей.
        public RegionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Region
        // Возвращает список всех регионов из базы данных.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Regions>>> GetRegions()
        {
            var regions = await _context.Regions.ToListAsync(); // Асинхронное получение всех регионов.
            if (regions == null || regions.Count == 0) // Проверка, пуст ли список регионов.
            {
                return NotFound("No regions found."); // Возврат ошибки, если регионы отсутствуют.
            }
            return Ok(regions); // Возврат списка регионов.
        }

        // GET: api/Region/{id}
        // Возвращает регион по указанному идентификатору.
        [HttpGet("{id}")]
        public async Task<ActionResult<Regions>> GetRegion(int id)
        {
            var region = await _context.Regions.FindAsync(id); // Поиск региона по ID.
            if (region == null) // Проверка, найден ли регион.
            {
                return NotFound($"Region with ID {id} not found."); // Возврат ошибки, если регион не найден.
            }
            return Ok(region); // Возврат найденного региона.
        }

        // POST: api/Region
        // Добавляет новый регион в базу данных.
        [HttpPost]
        public async Task<ActionResult<Regions>> PostRegion(Regions region)
        {
            if (region == null) // Проверка, переданы ли данные региона.
            {
                return BadRequest("Region data is null."); // Возврат ошибки, если данные отсутствуют.
            }

            _context.Regions.Add(region); // Добавление нового региона в контекст.
            await _context.SaveChangesAsync(); // Сохранение изменений в базе данных.
            return CreatedAtAction(nameof(GetRegion), new { id = region.RegionID }, region); // Возврат созданного региона с кодом 201.
        }

        // PUT: api/Region/{id}
        // Обновляет существующий регион по ID.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegion(int id, Regions region)
        {
            if (id != region.RegionID) // Проверка, совпадают ли ID в запросе и модели.
            {
                return BadRequest("ID mismatch."); // Возврат ошибки, если ID не совпадают.
            }

            _context.Entry(region).State = EntityState.Modified; // Пометка региона как измененного.

            try
            {
                await _context.SaveChangesAsync(); // Попытка сохранить изменения.
            }
            catch (DbUpdateConcurrencyException) // Обработка ошибок конкурентного обновления.
            {
                if (!RegionExists(id)) // Проверка существования региона.
                {
                    return NotFound($"Region with ID {id} not found."); // Возврат ошибки, если регион отсутствует.
                }
                else
                {
                    throw; // Переброс исключения, если это другая ошибка.
                }
            }

            return NoContent(); // Возврат успешного ответа без содержимого.
        }

        // DELETE: api/Region/{id}
        // Удаляет регион по указанному идентификатору.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegion(int id)
        {
            var region = await _context.Regions.FindAsync(id); // Поиск региона по ID.
            if (region == null) // Проверка, найден ли регион.
            {
                return NotFound($"Region with ID {id} not found."); // Возврат ошибки, если регион не найден.
            }

            _context.Regions.Remove(region); // Удаление региона из контекста.
            await _context.SaveChangesAsync(); // Сохранение изменений в базе данных.
            return NoContent(); // Возврат успешного ответа без содержимого.
        }

        // Приватный метод для проверки существования региона в базе данных.
        private bool RegionExists(int id)
        {
            return _context.Regions.Any(e => e.RegionID == id); // Проверка наличия региона с указанным ID.
        }
    }
}

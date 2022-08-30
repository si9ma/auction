using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace quiz_template.Controllers;

[ApiController]
[Route("api/[action]")]
public class QuizController : ControllerBase
{
    // Typed lambda expression for Select() method. 
    private static readonly Expression<Func<Item, ItemDto>> AsItemDto =
        x => new ItemDto
        {
            Title = x.Title,
            Id = x.Id,
            Owner = x.Owner,
            Description = x.Description,
            CurrentBid = x.CurrentBid,
            StartBid = x.StartBid,
            State = x.State
        };

    private readonly IHostingEnvironment _hostingEnvironment;

    private readonly QuizDBContext db = new();

    public QuizController(IHostingEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    [HttpGet(Name = "ListItems")]
    public IQueryable<ItemDto> ListItems()
    {
        return db.Items.Where(item => item.State == "active").Select(AsItemDto).OrderBy(x => x.StartBid)
            .ThenBy(x => x.Id);
    }

    [HttpPost(Name = "Register")]
    public string Register([FromBody] User user)
    {
        // check if user name already exist
        var userCount = db.Users.Where(b => b.UserName == user.UserName).Count();
        if (userCount > 0) return "Username not available.";

        db.Users.Add(user);
        db.SaveChanges();
        return "User successfully registered.";
    }

    [HttpGet(Name = "GetItem")]
    public ItemDto GetItem([FromQuery] int id)
    {
        var count = db.Items.Where(x => x.Id == id).Count();
        if (count <= 0) return null;

        var x = db.Items.Where(x => x.Id == id).First();
        return new ItemDto
        {
            Title = x.Title,
            Id = x.Id,
            Owner = x.Owner,
            Description = x.Description,
            CurrentBid = x.CurrentBid,
            StartBid = x.StartBid,
            State = x.State
        };
    }

    [HttpGet(Name = "GetItemPhoto")]
    public IActionResult GetItemPhoto([FromQuery] int id)
    {
        var root = _hostingEnvironment.ContentRootPath;
        var bts = new byte[0];
        if (db.Items.Where(x => x.Id == id).Count() <= 0)
        {
            bts = System.IO.File.ReadAllBytes(Path.Combine(root, "Photos", "logo.pdf"));
            return File(bts, "image/file");
        }

        var item = db.Items.Where(x => x.Id == id).First();

        var jpeg = Path.Combine(root, "Photos", item.Id + ".jpeg");
        if (System.IO.File.Exists(jpeg)) bts = System.IO.File.ReadAllBytes(jpeg);

        var gif = Path.Combine(root, "Photos", item.Id + ".gif");
        if (System.IO.File.Exists(gif)) bts = System.IO.File.ReadAllBytes(gif);

        var png = Path.Combine(root, "Photos", item.Id + ".png");
        if (System.IO.File.Exists(png)) bts = System.IO.File.ReadAllBytes(png);

        if (bts.Length <= 0) bts = System.IO.File.ReadAllBytes(Path.Combine(root, "Photos", "logo.pdf"));

        return File(bts, "image/file");
    }
}
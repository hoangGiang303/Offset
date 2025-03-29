using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Offset.Models;
using BCrypt.Net;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Offset.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private DbOffsetContext db = new DbOffsetContext();
    private string filter = "";

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(int? page, string? sort)
    {
        int pageSize = 4;
        int pageNumber = (page == null || page < 0 ? 1 : page.Value);
        var projects = db.Projects.AsQueryable();

        switch (sort)
        {
            case "Mới nhất trước":
                projects = projects.OrderByDescending(p => p.Id);
                break;
            case "Cũ nhất trước":
                projects = projects.OrderBy(p => p.Id);
                break;
            case "Giá thấp đến cao":
                projects = projects.OrderBy(p => p.Price);
                break;
            case "Giá cao đến thấp":
                projects = projects.OrderByDescending(p => p.Price);
                break;
            default:
                projects = projects.OrderByDescending(p => p.Id);
                break;
        }

        var lstProjects = projects.ToPagedList(pageNumber, pageSize);
        return View(lstProjects);
    }

    public IActionResult ProjectDetail(string id)
    {
        var project = db.Projects.Find(Convert.ToInt32(id));
        return View(project);
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(User user, string confirmPassword, bool checkbox)
    {
        TempData["Message"] = "";
        if(!checkbox)
        {
            TempData["Message"] = "Vui lòng đọc kĩ chính sách và tích chọn";
            return View();
        }    
        if(user.Password != confirmPassword)
        {
            TempData["Message"] = "Mật khẩu không trùng khớp";
            return View();
        }

        var email = db.Users.Where(u => u.Email == user.Email).FirstOrDefault();

        if(email != null)
        {
            TempData["Message"] = "Email đã tồn tại";
            return View();
        }    

        user.CreatedDate = DateTime.Now;
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.Password = hashedPassword;
        db.Users.Add(user);
        db.SaveChanges();

        return RedirectToAction("LogIn", "Home");
    }

    public IActionResult LogIn()
    {
        return View();
    }

    [HttpPost]
    public IActionResult LogIn(User user)
    {
        TempData["Message"] = "";
        var us = db.Users.Where(u => u.Email == user.Email).FirstOrDefault();
        if(us == null || !BCrypt.Net.BCrypt.Verify(user.Password, us.Password))
        {
            TempData["Message"] = "Tài khoản hoặc mật khẩu không đúng";
            return View();
        }

        HttpContext.Session.SetString("Email", user.Email.ToString());
        return RedirectToAction("Index", "Home");
    }

    public IActionResult LogOut()
    {
        HttpContext.Session.Clear();
        HttpContext.Session.Remove("Email");
        return RedirectToAction("Index", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

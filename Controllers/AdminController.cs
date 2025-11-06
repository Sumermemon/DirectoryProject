using DirectoryProject.Entity;
using DirectoryProject.Layer.Interface;
using DirectoryProject.Models;
using DirectoryProject.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace DirectoryProject.Controllers
{
    public class AdminController : Controller
    {
        #region [ctor]
        private readonly IAdminRepo _adminRepo;
        public AdminController(IAdminRepo adminRepo)
        {
            _adminRepo = adminRepo;
        }
        #endregion
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Users()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if(email == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Directory()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public async Task<IActionResult> PageDirectory()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null)
            {
                return RedirectToAction("Index");
            }
            var respdata = new ResponseModel<List<DirectoryMaster>>();
            respdata = await _adminRepo.GetAllDirectory();
            return View(respdata);
        }
        public IActionResult CardPanel()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null)
                return RedirectToAction("Index");

            ViewBag.IdCard = HttpContext.Session.GetString("IdCard");
            return View();
        }
        #region [Users Methods]
        [HttpGet]
        public async Task<IActionResult> users()
        {
            var respModel = new ResponseModel<List<UsersMasters>>();
            respModel = await _adminRepo.GetAllUsers();
            return View(respModel);
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(UsersVM model)
        {
            var respModel = new ResponseModel<UsersVM>();
            try
            {
                string image = String.Empty;

                if (model.file != null)
                {
                    image = model?.ProfilePhoto!;
                    string fileExtention = Path.GetExtension(model?.file?.FileName!);
                    model!.ProfilePhoto = $"/Files/Images/User/ProfilePhoto/" + Guid.NewGuid() + fileExtention;
                    if (model.Id > 0)
                    {
                        string dirPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "Files/Images/User/ProfilePhoto/");
                        var filepath = new PhysicalFileProvider(dirPath).Root + $@"\{image}";
                        FileInfo file = new FileInfo(filepath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                }
                respModel = await _adminRepo.UpsertUsers(model);
                if (respModel.Success)
                {
                    if (model.file != null)
                    {
                        string folderPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "Files", "Images", "Packages", "DestinationImage");
                        if (!System.IO.Directory.Exists(folderPath))
                        {
                            System.IO.Directory.CreateDirectory(folderPath);
                        }

                        // Create full file path
                        string filePath = Path.Combine(folderPath, Path.GetFileName(model.ProfilePhoto!));

                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            await model.file.CopyToAsync(fs);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                respModel.Success = false;
                respModel.Message = "Data Not Inserted";
                respModel.data = null;
            }
            return Json(respModel);

        }
        [HttpPost]
        public async Task<IActionResult> GetUserbyId(int Id)
        {
            var respModel = new ResponseModel<UsersMasters>();
            respModel = await _adminRepo.GetUserById(Id);
            return Json(respModel);
        }
        #endregion
        #region [Directory]
        [HttpGet]
        public async Task<IActionResult> GetAllDirectory()
        {
            var respdata = new ResponseModel<List<DirectoryMaster>>();
            respdata = await _adminRepo.GetAllDirectory();
            return Json(respdata);
        }

        [HttpGet]
        public async Task<IActionResult> GetDirectoryById(int id)
        {
            var respdata = await _adminRepo.GetDirectoryById(id);
            return Json(respdata);
        }

        [HttpPost]
        public async Task<IActionResult> AddDirectory([FromBody] DirectoryMaster model)
        {
            var result = await _adminRepo.AddDirectory(model);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDirectory([FromBody] DirectoryMaster model)
        {
            var result = await _adminRepo.UpdateDirectory(model);
            return Json(result);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteDirectoryById(int id)
        {
            var result = await _adminRepo.DeleteDirectoryById(id);
            return Json(result);
        }
        #endregion
    }
}

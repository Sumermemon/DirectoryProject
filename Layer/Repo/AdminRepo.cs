using DirectoryProject.DBHelper;
using DirectoryProject.Entity;
using DirectoryProject.Layer.Interface;
using DirectoryProject.Models;
using DirectoryProject.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;

namespace DirectoryProject.Layer.Repo
{
    public class AdminRepo : IAdminRepo
    {
        #region [ctor]
        private readonly AppDBContext _dbContext;
        public AdminRepo(AppDBContext dBContext)
        {
            _dbContext = dBContext;
        }
        #endregion
        #region Directory 
        public async Task<ResponseModel<List<DirectoryMaster>>> GetAllDirectory()
        {
            var respData = new ResponseModel<List<DirectoryMaster>>();
            try
            {
                var data = await _dbContext.DirectoryMaster.ToListAsync();
                if (data.Count > 0)
                {
                    respData.Success = true;
                    respData.data = data;
                }
                else
                {
                    respData.Success = false;
                }
            }
            catch (Exception ex)
            {
                respData.Success = false;
                respData.Message = ex.Message;
            }
            return respData;
        }

        // Get by Id
        public async Task<ResponseModel<DirectoryMaster>> GetDirectoryById(int id)
        {
            var respData = new ResponseModel<DirectoryMaster>();
            try
            {
                var data = await _dbContext.DirectoryMaster.FirstOrDefaultAsync(x => x.Id == id);
                if (data != null)
                {
                    respData.Success = true;
                    respData.data = data;
                }
                else
                {
                    respData.Success = false;
                    respData.Message = "Directory not found.";
                }
            }
            catch (Exception ex)
            {
                respData.Success = false;
                respData.Message = ex.Message;
            }
            return respData;
        }

        // Add new
        public async Task<ResponseModel<DirectoryMaster>> AddDirectory(DirectoryMaster model)
        {
            var respData = new ResponseModel<DirectoryMaster>();
            try
            {
                _dbContext.DirectoryMaster.Add(model);
                await _dbContext.SaveChangesAsync();
                respData.Success = true;
                respData.data = model;
            }
            catch (Exception ex)
            {
                respData.Success = false;
                respData.Message = ex.Message;
            }
            return respData;
        }

        // Update
        public async Task<ResponseModel<DirectoryMaster>> UpdateDirectory(DirectoryMaster model)
        {
            var respData = new ResponseModel<DirectoryMaster>();
            try
            {
                var existing = await _dbContext.DirectoryMaster.FindAsync(model.Id);
                if (existing != null)
                {
                    _dbContext.Entry(existing).CurrentValues.SetValues(model);
                    await _dbContext.SaveChangesAsync();

                    respData.Success = true;
                    respData.data = existing;
                }
                else
                {
                    respData.Success = false;
                    respData.Message = "Directory not found.";
                }
            }
            catch (Exception ex)
            {
                respData.Success = false;
                respData.Message = ex.Message;
            }
            return respData;
        }

        // Delete
        public async Task<ResponseModel<bool>> DeleteDirectoryById(int id)
        {
            var respData = new ResponseModel<bool>();
            try
            {
                var entity = await _dbContext.DirectoryMaster.FindAsync(id);
                if (entity != null)
                {
                    _dbContext.DirectoryMaster.Remove(entity);
                    await _dbContext.SaveChangesAsync();
                    respData.Success = true;
                    respData.data = true;
                }
                else
                {
                    respData.Success = false;
                    respData.data = false;
                    respData.Message = "Directory not found.";
                }
            }
            catch (Exception ex)
            {
                respData.Success = false;
                respData.data = false;
                respData.Message = ex.Message;
            }
            return respData;
        }
        #endregion
        #region [Users]
        public async Task<ResponseModel<List<UsersMasters>>> GetAllUsers()
        {
            var respData = new ResponseModel<List<UsersMasters>>();
            try
            {
                var data = await _dbContext.UsersMasters.ToListAsync();
                if (data != null && data.Count() > 0)
                {
                    respData.Success = true;
                    respData.data = data;
                }
                else
                {
                    respData.Success = false;
                    respData.Message = "";
                }
            }
            catch (Exception ex)
            {
                respData.Success = false;
                respData.Message = ex.Message;
            }
            return respData;
        }
        public async Task<ResponseModel<UsersVM>> UpsertUsers(UsersVM model)
        {
            var respData = new ResponseModel<UsersVM>();
            var users = new UsersMasters();
            try
            {
                if (model != null)
                {
                    if (model.Id > 0)
                    {
                        // Update
                        users = await _dbContext.UsersMasters.Where(w => w.Id == model.Id).FirstOrDefaultAsync();
                        if (users != null)
                        {
                            var exsit = await _dbContext.UsersMasters.Where(w => w.Name == model.Name && w.Id == model.Id).FirstOrDefaultAsync();
                            if (exsit == null)
                            {
                                users.Name = model.Name;
                                users.GTMDANo = model.GTMDANo;
                                users.ProfilePhoto = model.ProfilePhoto;
                                users.DOB = model.DOB;
                                users.Qualification = model.Qualification;
                                users.Email = model.Email;
                                users.RegNo = model.RegNo;
                                users.MobileNo = model.MobileNo;
                                users.IsActive = model.IsActive;
                                users.IsAdmin = model.IsAdmin;
                                _dbContext.SaveChanges();
                                respData.Success = true;
                            }
                            else
                            {
                                respData.Success = false;
                            }
                        }
                        else
                        {
                            respData.Message = "NotFound";
                        }
                    }
                    else
                    {
                        var data = await _dbContext.UsersMasters.Where(w => w.Name == model.Name && w.Id == model.Id).FirstOrDefaultAsync();
                        if (data == null)
                        {
                            users.Name = model.Name;
                            users.GTMDANo = model.GTMDANo;
                            users.ProfilePhoto = model.ProfilePhoto;
                            users.DOB = model.DOB;
                            users.Qualification = model.Qualification;
                            users.Email = model.Email;
                            users.RegNo = model.RegNo;
                            users.MobileNo = model.MobileNo;
                            users.IsActive = model.IsActive;
                            users.IsAdmin = model.IsAdmin;
                            await _dbContext.UsersMasters.AddAsync(users);
                            _dbContext.SaveChanges();
                            respData.Success = true;

                        }
                        else
                        {
                            respData.Success = false;
                            respData.Message = "Users Already Exist";
                        }
                    }
                }
                else
                {
                    respData.Success = false;
                }
            }
            catch (Exception ex)
            {
                respData.Success = false;
                respData.Message = ex.Message;
            }
            return respData;
        }
        public async Task<ResponseModel<UsersMasters>> GetUserById(int Id)
        {
            var respData = new ResponseModel<UsersMasters>();
            try
            {
                var users = await _dbContext.UsersMasters.Where(l => l.Id == Id).FirstOrDefaultAsync();
                if (users != null)
                {
                    respData.Success = true;
                    respData.data = users;
                }
                else
                {
                    respData.Success = false;
                }
            }
            catch (Exception ex)
            {
                respData.Success = false;
                respData.Message = ex.Message;
            }
            return respData;
        }
        #endregion
    }
}

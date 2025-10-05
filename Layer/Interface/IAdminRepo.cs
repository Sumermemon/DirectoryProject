using DirectoryProject.Entity;
using DirectoryProject.Models;
using DirectoryProject.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace DirectoryProject.Layer.Interface
{
    public interface IAdminRepo
    {
        #region Directory 
        Task<ResponseModel<List<DirectoryMaster>>> GetAllDirectory();
        Task<ResponseModel<DirectoryMaster>> GetDirectoryById(int id);
        Task<ResponseModel<DirectoryMaster>> AddDirectory(DirectoryMaster model);
        Task<ResponseModel<DirectoryMaster>> UpdateDirectory(DirectoryMaster model);
        Task<ResponseModel<bool>> DeleteDirectoryById(int id);
        #endregion
        #region Users 
        Task<ResponseModel<UsersMasters>> GetUserById(int Id);
        Task<ResponseModel<UsersVM>> UpsertUsers(UsersVM model);
        Task<ResponseModel<List<UsersMasters>>> GetAllUsers();
        #endregion

    }
}

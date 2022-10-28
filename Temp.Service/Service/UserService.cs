using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Temp.Common.Infrastructure;
using Temp.DataAccess.Data;
using Temp.Service.BaseService;
using Temp.Service.ViewModel;
using Microsoft.AspNetCore.Http;

namespace Temp.Service.Service
{
    /// <summary>
    /// user service class
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hostingEnviroment;

        
        /// <summary>
        /// userservice constructor
        /// </summary>
        public UserService(IUnitofWork unitofWork, IMapper mapper, IHostingEnvironment hostingEnvironment)
        {
            _unitofWork = unitofWork;
            _mapper = mapper;
            _hostingEnviroment = hostingEnvironment;
        }

        public IEnumerable<UserViewModel> GetAll()
        {
            var users = _unitofWork.UserBaseService.GetAll();
            return _mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(users);
        }
        public UserViewModel GetIdUser(int Id)
        {
            var user = _unitofWork.UserBaseService.GetById(Id);
            return _mapper.Map<User, UserViewModel>(user);
        }
        /// <summary>
        /// get all user
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserViewModel> GetAllIncluding()
        {
            var users = _unitofWork.UserBaseService.ObjectContext.Include(s => s.Role).ToList();
            return _mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(users);
        }
        
        /// <summary>
        /// create or edit user
        /// </summary>
        /// <param name="userDto"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Save(CreateUserViewModel userDto)
        {
            
            string uploadFile = Path.Combine(_hostingEnviroment.WebRootPath, "AvatarUsers");
            string filename = Guid.NewGuid().ToString() + " " + userDto.AvatarView.FileName;
            string path = Path.Combine(uploadFile, filename);
            userDto.AvatarView.CopyTo(new FileStream(path, FileMode.Create));           
            if (userDto.Id <= 0)
            {
                
                var user = _mapper.Map<CreateUserViewModel, User>(userDto);
                user.CreateDate = DateTime.Now;
                user.ExpiredDate = DateTime.Now;
                user.Type = (int) UserType.None;
                user.Avatar = filename;
                _unitofWork.UserBaseService.Add(user);
                _unitofWork.Save();
            }
            else
            {
                
                var user=_mapper.Map<CreateUserViewModel, User>(userDto);
                user.CreateDate = DateTime.Now;
                user.ExpiredDate = DateTime.Now;
                user.Type = (int)UserType.None;
                user.Avatar = filename;
                _unitofWork.UserBaseService.Update(user);
                _unitofWork.Save();
            }
        }
        
        /// <summary>
        /// delete user
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Delete(int id)
        {
            var user =  _unitofWork.UserBaseService.GetById(id);
            _unitofWork.UserBaseService.Delete(user);
            _unitofWork.Save();
        }
        
        /// <summary>
        /// approve request expired for user
        /// </summary>
        /// <param name="user"></param>
        public void ApproveRequestExpired(User user)
        {
            user.Type = (int) UserType.None;
            user.ExpiredDate = DateTime.Now.AddDays(30);
            _unitofWork.Save();
        }
        
        /// <summary>
        /// reject request expired for user
        /// </summary>
        /// <param name="user"></param>
        public void RejectRequestExpired(User user)
        {
            user.Type = (int) UserType.None;
            _unitofWork.Save();
        }
        
        /// <summary>
        /// get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetById(int id)
        {
            var user = _unitofWork.UserBaseService.GetById(id);
            return user;
        }

        public CreateUserViewModel GetUserEditById(int id)
        {
            var user = _unitofWork.UserBaseService.GetById(id);
            CreateUserViewModel userResult = _mapper.Map<User, CreateUserViewModel>(user);
            return userResult;

        }
    }
}
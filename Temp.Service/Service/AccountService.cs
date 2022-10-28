﻿using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Temp.Common.Infrastructure;
using Temp.DataAccess.Data;
using Temp.Service.BaseService;
using Temp.Service.ViewModel;
using Roles = Temp.Common.Infrastructure.Roles;

namespace Temp.Service.Service
{
    /// <summary>
    /// Account service
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IMapper _mapper;

        public AccountService(IUnitofWork unitofWork, IMapper mapper)
        {
            _unitofWork = unitofWork;
            _mapper = mapper;

        }
        
        /// <summary>
        /// login
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public User LogIn(LogInViewModel login)
        {
            //khoi tạo md5
            MD5 mh = MD5.Create();

            //Chuyển kiểu chuổi thành kiểu byte
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(login.Password);

            //mã hóa chuỗi đã chuyển
            byte[] hash = mh.ComputeHash(inputBytes);

            //tạo đối tượng StringBuilder (làm việc với kiểu dữ liệu lớn)
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            login.Password = Constants.MD5_key.key + sb.ToString();
            var account =
                _unitofWork.UserBaseService.ObjectContext.Include(s => s.Role)
                    .FirstOrDefault(s => s.Username == login.Username && s.Password == login.Password);
            return account;
        }
        
        /// <summary>
        /// register
        /// </summary>
        /// <param name="accModel"></param>
        public void CreateAccount(CreateAccViewModel accModel)
        {
            //khoi tạo md5
            MD5 mh = MD5.Create();

            //Chuyển kiểu chuổi thành kiểu byte
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(accModel.Password);

            //mã hóa chuỗi đã chuyển
            byte[] hash = mh.ComputeHash(inputBytes);

            //tạo đối tượng StringBuilder (làm việc với kiểu dữ liệu lớn)
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            accModel.Password = Constants.MD5_key.key + sb.ToString();
            accModel.RoleId = (int)Roles.User;
            accModel.CreateDate = DateTime.Now;
            accModel.ExpiredDate = DateTime.Now;
            accModel.Type = (int)UserType.None;
            var user = _mapper.Map<CreateAccViewModel, User>(accModel);
            _unitofWork.UserBaseService.Add(user);
            _unitofWork.Save();
        }
        
        /// <summary>
        /// check account
        /// </summary>
        /// <param name="accModel"></param>
        /// <returns></returns>
        public bool CheckAccount(CreateAccViewModel accModel)
        {
            var isExist = _unitofWork.UserBaseService.ObjectContext.Any(s => s.Username.Equals(accModel.Username));
            if (!isExist)
            {
                return true;
            }            
            return false;               
        }
        
        /// <summary>
        /// change password
        /// </summary>
        /// <param name="passModel"></param>
        /// <returns></returns>
        public bool ChangePass(ChangePassViewModel passModel)
        {
            var user = _unitofWork.UserBaseService.ObjectContext.Include(s => s.Role)
                .FirstOrDefault(s => s.Username == passModel.UserName);
            
            if (user == null) return false;
            var userUpdate = new User
            {
                Username = user.Username,
                Id = user.Id,
                Password = passModel.Password,
                RoleId = user.Id
            };
            _unitofWork.UserBaseService.Update(userUpdate);
            _unitofWork.Save();
            return true;
        }
        
        /// <summary>
        /// check password
        /// </summary>
        /// <param name="passModel"></param>
        /// <returns></returns>
        public bool CheckPass(ChangePassViewModel passModel)
        {
            var user = _unitofWork.UserBaseService.Get(s => s.Username == passModel.UserName);
            if (user != null && user.Password != passModel.CurrentPass)
            {
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// get account by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public User GetAccount(string name)
        {
            return _unitofWork.UserBaseService.ObjectContext.FirstOrDefault(s => s.Username == name);
        }
        
        /// <summary>
        /// request expired date
        /// </summary>
        /// <param name="user"></param>
        public void RequestExpired(User user)
        {
            user.Type = (int)UserType.Processing;
            _unitofWork.Save();
        }
    }
}
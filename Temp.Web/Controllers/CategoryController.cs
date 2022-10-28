﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Temp.Service.Service;
using Temp.Service.ViewModel;

namespace Temp.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public IActionResult Index()
        {
            var categories = _categoryService.GetAllCategory();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Save(int Id)
        {
            if (Id <= 0)
            {
                return View();
            }
            else
            {
                var cate = _categoryService.GetCateId(Id);
                return View(cate);
            }
        }
        [HttpPost]
        public IActionResult Save(CategoryViewModel categoryDto)
        {
            _categoryService.Save(categoryDto);
            return RedirectToAction("Index", "Category");
        }
        [HttpGet]
        [HttpPost]
        public IActionResult Delete(int Id)
        {
            _categoryService.Delete(Id);
            return RedirectToAction("Index", "Category");
        }
    }
}

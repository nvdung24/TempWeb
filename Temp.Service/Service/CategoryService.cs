using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Temp.DataAccess.Data;
using Temp.Service.BaseService;
using Temp.Service.ViewModel;

namespace Temp.Service.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitofWork _unitofwork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitofWork unitofWork, IMapper mapper)
        {
            _unitofwork = unitofWork;
            _mapper = mapper;
        }

        public void Delete(int Id)
        {
            var cate = _unitofwork.CategoryBaseService.GetById(Id);
            _unitofwork.CategoryBaseService.Delete(cate);
            _unitofwork.Save();
        }

        public IEnumerable<CategoryViewModel> GetAllCategory()
        {
            var categories = _unitofwork.CategoryBaseService.GetAll();
            return _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryViewModel>>(categories);
        }

        public CategoryViewModel GetCateId(int Id)
        {
            var category = _unitofwork.CategoryBaseService.GetById(Id);
            return _mapper.Map<Category, CategoryViewModel>(category);
        }

        public void Save(CategoryViewModel catrgoryDto)
        {
            if (catrgoryDto.Id <= 0)
            {
                var cate = _mapper.Map<CategoryViewModel, Category>(catrgoryDto);
                _unitofwork.CategoryBaseService.Add(cate);
                _unitofwork.Save();
                
            }
            else
            {
                var cate = _mapper.Map<CategoryViewModel, Category>(catrgoryDto);
                _unitofwork.CategoryBaseService.Update(cate);
                _unitofwork.Save();
            }
        }
        
    }
}

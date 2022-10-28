using System;
using System.Collections.Generic;
using System.Text;
using Temp.Service.ViewModel;

namespace Temp.Service.Service
{
    public interface ICategoryService
    {
        IEnumerable<CategoryViewModel> GetAllCategory();
        CategoryViewModel GetCateId(int Id);

        void Save(CategoryViewModel catrgoryDto);
        void Delete(int Id);
    }
}

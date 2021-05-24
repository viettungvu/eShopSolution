using eShopSolution.Data.Entites;
using eShopSolution.ViewModels.System.Languages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Languages
{
    public interface ILanguageService
    {
        Task<string> Create(LanguageCreateRequest request);
        Task<bool> Update(LanguageUpdateRequest request);
        Task<bool> Delete(string  languageId);
        Task<List<Language>> GetAll();
    }
}

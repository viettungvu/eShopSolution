using eShopSolution.Data.EF;
using eShopSolution.Data.Entites;
using eShopSolution.Ultilities.Exceptions;
using eShopSolution.ViewModels.System.Languages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace eShopSolution.Application.System.Languages
{
   public  class LanguageService : ILanguageService
    {
        private readonly EShopDbContext _context;
        public LanguageService(EShopDbContext context)
        {
            _context = context;
        }
        public async Task<string> Create(LanguageCreateRequest  request)
        {
            var language = await _context.Languages.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (language != null)
                throw new EShopException("Language da ton tai");
            var newLanguage = new Language()
            {
                Id=request.Id,
                Name = request.Name,
                IsDefault = request.IsDefault,
            };
            _context.Languages.Add(newLanguage);
            await _context.SaveChangesAsync();
            return request.Id;
        }
        public async Task<bool> Delete(string languageId)
        {
            var language = _context.Languages.Find(languageId);
            if (language == null)
                throw new EShopException("Language {languageId} khong ton tai");
            _context.Languages.Remove(language);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> Update(LanguageUpdateRequest request)
        {
            var language = _context.Languages.FirstOrDefault(x => x.Id == request.Id);
            if (language == null)
                throw new EShopException("Language {languageId} khong ton tai");
            language.Name = request.Name;
            language.IsDefault = request.IsDefault;
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<Language>> GetAll(){
            var languages = await _context.Languages.Select(
                x => new Language() { Id = x.Id, IsDefault = x.IsDefault, Name = x.Name }
                ).ToListAsync();
            return languages;
        }
    }
}

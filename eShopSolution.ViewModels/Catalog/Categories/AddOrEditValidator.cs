using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Categories
{
    public class AddOrEditValidator : AbstractValidator<CategoryCreateRequest>
    {
        public AddOrEditValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name should not be blank")
                .MaximumLength(100).WithMessage("Name should be maximum 100 characters");
            RuleFor(x => x.SeoAlias).NotEmpty().WithMessage("SEO Alias should not be blank")
                .MaximumLength(100).WithMessage("SEO Alias should be maximum 100 characters");
            RuleFor(x => x.SeoDescription).NotEmpty().WithMessage("SEO Description should not be blank")
                .MaximumLength(256).WithMessage("SEO DÉcription should be maximum 100 characters");
            RuleFor(x => x.SeoTitle).NotEmpty().WithMessage("SEO Title should not be blank")
                .MaximumLength(100).WithMessage("SEO Title should be maximum 100 characters");
        }
    }
}
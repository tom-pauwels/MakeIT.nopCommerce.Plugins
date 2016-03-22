using FluentValidation;
using MakeIT.Nop.Plugin.Payments.Ogone.Models;
using Nop.Services.Localization;

namespace MakeIT.Nop.Plugin.Payments.Ogone.Validators
{
	public class PaymentInfoValidator : AbstractValidator<PaymentInfoModel>
	{
		public PaymentInfoValidator(ILocalizationService localizationService)
		{ }
	}
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.Entities;
using OrchardCore.ReCaptcha.Configuration;
using OrchardCore.ReCaptcha.Services;
using OrchardCore.Settings;

namespace OrchardCore.ReCaptcha
{
    public class ReCaptchaLoginFilter : IAsyncResultFilter
    {
        private readonly ILayoutAccessor _layoutAccessor;
        private readonly ReCaptchaService _reCaptchaService;
        private readonly IShapeFactory _shapeFactory;

        private readonly ReCaptchaSettings _reCaptchaSettings;

        public ReCaptchaLoginFilter(
            ILayoutAccessor layoutAccessor,
            ReCaptchaService reCaptchaService,
            IShapeFactory shapeFactory,
            IOptions<ReCaptchaSettings> optionsAccessor)
        {
            _layoutAccessor = layoutAccessor;
            _reCaptchaService = reCaptchaService;
            _shapeFactory = shapeFactory;
            _reCaptchaSettings = optionsAccessor.Value;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!context.IsViewOrPageResult()
                || !string.Equals("OrchardCore.Users", Convert.ToString(context.RouteData.Values["area"]), StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }

            if (!_reCaptchaSettings.IsValid())
            {
                await next();
                return;
            }

            var layout = await _layoutAccessor.GetLayoutAsync();

            if (_reCaptchaService.IsThisARobot())
            {
                var afterLoginZone = layout.Zones["AfterLogin"];
                await afterLoginZone.AddAsync(await _shapeFactory.CreateAsync("ReCaptcha"));
            }

            var afterForgotPasswordZone = layout.Zones["AfterForgotPassword"];
            await afterForgotPasswordZone.AddAsync(await _shapeFactory.CreateAsync("ReCaptcha"));

            var afterRegisterZone = layout.Zones["AfterRegister"];
            await afterRegisterZone.AddAsync(await _shapeFactory.CreateAsync("ReCaptcha"));

            var afterResetPasswordZone = layout.Zones["AfterResetPassword"];
            await afterResetPasswordZone.AddAsync(await _shapeFactory.CreateAsync("ReCaptcha"));

            await next();
        }
    }
}

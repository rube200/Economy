﻿#region

using System;
using System.Threading.Tasks;
using Autofac;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Core.Console;
using OpenMod.Core.Helpers;
using OpenMod.Core.Plugins;
using OpenMod.Economy.Controllers;
using OpenMod.Extensions.Economy.Abstractions;

#endregion

[assembly:
    PluginMetadata("OpenMod.Economy", Author = "OpenMod,Rube200", DisplayName = "OpenMod.Economy",
        Website = "https://github.com/openmod/OpenMod.Economy")]

namespace OpenMod.Economy
{
    public sealed class Economy : OpenModUniversalPlugin
    {
        private readonly IConsoleActorAccessor m_ConsoleActorAccessor;
        internal readonly IStringLocalizer StringLocalizer;

        // ReSharper disable once SuggestBaseTypeForParameter
        public Economy(IConsoleActorAccessor consoleActorAccessor, IServiceProvider serviceProvider,
            IStringLocalizer stringLocalizer) : base(serviceProvider)
        {
            m_ConsoleActorAccessor = consoleActorAccessor;
            StringLocalizer = stringLocalizer;
        }

        protected override Task OnLoadAsync()
        {
            AsyncHelper.Schedule("Economy load 1 frame delay.", async () =>
            {
                await UniTask.DelayFrame(1);
                var economy = LifetimeScope.Resolve<IEconomyProvider>();
                if (economy is not DatabaseController baseController)
                    return;

                Logger.LogInformation($"Database type set to: '{baseController.DbStoreType}'");
                await economy.GetBalanceAsync(m_ConsoleActorAccessor.Actor.Type,
                    m_ConsoleActorAccessor.Actor.Id); //force call to detect missing libs
            });

            return Task.CompletedTask;
        }
    }
}
﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoDesign.Component.Extension;
using MonoDesign.Core;
using MonoDesign.Engine;
using MonoDesign.Engine.Extension;
using NLog;
using NLog.Extensions.Logging;

namespace MonoDesign.Game {
	public static class Program {
		[STAThread]
		static void Main() {
			using (var game = new MyGame()) {
				ConfigureServices(game);
				var graphicsDeviceService = GameServices.GetService<IGraphicsDeviceService>();
				game.Services.AddService(graphicsDeviceService.GetType(), graphicsDeviceService);
				game.DesignEngine = GameServices.GetService<DesignEngine>();
				game.Run();
			}
		}
		private static void ConfigureServices(Microsoft.Xna.Framework.Game game) {
			
		}
	}
}

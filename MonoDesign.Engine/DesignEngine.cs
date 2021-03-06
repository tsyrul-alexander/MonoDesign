﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using MonoDesign.Core.Annotations;
using MonoDesign.Core.Entity.GameObject;
using MonoDesign.Core.Entity.Scene;
using MonoDesign.Core.File;
using MonoDesign.Core.Process;
using MonoDesign.Engine.Manager;
using ProjectInfo = MonoDesign.Engine.Project.ProjectInfo;
using SceneLookup = MonoDesign.Engine.Project.SceneLookup;

namespace MonoDesign.Engine
{
	public class DesignEngine: INotifyPropertyChanged {
		private readonly IProjectManager _projectManager;
		private readonly ISceneManager _sceneManager;
		private readonly IFileService _fileService;
		private readonly IAssetManager _assetManager;
		private Scene _currentScene;
		private ProjectInfo _projectInfo;
		public event PropertyChangedEventHandler PropertyChanged;
		public ProjectInfo ProjectInfo {
			get => _projectInfo;
			private set {
				if (Equals(value, _projectInfo))
					return;
				_projectInfo = value;
				OnPropertyChanged();
			}
		}
		public Scene CurrentScene {
			get => _currentScene;
			set {
				_currentScene = value;
				OnPropertyChanged();
			}
		}
		public DesignEngine(IProjectManager projectManager, ISceneManager sceneManager,
				IFileService fileService, IAssetManager assetManager) {
			_projectManager = projectManager;
			_sceneManager = sceneManager;
			_fileService = fileService;
			_assetManager = assetManager;
		}
		public virtual void LoadProject(string path) {
			if (_fileService.GetIsDirectory(path)) {
				path = _fileService.CombinePath(path, EngineSetting.ProjectInfoFileName);
			}
			ProjectInfo = _projectManager.Load(path);
		}
		public virtual void CreateProject(string directory, string name) {
			ProjectInfo = new ProjectInfo {
				Id = Guid.NewGuid(),
				Name = name,
				RootDirectory = _fileService.CombinePath(directory, name)
			};
		}
		public virtual void CreateScene() {
			var scene = CreateEmptyScene();
			ProjectInfo.Scenes.Add(SceneLookup.Create(scene));
			CurrentScene = scene;
		}
		protected virtual Scene CreateEmptyScene() {
			return new Scene {
				Id = Guid.NewGuid(),
				Name = "NewScene"
			};
		}
		public virtual void SaveProject() {
			_projectManager.Save(ProjectInfo);
		}
		public virtual void SaveScene() {
			_sceneManager.Save(ProjectInfo, CurrentScene);
		}
		public virtual void LoadScene(SceneLookup lookup) {
			CurrentScene = _sceneManager.Load(ProjectInfo, lookup);
		}

		public Texture2D LoadTexture(string name) {
			return _assetManager.LoadTexture(ProjectInfo, name);
		}
		public virtual void CreateGameObject() {
			var gameObject = new GameObject {
				Name = nameof(GameObject)
			};
			gameObject.Initialize();
			CurrentScene.GameObjects.Add(gameObject);
		}
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		public void BuildProject() {
			_projectManager.BuildProject(ProjectInfo);
		}
		public virtual string GetProjectBuildDirectory() {
			var projectDirectory = _projectManager.GetProjectSolutionPath(ProjectInfo);
			return _fileService.CombinePath(projectDirectory, EngineSetting.SolutionDebugBuildFolder);
		}

		public virtual string GetProjectDllPath() {
			var buildDirectory = GetProjectBuildDirectory();
			return _fileService.CombinePath(buildDirectory, $"{ProjectInfo.Name}.dll");
		}
		public virtual string GetProjectDirectory() {
			return ProjectInfo.RootDirectory;
		}
	}
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Particles.Physics.Particles;
using Particles.Physics.Particles.Forces;

namespace Particles
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class ParticlesGame : Microsoft.Xna.Framework.Game
	{
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private Matrix _view, _projection;

		private Ground _ground;

		private ParticleSystem _particleSystem;

		private Model _sphere;

		public ParticlesGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			_view = Matrix.CreateLookAt(new Vector3(50, 5, -50), new Vector3(25, 0, -25), Vector3.Up);
			_projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.Width / (float) GraphicsDevice.Viewport.Height, 1.0f, 100.0f);

			_particleSystem = new ParticleSystem();

			Particle particleA = new Particle { Position = new Vector3(25, 5, -25), MassInverse = 0 };
			Particle particleB = new Particle { Position = new Vector3(25, 3, -25), MassInverse = 1 };
			_particleSystem.Particles.Add(particleA); // fixed
			_particleSystem.Particles.Add(particleB);

			DampedSpringForce springForce = new DampedSpringForce(4, 20f, 1f);
			springForce.ParticleA = particleA;
			springForce.ParticleB = particleB;
			_particleSystem.Forces.Add(springForce);

			_ground = new Ground(this);
			Components.Add(_ground);

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			_sphere = Content.Load<Model>(@"Models\sphere0");
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			if (gameTime.ElapsedGameTime != TimeSpan.Zero)
			{
				float deltaTime = 1.0f / (float) gameTime.ElapsedGameTime.TotalMilliseconds;

				/*// Rotate motor particle around Y axis.
				_motorYaw += (deltaTime * _motorSpeed);
				Vector3 newPosition = Vector3.Transform(new Vector3(10, _motorParticle.Position.Y, 0), Matrix.CreateRotationY(_motorYaw));
				_motorParticle.Velocity = (newPosition - _motorParticle.Position) / deltaTime;
				_motorParticle.Position = newPosition;*/

				_particleSystem.Update(deltaTime);
			}

			_ground.UpdateCamera(_view, _projection);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

			foreach (Particle particle in _particleSystem.Particles)
			{
				foreach (ModelMesh mesh in _sphere.Meshes)
				{
					foreach (ModelMeshPart meshPart in mesh.MeshParts)
					{
						BasicEffect basicEffect = (BasicEffect) meshPart.Effect;
						basicEffect.AmbientLightColor = new Vector3(0.3f);
						basicEffect.EnableDefaultLighting();
						basicEffect.PreferPerPixelLighting = true;
						basicEffect.World = Matrix.CreateScale(0.25f) * Matrix.CreateTranslation(particle.Position);
						basicEffect.View = _view;
						basicEffect.Projection = _projection;
					}

					mesh.Draw();
				}
			}
		}
	}
}

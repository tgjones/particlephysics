using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Particles
{
    public class Ground : DrawableGameComponent
    {
        private const int SIZE = 100;

        private VertexDeclaration _vertexDeclaration;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private int _numVertices;
        private int _numIndices;

        private BasicEffect _basicEffect;

        public Ground(Game game)
            : base(game)
        {

        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _basicEffect = new BasicEffect(GraphicsDevice, null);
            _basicEffect.EnableDefaultLighting();

            _numVertices = SIZE * SIZE;

            int numInternalRows = SIZE - 2;
            _numIndices = (2 * SIZE * (1 + numInternalRows)) + (2 * numInternalRows);

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[_numVertices];
            for (int z = 0; z < SIZE; z++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    vertices[GetIndex(x, z)] = new VertexPositionNormalTexture(
                        new Vector3(x, 0, -z), new Vector3(0, 1, 0),
                        new Vector2(x / (float)(SIZE - 1) * 8, z / (float)(SIZE - 1) * 8));
                }
            }

            _vertexBuffer = new VertexBuffer(
                this.GraphicsDevice,
                typeof(VertexPositionNormalTexture),
                vertices.Length,
                BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);

            short[] indices = new short[_numIndices]; int indexCounter = 0;
            for (int z = 0; z < SIZE - 1; z++)
            {
                // insert index for degenerate triangle
                if (z > 0)
                    indices[indexCounter++] = GetIndex(0, z);

                for (int x = 0; x < SIZE; x++)
                {
                    indices[indexCounter++] = GetIndex(x, z);
                    indices[indexCounter++] = GetIndex(x, z + 1);
                }

                // insert index for degenerate triangle
                if (z < SIZE - 2)
                    indices[indexCounter++] = GetIndex(SIZE - 1, z);
            }

            _indexBuffer = new IndexBuffer(
                this.GraphicsDevice,
                typeof(short),
                indices.Length,
                BufferUsage.WriteOnly);
            _indexBuffer.SetData<short>(indices);

            _vertexDeclaration = new VertexDeclaration(
                this.GraphicsDevice, VertexPositionNormalTexture.VertexElements);

            Texture2D texture = Game.Content.Load<Texture2D>(@"Textures\dirt");

            _basicEffect.Texture = texture;
            _basicEffect.TextureEnabled = true;
        }

        private static short GetIndex(int x, int z)
        {
            return (short)((z * SIZE) + x);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            //this.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
            this.GraphicsDevice.VertexDeclaration = _vertexDeclaration;
            this.GraphicsDevice.Vertices[0].SetSource(_vertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
            this.GraphicsDevice.Indices = _indexBuffer;

            Effect effect = _basicEffect;
            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                this.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    0,
                    0,
                    _numVertices,
                    0,
                    _numIndices - 2);

                pass.End();
            }
            effect.End();
        }

        public void UpdateCamera(Matrix view, Matrix projection)
        {
            _basicEffect.View = view;
            _basicEffect.Projection = projection;
        }
    }
}
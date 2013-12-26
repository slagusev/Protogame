namespace Protogame
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The RenderContext interface.
    /// </summary>
    public interface IRenderContext
    {
        /// <summary>
        /// Gets the associated graphics device.
        /// </summary>
        /// <value>
        /// The graphics device.
        /// </value>
        GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        /// Gets a sprite batch associated with the current device, upon which 2D rendering is performed.
        /// </summary>
        /// <value>
        /// The sprite batch.
        /// </value>
        SpriteBatch SpriteBatch { get; }

        /// <summary>
        /// Gets a texture representing a single white pixel.
        /// </summary>
        /// <value>
        /// The single white pixel.
        /// </value>
        Texture2D SingleWhitePixel { get; }

        /// <summary>
        /// Gets the current active effect for rendering.
        /// </summary>
        /// <value>
        /// The effect.
        /// </value>
        Effect Effect { get; }

        /// <summary>
        /// Gets the bounding frustum for the current view and projection matrixes.
        /// </summary>
        /// <value>
        /// The bounding frustum for the current view and projection matrixes.
        /// </value>
        BoundingFrustum BoundingFrustum { get; }

        /// <summary>
        /// Gets or sets the view matrix for 3D rendering.
        /// </summary>
        /// <value>
        /// The view matrix for 3D rendering.
        /// </value>
        Matrix View { get; set; }

        /// <summary>
        /// Gets or sets the world matrix for 3D rendering.
        /// </summary>
        /// <value>
        /// The world matrix for 3D rendering.
        /// </value>
        Matrix World { get; set; }

        /// <summary>
        /// Gets or sets the projection matrix for 3D rendering.
        /// </summary>
        /// <value>
        /// The projection matrix for 3D rendering.
        /// </value>
        Matrix Projection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the world manager is currently
        /// rendering a 3D context.  For games using a 2D world manager, this will
        /// always be false.  For 3D games, world managers will do an initial 3D pass
        /// followed by a 2D pass that is rendered on top (for UI, etc.) In a 3D
        /// context, only I3DRenderUtilities can be used; in a 2D context, only
        /// I2DRenderUtilities can be used.
        /// </summary>
        /// <value>
        /// Whether the rendering context is currently a 3D context.
        /// </value>
        bool Is3DContext { get; set; }

        /// <summary>
        /// Called by the world manager to set up the render context at the beginning of a render.
        /// </summary>
        /// <param name="context">
        /// The current game context.
        /// </param>
        void Render(IGameContext context);

        /// <summary>
        /// Enables rendering with textures for the current effect.
        /// </summary>
        void EnableTextures();

        /// <summary>
        /// Enables rendering with plain colors for the current effect.
        /// </summary>
        void EnableVertexColors();

        /// <summary>
        /// Set the active texture used in rendering.
        /// </summary>
        /// <param name="texture">
        /// The active texture.
        /// </param>
        void SetActiveTexture(Texture2D texture);

        /// <summary>
        /// Push an effect onto the current rendering context, making it the active effect used for rendering.
        /// </summary>
        /// <typeparam name="T">
        /// The effect type.
        /// </typeparam>
        /// <param name="effect">
        /// The effect instance.
        /// </param>
        void PushEffect<T>(T effect) where T : Effect;

        /// <summary>
        /// Pop an effect from the current rendering context.
        /// </summary>
        void PopEffect();

        /// <summary>
        /// Push a render target onto the current rendering context, making it
        /// the active target for rendering.  By using the PushRenderTarget / PopRenderTarget
        /// methods, this allows you to safely chain render target switches, without risk
        /// of losing the previous render target.  An example of where this can be used is
        /// if you want to capture the next frame, you can simply start with a PushRenderTarget
        /// and as long as all other render target switching uses these methods or respects the
        /// previous render target, then everything will be captured as intended.
        /// </summary>
        /// <param name="renderTarget">The render target instance to make active.</param>
        void PushRenderTarget(RenderTarget2D renderTarget);

        /// <summary>
        /// Pops the current render target from the current rendering context.  If there are no more render targets
        /// in the stack after this call, then the rendering will default back to rendering to the back buffer.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the current render target does not match the top of the stack.  This indicates that there
        /// is other code calling SetRenderTarget, and changing the render target with this method may corrupt
        /// the rendering state.
        /// </exception>
        void PopRenderTarget();
    }
}
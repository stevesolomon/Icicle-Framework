using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using IcicleFramework.Renderables.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Renderables
{
    public enum AnimationState
    {
        Running,
        Paused,
        Stopped
    }

    public delegate void OnAnimationCompletedHandler(IAnimation source, string animationName, string customValue);

    public class AnimatedSprite : Sprite
    {
        protected Dictionary<string, IAnimation> animations;

         #region Internal Variables

        protected int spriteSheetRows;

        protected int spriteSheetCols;

        protected int currentFrame;

        /// <summary>
        /// The number of seconds between frame changes.
        /// </summary>
        protected float frameSpeed;

        /// <summary>
        /// The current state of animation for this AnimatedImage.
        /// </summary>
        protected AnimationState state;

        /// <summary>
        /// The time since the frame was last changed.
        /// </summary>
        protected float lastChangeTime;
        
        #endregion


        #region Properties

        public string ActiveAnimationName { get; protected set; }

        protected IAnimation ActiveAnimation { get; set; }

        /// <summary>
        /// Gets or sets the speed at which animation frames are swapped.
        /// </summary>
        public float FramesPerSecond
        {
            get { return frameSpeed; }
            set
            {
                if (value <= 0.0f)
                    value = 0.01f;

                frameSpeed = value;
                animations[ActiveAnimationName].FramesPerSecond = FramesPerSecond;
            }
        }

        #endregion


        #region Events

        public event OnAnimationCompletedHandler OnAnimationCompleted;

        #endregion


        #region Constructors

        public AnimatedSprite()
        {
            animations = new Dictionary<string, IAnimation>();
            this.state = AnimationState.Stopped;
        }

        protected AnimatedSprite(AnimatedSprite old)
            : base(old)
        {
            this.lastChangeTime = 0f;
            this.frameSpeed = old.frameSpeed;
            this.state = AnimationState.Running;

            this.animations = new Dictionary<string, IAnimation>();

            foreach (var anim in old.animations)
            {
                animations.Add(anim.Key, anim.Value.DeepClone());
            }
        }

        #endregion

        public override void Initialize()
        {
            foreach (var anim in animations.Values)
            {
                if (ActiveAnimation == null)
                {
                    ActiveAnimation = anim;
                    ActiveAnimationName = anim.Name;
                }
            }

            Source = ActiveAnimation.Frames[0];
            state = AnimationState.Running;

            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);

            foreach (var anim in animations.Values)
            {
                anim.BuildFrames(Texture, spriteSheetRows, spriteSheetCols);
            }
        }

        public void StartAnimation(string name)
        {
            if (animations.ContainsKey(name))
            {
                ActiveAnimationName = name;
                ActiveAnimation = animations[name];
            }

            state = AnimationState.Running;
            currentFrame = 0;
        }

        public void StopAnimation()
        {
            currentFrame = 0;
            var oldState = state;
            state = AnimationState.Stopped;

            //If we had an animation running at all, then let any listeners know that it's been completed.
            if (OnAnimationCompleted != null && oldState != AnimationState.Stopped)
            {
                OnAnimationCompleted(ActiveAnimation, ActiveAnimationName, ActiveAnimation.CompletedValue);
            }
        }

        public void PauseAnimation()
        {
            state = AnimationState.Paused;
        }

        public override void Update(GameTime gameTime)
        {
            if (state != AnimationState.Running || ActiveAnimation == null)
                return;

            lastChangeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Change the frame if it's time and we're not paused or stopped.
            if (lastChangeTime >= ActiveAnimation.FramesPerSecond)
            {
                currentFrame++;
                lastChangeTime = 0f;

                if (currentFrame == ActiveAnimation.TotalFrames)
                {
                    currentFrame = 0;

                    //If we're not looping then stop the animation outright.
                    if (!ActiveAnimation.Looping)
                    {
                        StopAnimation();
                    }
                }

                Source = ActiveAnimation.Frames[currentFrame];
            }

            base.Update(gameTime);
        }
        
        public override IRenderable DeepClone()
        {
            return new AnimatedSprite(this);
        }

        public override void Deserialize(XElement element)
        {
            int numRows = 1, numCols = 1;

            if (element.Element("numRows") != null)
                numRows = int.Parse(element.Element("numRows").Value, NumberStyles.Integer, CultureInfo.InvariantCulture);

            if (element.Element("numCols") != null)
                numCols = int.Parse(element.Element("numCols").Value, NumberStyles.Integer, CultureInfo.InvariantCulture);

            spriteSheetRows = numRows;
            spriteSheetCols = numCols;

            //Deserialize each IAnimation
            IEnumerable<XElement> animElems = element.Element("animations").Elements("animation");

            foreach (var animElem in animElems)
            {
                IAnimation anim = new SpriteAnimation();
                anim.Deserialize(animElem);
                animations.Add(anim.Name, anim);
            }

            base.Deserialize(element);
        }
    }
}

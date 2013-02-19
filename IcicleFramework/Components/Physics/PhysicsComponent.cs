using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.Physics;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Components.Physics
{
    public abstract class PhysicsComponent : BaseComponent, IPhysicsComponent
    {
        protected bool active;

        protected Vector2 oldPosition;

        protected Body primaryBody;

        protected Dictionary<string, Joint> copyJoints { get; set; }

        protected IPhysicsManager physicsManager;

        public override bool Active
        {
            get { return active; }
            set
            {
                if (value != active)
                {
                    if (Bodies != null)
                    {
                        foreach (Body body in Bodies.Values)
                            body.Enabled = value;
                    }

                    if (Joints != null)
                    {
                        foreach (Joint joint in Joints.Values)
                            joint.Enabled = value;
                    }

                    active = value;
                }
            }
        }

        public bool Solid { get; set; }

        public virtual int NumBodies
        {
            get { return Bodies.Count; }
        }

        public virtual Vector2 Velocity
        {
            get { return primaryBody.LinearVelocity; }
            set { primaryBody.LinearVelocity = value; }
        }

        public float MaxVelocity { get; set; }

        public Dictionary<string, Body> Bodies { get; set; }

        public Dictionary<string, Joint> Joints { get; set; }

        public PhysicsComponent()
        {
            Bodies = new Dictionary<string, Body>();
            Joints = new Dictionary<string, Joint>();
            copyJoints = new Dictionary<string, Joint>();

            Solid = true;
        }
        
        public override void Initialize()
        {
            Parent.OnMove += OnMove;

            physicsManager = GameServiceManager.GetService<IPhysicsManager>();

            foreach (var body in Bodies.Values)
            {
                body.Position = ConvertUnits.ToSimUnits(Parent.Position + ((FarseerUserData)body.UserData).Offset) -
                                ((FarseerUserData)body.UserData).TopLeftOffset;
                body.Enabled = true;
            }

            physicsManager.PhysicsWorld.Step(0.000001f);

            foreach (var joint in copyJoints)
            {
                var data = joint.Value.UserData as FarseerJointUserData;

                var bodyA = !string.IsNullOrWhiteSpace(data.BodyAName) &&
                            Bodies.ContainsKey(data.BodyAName)
                                ? Bodies[data.BodyAName]
                                : null;
                var bodyB = !string.IsNullOrWhiteSpace(data.BodyBName) &&
                            Bodies.ContainsKey(data.BodyBName)
                                ? Bodies[data.BodyBName]
                                : null;

                var newJoint = FarseerDeserialization.CopyJoint(joint.Value, bodyA, bodyB, physicsManager.PhysicsWorld);
                physicsManager.PhysicsWorld.AddJoint(newJoint);
                Joints.Add(joint.Key, newJoint);
            }

            copyJoints.Clear();
            
            base.Initialize();
        }

        public virtual Body GetBody(string name)
        {
            return Bodies.ContainsKey(name) ? Bodies[name] : null;
        }

        public virtual Body GetBody(int index)
        {
            int count = 0;

            if (index >= NumBodies)
                return null;
            
            foreach (Body body in Bodies.Values)
            {
                if (index == count)
                    return body;

                count++;
            }

            return null;
        }

        public virtual bool AddBody(Body body, string name)
        {
            bool added = false;

            if (!Bodies.ContainsKey(name))
            {
                Bodies.Add(name, body);

                //We can only ever have one primary body in an IPhysicsComponent.
                //Let's assume that if the new body coming in is Primary then the user
                //wants to replace the old primary body.
                if (primaryBody == null || ((FarseerUserData) body.UserData).Primary)
                    primaryBody = body;

                added = true;
            }

            return added;
        }

        public virtual Body GetFirstBody()
        {
            return NumBodies > 0 ? Bodies.Values.ToArray()[0] : null;
        }

        public virtual Body GetPrimaryBody()
        {
            //If we don't have a primary body registered for whatever reason, then search for one.
            if (primaryBody == null && Bodies != null)
            {
                foreach (Body body in Bodies.Values)
                {
                    if (((FarseerUserData) body.UserData).Primary)
                    {
                        primaryBody = body;
                        break;
                    }
                }

                //If none of the bodies were marked as primary then just grab the first body and be done with it.
                if (primaryBody == null)
                    primaryBody = GetFirstBody();
            }

            return primaryBody;
        }
        
        public virtual void ApplyImpulse(Vector2 impulse, string name)
        {
            Body body = GetBody(name);

            if (body != null)
                body.ApplyLinearImpulse(impulse);
        }

        public virtual void ApplyImpulse(Vector2 impulse)
        {
            foreach (Body body in Bodies.Values)
                body.ApplyLinearImpulse(impulse);
        }

        protected void OnMove(IGameObject sender)
        {
            //IGameObjects use a position located in the top-left corner of the object
            //and use world units, not sim units, so we need to convert both.
            if (Bodies != null)
            {
                Vector2 position = ConvertUnits.ToSimUnits(sender.Position);

                foreach (Body body in Bodies.Values)
                {
                    if (!body.IsStatic)
                    {
                        FarseerUserData userData = (FarseerUserData) body.UserData;
                        Vector2 myPosition = position - userData.TopLeftOffset + userData.Offset;

                        //If the position is different from the current position of the PhysicsBody then we need to 
                        //change the actual position of the PhysicsBody.
                        if (Math.Abs(myPosition.X - body.Position.X) > 0.0001f ||
                            Math.Abs(myPosition.Y - body.Position.Y) > 0.0001f)
                        {
                            //Change the position of the PhysicsBody...
                            body.Position = myPosition;
                        }
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Paused)
                return;

            //Update the position of our Parent to the current position of the PhysicsBody.
            //WTo do this we'll track down the first 'Primary' body that we can find.
            Body primary = null;

            if (Bodies != null)
            {
                primary = GetPrimaryBody();

                if (primary != null && (oldPosition.X != primary.Position.X || oldPosition.Y != primary.Position.Y))
                {
                    Vector2 position = primary.Position;
                    FarseerUserData userData = (FarseerUserData) primary.UserData;

                    position += userData.TopLeftOffset;
                    position = ConvertUnits.ToDisplayUnits(position);
                    Parent.Position = position;

                    oldPosition = primary.Position;

                    Vector2 newVelocity = primary.LinearVelocity;

                    if (Math.Abs(primary.LinearVelocity.X) > MaxVelocity)
                        newVelocity.X = MaxVelocity;

                    if (Math.Abs(primary.LinearVelocity.Y) > MaxVelocity)
                        newVelocity.Y = MaxVelocity;

                    primary.LinearVelocity = newVelocity;

                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Scales the bodies of this PhysicsComponent. Works okay but can't do anything for joints right now.
        /// I suggest you don't use it ;)
        /// </summary>
        public virtual void ApplyScale(float xScaling, float yScaling)
        {
            foreach (Body body in Bodies.Values)
                ScaleBody(body, xScaling, yScaling);
        }

        protected void ScaleBody(Body body, float xScaling, float yScaling)
        {
            foreach (Fixture fixture in body.FixtureList)
            {
                PolygonShape shape = (PolygonShape) fixture.Shape;
                Vector2 scale = new Vector2(xScaling, yScaling);
                shape.Vertices.Scale(ref scale); 
            }

        }

        public override void Deserialize(XElement element)
        {
            IPhysicsManager physicsManager = GameServiceManager.GetService<IPhysicsManager>();

            if (physicsManager != null && element != null)
            {
                //Read in the max velocity value.
                float maxVelocity = float.PositiveInfinity;
                if (element.Element("maxVelocity") != null)
                    float.TryParse(element.Element("maxVelocity").Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxVelocity);
                MaxVelocity = ConvertUnits.ToSimUnits(maxVelocity);

                if (element.Element("bodies") != null)
                    Bodies = FarseerDeserialization.DeserializeBodies(element.Element("bodies"), physicsManager, this);

                //Record the primary body
                primaryBody = GetPrimaryBody();

                foreach (var body in Bodies.Values)
                {
                    body.Enabled = false;
                    body.Position = new Vector2(-10f, -10f);
                }

                //A single-body physics component only supports joints that require only a single body (for obvious reasons...)
                //But we'll expect the FarseerDeserialization class to handle any user error with this.
                if (element.Element("joints") != null)
                {
                    this.Joints = FarseerDeserialization.DeserializeJoints(Parent, element.Element("joints"),
                                                                           physicsManager, Bodies);
                }
            }
        }

        public override void CopyInto(IBaseComponent newObject)
        {
            var physicsComponent = newObject as PhysicsComponent;

            Debug.Assert(physicsComponent != null, "physicsComponent != null");

            physicsComponent.MaxVelocity = MaxVelocity;
            physicsComponent.Solid = Solid;

            foreach (KeyValuePair<string, Body> bodyEntry in Bodies)
            {
                var body = bodyEntry.Value.DeepClone();
                ((FarseerUserData)body.UserData).Owner = physicsComponent;
                physicsComponent.AddBody(bodyEntry.Value.DeepClone(), bodyEntry.Key);
                physicsManager.RegisterBody(body);
            }

            foreach (KeyValuePair<string, Joint> joint in Joints)
            {
                physicsComponent.copyJoints.Add(joint.Key, joint.Value);
            }

            base.CopyInto(newObject);
        }
    }
}

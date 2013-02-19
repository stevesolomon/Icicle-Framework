using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using IcicleFramework.Components.Physics;
using IcicleFramework.Entities;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Physics
{
    public static class FarseerDeserialization
    {
        private static long jointNum = 0;

        private static long bodyNum = 0;

        #region Body Deserialization

        public static Dictionary<string, Body> DeserializeBodies(XElement element, IPhysicsManager physicsManager, IPhysicsComponent parent)
        {
            Dictionary<string, Body> bodies = new Dictionary<string, Body>();
            IEnumerable<XElement> elements = element.Elements("body");

            foreach (XElement bodyElement in elements)
            {
                Body body = DeserializeBody(bodyElement, physicsManager, parent);

                if (body != null)
                {
                    string name = ((FarseerUserData) body.UserData).Name;
                    bodies.Add(name, body);
                }
            }

            return bodies;
        }

        public static Body DeserializeBody(XElement element, IPhysicsManager physicsManager, IPhysicsComponent parent)
        {
            Body body = null;
            FarseerUserData userData = new FarseerUserData();
                
            //Read the shape used for this body.
            BodyType bodyType = BodyType.Static;
            if (element.Attribute("bodyType") != null)
            {
                if (element.Attribute("bodyType").Value.StartsWith("d", StringComparison.InvariantCultureIgnoreCase))
                    bodyType = BodyType.Dynamic;
                else if (element.Attribute("bodyType").Value.StartsWith("s", StringComparison.InvariantCultureIgnoreCase))
                    bodyType = BodyType.Static;
                else if (element.Attribute("bodyType").Value.StartsWith("k", StringComparison.InvariantCultureIgnoreCase))
                    bodyType = BodyType.Kinematic;
            }

            bool primary = false;
            if (element.Attribute("primary") != null)
                bool.TryParse(element.Attribute("primary").Value, out primary);

            Shape shape = null;
            XElement shapeElement = element.Element("shape");

            if (shapeElement != null && shapeElement.Attribute("type") != null)
            {
                if (shapeElement.Attribute("type").Value.StartsWith("b", StringComparison.InvariantCultureIgnoreCase))
                    shape = DeserializeBoxShape(shapeElement);
                else if (shapeElement.Attribute("type").Value.StartsWith("c", StringComparison.InvariantCultureIgnoreCase))
                    shape = DeserializeCircleShape(shapeElement);
                else if (shapeElement.Attribute("type").Value.StartsWith("p", StringComparison.InvariantCultureIgnoreCase))
                    shape = DeserializePolygonShape(shapeElement);
            }

            if (shape != null)
            {
                string name;

                body = BodyFactory.CreateBody(physicsManager.PhysicsWorld);
                body.BodyType = bodyType;
                Vector2 offset = Vector2.Zero;

                float friction = 0.5f, linearDamping = 5f, restitution = 0f, mass = 1f;
                bool fixedRotation = false;

                if (element.Element("friction") != null)
                    float.TryParse(element.Element("friction").Value, NumberStyles.Float, CultureInfo.InvariantCulture, out friction);

                if (element.Element("linearDamping") != null)
                    float.TryParse(element.Element("linearDamping").Value, NumberStyles.Float, CultureInfo.InvariantCulture, out linearDamping);

                if (element.Element("restitution") != null)
                    float.TryParse(element.Element("restitution").Value, NumberStyles.Float, CultureInfo.InvariantCulture, out restitution);

                if (element.Element("mass") != null)
                    float.TryParse(element.Element("mass").Value, NumberStyles.Float, CultureInfo.InvariantCulture, out mass);

                if (element.Element("fixedRotation") != null)
                    bool.TryParse(element.Element("fixedRotation").Value, out fixedRotation);

                if (element.Element("offset") != null)
                    offset = offset.DeserializeOffset(element.Element("offset"));
                    
                body.CreateFixture(shape);
                body.Friction = friction;
                body.LinearDamping = linearDamping;
                body.Restitution = restitution;
                body.Mass = mass;
                body.FixedRotation = fixedRotation;

                name = element.Attribute("name") != null ? element.Attribute("name").Value : body.BodyId.ToString();

                userData.TopLeftOffset = ConvertUnits.GetTopLeftCorner(body.FixtureList[0], 0);
                userData.Name = name;
                userData.Primary = primary;
                userData.Owner = parent;
                userData.Offset = offset;

                body.UserData = userData;
            }

            return body;
        }

        private static Shape DeserializePolygonShape(XElement element)
        {
            Vertices vertices = new Vertices();
            float density = DeserializeDensity(element);

            if (element.Element("vertices") != null)
            {
                IEnumerable<XElement> elements = element.Element("vertices").Elements("vertex");

                foreach (XElement vertexElement in elements)
                {
                    Vector2 vec = Vector2.Zero;
                    vec = vec.DeserializeOffset(vertexElement);
                    vec = ConvertUnits.ToSimUnits(vec);
                    vertices.Add(vec);
                }
            }
            
            return new PolygonShape(vertices, density);
        }

        private static Shape DeserializeBoxShape(XElement element)
        {
            int width = 1;
            int height = 1;

            if (element.Element("width") != null)
                int.TryParse(element.Element("width").Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out width);

            if (element.Element("height") != null)
                int.TryParse(element.Element("height").Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out height);

            float density = DeserializeDensity(element);

            Vertices vertices = PolygonTools.CreateRectangle(ConvertUnits.ToSimUnits(width / 2), ConvertUnits.ToSimUnits(height / 2));
            
            return new PolygonShape(vertices, density);
        }

        private static Shape DeserializeCircleShape(XElement element)
        {
            int radius = 1;

            if (element.Element("radius") != null)
                int.TryParse(element.Element("radius").Value, NumberStyles.Integer, CultureInfo.InvariantCulture,
                             out radius);

            float density = DeserializeDensity(element);
            
            return new CircleShape(ConvertUnits.ToSimUnits(radius), density);
        }

        private static float DeserializeDensity(XElement element)
        {
            float density = 1.0f;

            if (element.Element("density") != null)
                float.TryParse(element.Element("density").Value, NumberStyles.Float, CultureInfo.InvariantCulture, out density);

            return density;
        }

        #endregion


        #region Joint Deserialization

        public static Dictionary<string, Joint> DeserializeJoints(IGameObject parent, XElement element, IPhysicsManager physicsManager, Dictionary<string, Body> bodies)
        {
            Dictionary<string, Joint> joints = new Dictionary<string, Joint>();

            IEnumerable<XElement> jointElements = element.Elements("joint");

            foreach (XElement jointElement in jointElements)
            {
                Joint joint = DeserializeJoint(parent, jointElement, physicsManager, bodies);
                var name = jointNum.ToString(CultureInfo.InvariantCulture);

                var xAttribute = jointElement.Attribute("name");
                if (xAttribute != null) name = xAttribute.Value;

                joints.Add(name, joint);
            }

            return joints;
        }

        private static Joint DeserializeJoint(IGameObject parent, XElement jointElement, IPhysicsManager physicsManager, Dictionary<string, Body> bodies)
        {
            Joint joint = null;
            JointType type = JointType.Angle;
            Body bodyA = null, bodyB = null;
            bool collideConnected = false;
            World world = physicsManager.PhysicsWorld;

            var jointData = new FarseerJointUserData();

            if (jointElement.Attribute("type") != null)
                type = (JointType) Enum.Parse(typeof(JointType), jointElement.Attribute("type").Value, true);

            if (jointElement.Element("bodyA") != null)
            {
                var jointName = jointElement.Element("bodyA").Value;
                bodyA = bodies[jointName];
                jointData.BodyAName = jointName;
            }
                

            if (jointElement.Element("bodyB") != null)
            {
                var jointName = jointElement.Element("bodyB").Value;
                bodyB = bodies[jointName];
                jointData.BodyBName = jointName;
            }

            if (jointElement.Element("collideConnected") != null)
                bool.TryParse(jointElement.Element("collideConnected").Value, out collideConnected);

            #region Joint Types Instantiation

            if (bodyA != null)
            {
                switch (type)
                {
                    case JointType.Angle:
                        if (bodyB != null)
                            joint = DeserializeAngleJoint(jointElement, bodyA, bodyB, world);
                        break;
                    case JointType.Distance:
                        if (bodyB != null)
                            joint = DeserializeDistanceJoint(jointElement, bodyA, bodyB, world);
                        break;
                    case JointType.FixedAngle:
                        joint = DeserializeFixedAngleJoint(jointElement, bodyA, world);
                        break;
                    case JointType.FixedDistance:
                        joint = DeserializeFixedDistanceJoint(jointElement, bodyA, world);
                        break;
                    case JointType.FixedFriction:
                        joint = DeserializeFixedFrictionJoint(jointElement, bodyA, world);
                        break;
                    case JointType.FixedPrismatic:
                        joint = DeserializeFixedPrismaticJoint(jointElement, bodyA, parent, world);
                        break;
                    case JointType.FixedRevolute:
                        joint = DeserializeFixedRevoluteJoint(jointElement, bodyA, world);
                        break;
                    case JointType.Friction:
                        if (bodyB != null)
                            joint = DeserializeFrictionJoint(jointElement, bodyA, bodyB, world);
                        break;
                    case JointType.Line:
                        if (bodyB != null)
                            joint = DeserializeLineJoint(jointElement, bodyA, bodyB, world);
                        break;
                    case JointType.Prismatic:
                        if (bodyB != null)
                            joint = DeserializePrismaticJoint(jointElement, bodyA, bodyB, world);
                        break;
                    case JointType.Pulley:
                        if (bodyB != null)
                            joint = DeserializePulleyJoint(jointElement, bodyA, bodyB, world);
                        break;
                    case JointType.Revolute:
                        if (bodyB != null)
                            joint = DeserializeRevoluteJoint(jointElement, bodyA, bodyB, world);
                        break;
                    case JointType.Slider:
                        if (bodyB != null)
                            joint = DeserializeSliderJoint(jointElement, bodyA, bodyB, world);
                        break;
                    case JointType.Weld:
                        if (bodyB != null)
                            joint = DeserializeWeldJoint(jointElement, bodyA, bodyB, world);
                        break;
                }
            }
            #endregion

            if (joint != null)
                joint.CollideConnected = collideConnected;

            joint.UserData = jointData;

            return joint;
        }

        #region Individual Joint Type Deserialization

        private static Joint DeserializeDistanceJoint(XElement jointElement, Body bodyA, Body bodyB, World world)
        {
            DistanceJoint joint = null;
            Vector2 localAnchorA, localAnchorB;
            float dampingRatio = 0.0f, frequency = 0.0f, length = 0.0f;

            localAnchorA = bodyA.WorldCenter;
            localAnchorB = bodyB.WorldCenter;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "dampingratio":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out dampingRatio);
                        break;
                    case "frequency":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out frequency);
                        break;
                    case "length":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out length);
                        break;
                    case "localanchora":
                        localAnchorA = localAnchorA.DeserializeOffset(element);
                        localAnchorA = ConvertUnits.ToSimUnits(localAnchorA);
                        break;
                    case "localanchorb":
                        localAnchorB = localAnchorB.DeserializeOffset(element);
                        localAnchorB = ConvertUnits.ToSimUnits(localAnchorB);
                        break;
                }
            }
           
            joint = JointFactory.CreateDistanceJoint(world, bodyA, bodyB, localAnchorA, localAnchorB);
            joint.DampingRatio = dampingRatio;
            joint.Frequency = frequency;
            joint.Length = ConvertUnits.ToSimUnits(length);

            return joint;
        }

        private static Joint DeserializeFixedDistanceJoint(XElement jointElement, Body bodyA, World world)
        {
            FixedDistanceJoint joint = null;
            Vector2 localAnchor, worldAnchor;
            float dampingRatio = 0.0f, frequency = 0.0f, length = 0.0f;

            localAnchor = bodyA.WorldCenter;
            worldAnchor = Vector2.Zero;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "dampingratio":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out dampingRatio);
                        break;
                    case "frequency":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out frequency);
                        break;
                    case "length":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out length);
                        break;
                    case "localanchor":
                        localAnchor = localAnchor.DeserializeOffset(element);
                        localAnchor = ConvertUnits.ToSimUnits(localAnchor);
                        break;
                    case "worldanchor":
                        worldAnchor = worldAnchor.DeserializeOffset(element);
                        worldAnchor = ConvertUnits.ToSimUnits(worldAnchor);
                        break;
                }
            }

            joint = JointFactory.CreateFixedDistanceJoint(world, bodyA, localAnchor, worldAnchor);
            joint.DampingRatio = dampingRatio;
            joint.Frequency = frequency;
            joint.Length = length;

            return joint;
        }

        private static Joint DeserializeFixedFrictionJoint(XElement jointElement, Body bodyA, World world)
        {
            FixedFrictionJoint joint = null;
            Vector2 localAnchorA;
            float maxForce = 0.0f, maxTorque = 0.0f;

            localAnchorA = bodyA.WorldCenter;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "maxforce":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxForce);
                        break;
                    case "maxtorque":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxTorque);
                        break;
                    case "localanchora":
                        localAnchorA = localAnchorA.DeserializeOffset(element);
                        localAnchorA = ConvertUnits.ToSimUnits(localAnchorA);
                        break;
                }
            }

            joint = JointFactory.CreateFixedFrictionJoint(world, bodyA, localAnchorA);
            joint.MaxForce = maxForce;
            joint.MaxTorque = maxTorque;

            return joint;
        }

        private static Joint DeserializeFrictionJoint(XElement jointElement, Body bodyA, Body bodyB, World world)
        {
            FrictionJoint joint = null;
            Vector2 localAnchorA, localAnchorB;
            float maxForce = 0.0f, maxTorque = 0.0f;

            localAnchorA = bodyA.WorldCenter;
            localAnchorB = bodyB.WorldCenter;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "maxforce":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxForce);
                        break;
                    case "maxtorque":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxTorque);
                        break;
                    case "localanchora":
                        localAnchorA = localAnchorA.DeserializeOffset(element);
                        localAnchorA = ConvertUnits.ToSimUnits(localAnchorA);
                        break;
                    case "localanchorb":
                        localAnchorB = localAnchorB.DeserializeOffset(element);
                        localAnchorB = ConvertUnits.ToSimUnits(localAnchorB);
                        break;
                }
            }

            joint = JointFactory.CreateFrictionJoint(world, bodyA, bodyB, localAnchorA, localAnchorB);
            joint.MaxForce = maxForce;
            joint.MaxTorque = maxTorque;

            return joint;
        }

        private static Joint DeserializeLineJoint(XElement jointElement, Body bodyA, Body bodyB, World world)
        {
            LineJoint joint = null;
            Vector2 localAnchorA, localXAxis = Vector2.Zero;
            float motorSpeed = 0.0f, dampingRatio = 0.0f, maxTorque = 0.0f, frequency = 0.0f;
            bool enableMotor = false;

            localAnchorA = bodyA.WorldCenter;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "enablemotor":
                        bool.TryParse(element.Value, out enableMotor);
                        break;
                    case "motorspeed":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out motorSpeed);
                        break;
                    case "dampingratio":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out dampingRatio);
                        break;
                    case "maxtorque":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxTorque);
                        break;
                    case "frequency":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out frequency);
                        break;
                    case "localxaxis":
                        localXAxis = localXAxis.DeserializeOffset(element);
                        localXAxis = ConvertUnits.ToSimUnits(localXAxis);
                        break;
                    case "localanchora":
                        localAnchorA = localAnchorA.DeserializeOffset(element);
                        localAnchorA = ConvertUnits.ToSimUnits(localAnchorA);
                        break;
                }
            }

            joint = JointFactory.CreateLineJoint(world, bodyA, bodyB, localAnchorA, localXAxis);
            joint.MotorEnabled = enableMotor;
            joint.MotorSpeed = motorSpeed;
            joint.DampingRatio = dampingRatio;
            joint.MaxMotorTorque = maxTorque;
            joint.Frequency = frequency;
            joint.LocalXAxis = localXAxis;

            return joint;
        }

        private static Joint DeserializePrismaticJoint(XElement jointElement, Body bodyA, Body bodyB, World world)
        {
            PrismaticJoint joint = null;
            Vector2 localAnchorA, localXAxis = Vector2.Zero;
            float maxMotorForce = 0.0f, motorSpeed = 0.0f, lowerLimit = 0.0f, upperLimit = 0.0f, referenceAngle = 0.0f;
            bool enableLimit = true, enableMotor = false;

            localAnchorA = bodyA.WorldCenter;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "enablelimit":
                        bool.TryParse(element.Value, out enableLimit);
                        break;
                    case "enablemotor":
                        bool.TryParse(element.Value, out enableMotor);
                        break;
                    case "maxmotorforce":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxMotorForce);
                        break;
                    case "motorspeed":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out motorSpeed);
                        break;
                    case "lowerlimit":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out lowerLimit);
                        lowerLimit = ConvertUnits.ToSimUnits(lowerLimit);
                        break;
                    case "upperlimit":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out upperLimit);
                        upperLimit = ConvertUnits.ToSimUnits(upperLimit);
                        break;
                    case "referenceangle":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out referenceAngle);
                        break;
                    case "localxaxis":
                        localXAxis = localXAxis.DeserializeOffset(element);
                        localXAxis = ConvertUnits.ToSimUnits(localXAxis);
                        break;
                    case "localanchora":
                        localAnchorA = localAnchorA.DeserializeOffset(element);
                        localAnchorA = ConvertUnits.ToSimUnits(localAnchorA);
                        break;
                }
            }
            
            joint = JointFactory.CreatePrismaticJoint(world, bodyA, bodyB, localAnchorA, localXAxis);

            joint.LimitEnabled = enableLimit;
            joint.MotorEnabled = enableMotor;
            joint.MaxMotorForce = maxMotorForce;
            joint.MotorSpeed = motorSpeed;
            joint.LowerLimit = lowerLimit;
            joint.UpperLimit = upperLimit;
            joint.ReferenceAngle = referenceAngle;
            joint.LocalXAxis1 = localXAxis;

            return joint;
        }

        private static Joint DeserializeFixedPrismaticJoint(XElement jointElement, Body bodyA, IGameObject parent, World world)
        {
            FixedPrismaticJoint joint = null;
            Vector2 worldAnchor = Vector2.Zero, axis = Vector2.Zero;
            float lowerLimit = 0.0f, upperLimit = 0.0f, motorSpeed = 0.0f, maxMotorForce = 0.0f;
            bool motorEnabled = false;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "axis":
                        axis = axis.DeserializeOffset(element);
                        break;
                    case "worldanchora":
                        worldAnchor = worldAnchor.DeserializeOffset(element);
                        worldAnchor = ConvertUnits.ToSimUnits(worldAnchor);
                        break;
                    case "lowerlimit":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out lowerLimit);
                        lowerLimit = ConvertUnits.ToSimUnits(lowerLimit);
                        break;
                    case "upperlimit":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out upperLimit);
                        upperLimit = ConvertUnits.ToSimUnits(upperLimit);
                        break;
                    case "motorspeed":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out motorSpeed);
                        break;
                    case "maxmotorforce":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxMotorForce);
                        break;
                    case "motorenabled":
                        bool.TryParse(element.Value, out motorEnabled);
                        break;
                }
            }

            if (worldAnchor == Vector2.Zero)
            {
                Vector2 offset = ((FarseerUserData) bodyA.UserData).TopLeftOffset +
                                 ((FarseerUserData) bodyA.UserData).Offset;
                worldAnchor = ConvertUnits.ToSimUnits(parent.Position) - offset;
            }
            joint = JointFactory.CreateFixedPrismaticJoint(world, bodyA, worldAnchor, axis);

            joint.UpperLimit = upperLimit;
            joint.LowerLimit = lowerLimit;
            joint.MotorSpeed = motorSpeed;
            joint.MaxMotorForce = maxMotorForce;
            joint.MotorEnabled = motorEnabled;

            if (upperLimit != 0.0f || lowerLimit != 0.0f)
                joint.LimitEnabled = true;

            return joint;
        }

        private static Joint DeserializePulleyJoint(XElement jointElement, Body bodyA, Body bodyB, World world)
        {
            PulleyJoint joint = null;
            Vector2 groundAnchorA = Vector2.Zero, groundAnchorB = Vector2.Zero, localAnchorA, localAnchorB;
            float lengthA = 0.0f, lengthB = 0.0f, maxLengthA = 0.0f, maxLengthB = 0.0f, ratio = 0.0f;

            localAnchorA = bodyA.WorldCenter;
            localAnchorB = bodyB.WorldCenter;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "lengtha":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out lengthA);
                        lengthA = ConvertUnits.ToSimUnits(lengthA);
                        break;
                    case "lengthb":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out lengthB);
                        lengthB = ConvertUnits.ToSimUnits(lengthB);
                        break;
                    case "maxlengtha":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxLengthA);
                        maxLengthA = ConvertUnits.ToSimUnits(maxLengthA);
                        break;
                    case "maxlengthb":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxLengthB);
                        maxLengthB = ConvertUnits.ToSimUnits(maxLengthB);
                        break;
                    case "ratio":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out ratio);
                        break;
                    case "groundanchora":
                        groundAnchorA = groundAnchorA.DeserializeOffset(element);
                        groundAnchorA = ConvertUnits.ToSimUnits(groundAnchorA);
                        break;
                    case "groundanchorb":
                        groundAnchorB = groundAnchorB.DeserializeOffset(element);
                        groundAnchorB = ConvertUnits.ToSimUnits(groundAnchorB);
                        break;
                    case "localanchora":
                        localAnchorA = localAnchorA.DeserializeOffset(element);
                        localAnchorA = ConvertUnits.ToSimUnits(localAnchorA);
                        break;
                    case "localanchorb":
                        localAnchorB = localAnchorB.DeserializeOffset(element);
                        localAnchorB = ConvertUnits.ToSimUnits(localAnchorB);
                        break;
                }
            }

            joint = JointFactory.CreatePulleyJoint(world, bodyA, bodyB, groundAnchorA, groundAnchorB, localAnchorA, localAnchorB, ratio);
            joint.LengthA = lengthA;
            joint.LengthB = lengthB;
            joint.MaxLengthA = maxLengthA;
            joint.MaxLengthB = maxLengthB;

            return joint;
        }

        private static Joint DeserializeFixedRevoluteJoint(XElement jointElement, Body bodyA, World world)
        {
            FixedRevoluteJoint joint = null;
            Vector2 localAnchor, worldAnchor;
            float maxMotorTorque = 0.0f, motorSpeed = 0.0f, lowerAngle = 0.0f, upperAngle = 0.0f, referenceAngle = 0.0f;
            bool enableLimit = false, enableMotor = false;

            localAnchor = bodyA.WorldCenter;
            worldAnchor = Vector2.Zero;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "enablelimit":
                        bool.TryParse(element.Value, out enableLimit);
                        break;
                    case "enablemotor":
                        bool.TryParse(element.Value, out enableMotor);
                        break;
                    case "maxmotortorque":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxMotorTorque);
                        break;
                    case "motorspeed":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out motorSpeed);
                        motorSpeed = ConvertUnits.ToSimUnits(motorSpeed);
                        break;
                    case "lowerangle":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out lowerAngle);
                        break;
                    case "upperangle":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out upperAngle);
                        break;
                    case "referenceangle":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out referenceAngle);
                        break;
                    case "localanchora":
                        localAnchor = localAnchor.DeserializeOffset(element);
                        localAnchor = ConvertUnits.ToSimUnits(localAnchor);
                        break;
                    case "worldanchor":
                        worldAnchor = worldAnchor.DeserializeOffset(element);
                        worldAnchor = ConvertUnits.ToSimUnits(worldAnchor);
                        break;
                }
            }

            joint = JointFactory.CreateFixedRevoluteJoint(world, bodyA, localAnchor, worldAnchor);
            joint.LimitEnabled = enableLimit;
            joint.MotorEnabled = enableMotor;
            joint.MaxMotorTorque = maxMotorTorque;
            joint.MotorSpeed = motorSpeed;
            joint.LowerLimit = lowerAngle;
            joint.UpperLimit = upperAngle;
            joint.ReferenceAngle = referenceAngle;

            return joint;
        }

        private static Joint DeserializeRevoluteJoint(XElement jointElement, Body bodyA, Body bodyB, World world)
        {
            RevoluteJoint joint = null;
            Vector2 localAnchorA, localAnchorB;
            float maxMotorTorque = 0.0f, motorSpeed = 0.0f, lowerAngle = 0.0f, upperAngle = 0.0f, referenceAngle = 0.0f;
            bool enableLimit = false, enableMotor = false;

            localAnchorA = bodyA.WorldCenter;
            localAnchorB = bodyB.WorldCenter;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "enablelimit":
                        bool.TryParse(element.Value, out enableLimit);
                        break;
                    case "enablemotor":
                        bool.TryParse(element.Value, out enableMotor);
                        break;
                    case "maxmotortorque":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxMotorTorque);
                        break;
                    case "motorspeed":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out motorSpeed);
                        motorSpeed = ConvertUnits.ToSimUnits(motorSpeed);
                        break;
                    case "lowerangle":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out lowerAngle);
                        break;
                    case "upperangle":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out upperAngle);
                        break;
                    case "referenceangle":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out referenceAngle);
                        break;
                    case "localanchora":
                        localAnchorA = localAnchorA.DeserializeOffset(element);
                        localAnchorA = ConvertUnits.ToSimUnits(localAnchorA);
                        break;
                    case "localanchorb":
                        localAnchorB = localAnchorB.DeserializeOffset(element);
                        localAnchorB = ConvertUnits.ToSimUnits(localAnchorB);
                        break;
                }
            }

            joint = JointFactory.CreateRevoluteJoint(world, bodyA, bodyB, localAnchorA);
            joint.LimitEnabled = enableLimit;
            joint.MotorEnabled = enableMotor;
            joint.MaxMotorTorque = maxMotorTorque;
            joint.MotorSpeed = motorSpeed;
            joint.LowerLimit = lowerAngle;
            joint.UpperLimit = upperAngle;
            joint.ReferenceAngle = referenceAngle;

            return joint;
        }

        private static Joint DeserializeWeldJoint(XElement jointElement, Body bodyA, Body bodyB, World world)
        {
            WeldJoint joint = null;
            Vector2 localAnchorA, localAnchorB;

            localAnchorA = bodyA.WorldCenter;
            localAnchorB = bodyB.WorldCenter;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "localanchora":
                        localAnchorA = localAnchorA.DeserializeOffset(element);
                        localAnchorA = ConvertUnits.ToSimUnits(localAnchorA);
                        break;
                    case "localanchorb":
                        localAnchorB = localAnchorB.DeserializeOffset(element);
                        localAnchorB = ConvertUnits.ToSimUnits(localAnchorB);
                        break;
                }
            }

            joint = JointFactory.CreateWeldJoint(world, bodyA, bodyB, localAnchorA, localAnchorB);

            return joint;
        }

        private static Joint DeserializeRopeJoint(XElement jointElement, Body bodyA, Body bodyB, World world)
        {
            RopeJoint joint = null;
            Vector2 localAnchorA, localAnchorB;
            float maxLength = 100.0f;

            localAnchorA = bodyA.WorldCenter;
            localAnchorB = bodyB.WorldCenter;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "maxlength":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxLength);
                        maxLength = ConvertUnits.ToSimUnits(maxLength);
                        break;
                    case "localanchora":
                        localAnchorA = localAnchorA.DeserializeOffset(element);
                        localAnchorA = ConvertUnits.ToSimUnits(localAnchorA);
                        break;
                    case "localanchorb":
                        localAnchorB = localAnchorB.DeserializeOffset(element);
                        localAnchorB = ConvertUnits.ToSimUnits(localAnchorB);
                        break;
                }
            }

            joint = new RopeJoint(bodyA, bodyB, localAnchorA, localAnchorB);
            joint.MaxLength = maxLength;
            world.AddJoint(joint);

            return joint;
        }

        private static Joint DeserializeFixedAngleJoint(XElement jointElement, Body bodyA, World world)
        {
            FixedAngleJoint joint = null;
            float biasFactor = 0.0f, maxImpulse = 0.0f, softness = 0.0f, targetAngle = 0.0f;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "biasfactor":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out biasFactor);
                        break;
                    case "maximpulse":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxImpulse);
                        break;
                    case "softness":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out softness);
                        break;
                    case "targetangle":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out targetAngle);
                        break;
                }
            }

            joint = JointFactory.CreateFixedAngleJoint(world, bodyA);
            joint.BiasFactor = biasFactor;
            joint.MaxImpulse = maxImpulse;
            joint.Softness = softness;
            joint.TargetAngle = targetAngle;

            return joint;
        }

        private static Joint DeserializeAngleJoint(XElement jointElement, Body bodyA, Body bodyB, World world)
        {
            AngleJoint joint = null;
            float biasFactor = 0.0f, maxImpulse = 0.0f, softness = 0.0f, targetAngle = 0.0f;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "biasfactor":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out biasFactor);
                        break;
                    case "maximpulse":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxImpulse);
                        break;
                    case "softness":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out softness);
                        break;
                    case "targetangle":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out targetAngle);
                        break;
                }
            }

            joint = JointFactory.CreateAngleJoint(world, bodyA, bodyB);
            joint.BiasFactor = biasFactor;
            joint.MaxImpulse = maxImpulse;
            joint.Softness = softness;
            joint.TargetAngle = targetAngle;

            return joint;
        }

        private static Joint DeserializeSliderJoint(XElement jointElement, Body bodyA, Body bodyB, World world)
        {
            SliderJoint joint = null;
            Vector2 localAnchorA, localAnchorB;
            float minLength = 0.0f, maxLength = 100.0f, dampingRatio = 0.0f, frequency = 0.0f;

            localAnchorA = bodyA.WorldCenter;
            localAnchorB = bodyB.WorldCenter;

            foreach (XElement element in jointElement.Elements())
            {
                switch (element.Name.ToString().ToLower())
                {
                    case "minlength":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out minLength);
                        break;
                    case "maxlength":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out maxLength);
                        break;
                    case "dampingratio":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out dampingRatio);
                        break;
                    case "frequency":
                        float.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out frequency);
                        break;
                    case "localanchora":
                        localAnchorA = localAnchorA.DeserializeOffset(element);
                        localAnchorA = ConvertUnits.ToSimUnits(localAnchorA);
                        break;
                    case "localanchorb":
                        localAnchorB = localAnchorB.DeserializeOffset(element);
                        localAnchorB = ConvertUnits.ToSimUnits(localAnchorB);
                        break;
                }
            }

            joint = JointFactory.CreateSliderJoint(world, bodyA, bodyB, localAnchorA, localAnchorB, minLength, maxLength);
            joint.DampingRatio = dampingRatio;
            joint.Frequency = frequency;

            return joint;
        }

        #endregion

        #endregion
    
        public static Joint CopyJoint(Joint joint, Body bodyA, Body bodyB, World world)
        {
            Joint newJoint = null;

            switch (joint.JointType)
            {
                case JointType.Angle:
                    newJoint = JointFactory.CreateAngleJoint(world, bodyA, bodyB);
                    break;
                case JointType.Distance:
                    newJoint = new DistanceJoint(bodyA, bodyB, bodyA.WorldCenter, bodyB.WorldCenter);
                    break;
                case JointType.FixedAngle:
                    newJoint = new FixedAngleJoint(bodyA);
                    break;
                case JointType.FixedDistance:
                    newJoint = new FixedDistanceJoint(bodyA, bodyA.WorldCenter, Vector2.Zero);
                    break;
                case JointType.FixedFriction:
                    newJoint = new FixedFrictionJoint(bodyA, bodyA.WorldCenter);
                    break;
                case JointType.FixedPrismatic:
                    var fpJoint = joint as FixedPrismaticJoint;
                    var fpAxis = fpJoint.LocalXAxis1;
                    newJoint = new FixedPrismaticJoint(bodyA, bodyA.WorldCenter, fpAxis);
                    break;
                case JointType.FixedRevolute:
                    newJoint = new FixedRevoluteJoint(bodyA, bodyA.WorldCenter, Vector2.Zero);
                    break;
                case JointType.Friction:
                    newJoint = new FrictionJoint(bodyA, bodyB, bodyA.WorldCenter, bodyB.WorldCenter);
                    break;
                case JointType.Line:
                    var lineJoint = joint as LineJoint;
                    var axis = lineJoint.LocalXAxis;
                    newJoint = new LineJoint(bodyA, bodyB, bodyA.WorldCenter, axis);
                    break;
                case JointType.Prismatic:
                    var pJoint = joint as PrismaticJoint;
                    var pAxis = pJoint.LocalXAxis1;
                    newJoint = new PrismaticJoint(bodyA, bodyB, bodyA.WorldCenter, bodyB.WorldCenter, pAxis);
                    ((PrismaticJoint)newJoint).LimitEnabled = pJoint.LimitEnabled;
                    ((PrismaticJoint)newJoint).MotorEnabled = pJoint.MotorEnabled;
                    ((PrismaticJoint)newJoint).MaxMotorForce = pJoint.MaxMotorForce;
                    ((PrismaticJoint)newJoint).MotorSpeed = pJoint.MotorSpeed;
                    ((PrismaticJoint)newJoint).LowerLimit = pJoint.LowerLimit;
                    ((PrismaticJoint)newJoint).UpperLimit = pJoint.UpperLimit;
                    ((PrismaticJoint)newJoint).ReferenceAngle = pJoint.ReferenceAngle;
                    ((PrismaticJoint)newJoint).LocalXAxis1 = pJoint.LocalXAxis1;
                    break;
                case JointType.Pulley:
                    var pulleyJoint = joint as PulleyJoint;
                    var ratio = pulleyJoint.Ratio;
                    newJoint = new PulleyJoint(bodyA, bodyB, Vector2.Zero, Vector2.Zero, bodyA.WorldCenter, bodyB.WorldCenter, ratio);
                    break;
                case JointType.Revolute:
                    newJoint = new RevoluteJoint(bodyA, bodyB, bodyA.WorldCenter, bodyB.WorldCenter);
                    break;
                case JointType.Slider:
                    var sliderJoint = joint as SliderJoint;
                    var minLength = sliderJoint.MinLength;
                    var maxLength = sliderJoint.MaxLength;
                    newJoint = new SliderJoint(bodyA, bodyB, bodyA.WorldCenter, bodyB.WorldCenter, minLength, maxLength);
                    break;
                case JointType.Weld:
                    newJoint = new WeldJoint(bodyA, bodyB, bodyA.WorldCenter, bodyB.WorldCenter);
                    break;
            }

            var data = new FarseerJointUserData();
            data.BodyAName = ((FarseerJointUserData) joint.UserData).BodyAName;
            data.BodyBName = ((FarseerJointUserData) joint.UserData).BodyBName;

            joint.UserData = data;

            return newJoint;
        }
    
    }
}

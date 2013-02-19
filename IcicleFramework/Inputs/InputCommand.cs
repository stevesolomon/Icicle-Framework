using Microsoft.Xna.Framework.Input;

namespace IcicleFramework.Inputs
{
    public class InputCommand
    {
        public string Name { get; protected set; }

        public Keys Key { get; set; }

        public Buttons Button { get; set; }

        public IInputHandler InputHandler { get; set; }
        
        public InputCommand(string name)
        {
            Name = name;
        }
    }
}

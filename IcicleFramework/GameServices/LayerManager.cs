using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using IcicleFramework.GameServices;
using Microsoft.Xna.Framework.Content;

namespace IcicleFramework
{
    public class LayerManager : GameService, ILayerManager
    {
        private bool[,] layers;

        private static int currIndex = 0;

        private Dictionary<string, int> layerNameToIndexMapping;

        private XDocument xmlDoc;

        public LayerManager(string layerXmlPath, ContentManager content)
        {
            var fullXmlPath = Path.Combine(content.RootDirectory, layerXmlPath);
            xmlDoc = XDocument.Load(fullXmlPath);
        }

        public override void Initialize()
        {
            DeserializeLayers();
            DeserializeInteractions();

            base.Initialize();
        }

        public bool LayersInteract(string layer1, string layer2)
        {
            if (layerNameToIndexMapping.ContainsKey(layer1) && layerNameToIndexMapping.ContainsKey(layer2))
            {
                return layers[layerNameToIndexMapping[layer1], layerNameToIndexMapping[layer2]];
            }

            return false;
        }

        private void DeserializeLayers()
        {
            var layerElements = xmlDoc.Element("layerDefinitions").Element("layers").Elements("layer");
            var elems = layerElements.ToList();
            var numElems = elems.Count;

            layers = new bool[numElems, numElems];

            InitializeLayers();

            layerNameToIndexMapping = new Dictionary<string, int>(numElems);

            //First pass to build up a layer name to index mapping...
            foreach (var layerElem in elems)
            {
                string name = currIndex.ToString(CultureInfo.InvariantCulture);

                //Parse the name and add it to the layer index mapping
                var elem = layerElem.Attribute("name");
                if (elem != null)
                {
                    name = elem.Value.ToLowerInvariant();
                }

                layerNameToIndexMapping.Add(name, currIndex);
                    
                currIndex++;
            }
        }

        private void DeserializeInteractions()
        {
            var interactionElements = xmlDoc.Element("layerDefinitions").Element("interactions").Elements("interaction");
            
            foreach (var interaction in interactionElements)
            {
                string layer1 = interaction.Attribute("layer1").Value.ToLowerInvariant();
                string layer2 = interaction.Attribute("layer2").Value.ToLowerInvariant();

                int index1 = 0;
                int index2 = 0;

                if (layerNameToIndexMapping.ContainsKey(layer1))
                    index1 = layerNameToIndexMapping[layer1];

                if (layerNameToIndexMapping.ContainsKey(layer2))
                    index2 = layerNameToIndexMapping[layer2];

                layers[index1, index2] = true;
                layers[index2, index1] = true;
            }
        }

        private void InitializeLayers()
        {
            for(var i = 0; i < layers.GetUpperBound(0); i++)
            {
                for (var j = 0; j < layers.GetUpperBound(1); j++)
                {
                    layers[i, j] = false;
                }
            }
        }
    }
}

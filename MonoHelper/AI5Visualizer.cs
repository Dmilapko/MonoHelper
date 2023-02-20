using AILib;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonogameLabel;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoHelper
{
    public class AI5Visualizer : Evo0Visualiser
    { 
        public class NeuronVisualizer:Evo0NeuronVisualizer
        {
            public SpriteFont textfont;
            int texture_width;

            public override void CreateNewConnectionVisualizer(AIFamilyEvo0.ConnectionFamilyEvo0 connection)
            {
                connectionvisualizers.Add(new AI5ConnectionVisualizer(connection));
            }

            public NeuronVisualizer(AIFamilyEvo0.NeuronFamilyEvo0 neuron, int texture_width) : base(neuron)
            {
                this.texture_width = texture_width;
            }

            public override void Draw(SpriteBatch spriteBatch, Vector2 draw_position)
            {
                string text = neuron.value.ToString();
                if (text.Count() == 1) text += ".";
                if (text.Count() > 4) text = text.Substring(0, 4);
                else
                {
                    while (text.Count() != 4) text += "0";
                }
                float text_width = textfont.MeasureString(text).X;
                float text_height = textfont.MeasureString(text).Y;
                float r = (texture_width-5) / text_width;
                spriteBatch.DrawString(textfont, text, draw_position + pos.ToVector2(), Color.Red, 0f, new Vector2(text_width / 2, text_height / 2), r, SpriteEffects.None, 1);
            }
        }

        internal class AI5ConnectionVisualizer:Evo0ConnectionVisualizer
        {
            public AI5ConnectionVisualizer(AIFamilyEvo0.ConnectionFamilyEvo0 connection) : base(connection)
            {
            }
        }

        public SpriteFont textfont;

        public AI5Visualizer(GraphicsDevice graphicsDevice, AIFamilyEvo0 ai, int widthpx, int heightpx, SpriteFont textfont) :base(graphicsDevice, ai, widthpx, heightpx)
        {
            foreach (NeuronVisualizer item in neuronvisualizers)
            {
                item.textfont = textfont;
            }
        }

        public override void PreProcess()
        {
            foreach (NeuronVisualizer nvis in neuronvisualizers)
            {
                nvis.color = new Color(0, 0.5f + (float)nvis.neuron.value / 2f, 1 - (0.5f + (float)nvis.neuron.value / 2f));
                //nvis.color = new Color(0, (float)nvis.neuron.value, -(float)nvis.neuron.value);
            }
        }

        public override void CreateNewNeuronVisualizer(AIFamilyEvo0.NeuronFamilyEvo0 neuron)
        {
            neuronvisualizers.Add(new NeuronVisualizer(neuron, neurontexture.Width));
        }
    }
}

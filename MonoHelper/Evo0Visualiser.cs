using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AILib;
using System.Runtime.InteropServices;

namespace MonoHelper
{
    public abstract class Evo0Visualiser
    {
        public abstract class Evo0NeuronVisualizer
        {
            public double size = 1;
            public PointD pos;
            internal AIFamilyEvo0.NeuronFamilyEvo0 neuron;
            internal Color color;
            internal bool selected = false;
            internal List<Evo0ConnectionVisualizer> connectionvisualizers = new List<Evo0ConnectionVisualizer>();

            public abstract void CreateNewConnectionVisualizer(AIFamilyEvo0.ConnectionFamilyEvo0 connection);

            public Evo0NeuronVisualizer(AIFamilyEvo0.NeuronFamilyEvo0 neuron)
            {
                this.neuron = neuron;
            }

            public virtual void Draw(SpriteBatch spriteBatch, Vector2 draw_position, Color textcolor)
            {
            }
        }

        internal abstract class Evo0ConnectionVisualizer
        {
            double size = 1;
            public PointD pos;
            internal AIFamilyEvo0.ConnectionFamilyEvo0 connection;

            public Evo0ConnectionVisualizer(AIFamilyEvo0.ConnectionFamilyEvo0 connection)
            {
                this.connection = connection;
            }

            public virtual void Draw(SpriteBatch spriteBatch, Vector2 draw_position) { }
        }

        internal Texture2D neurontexture;
        internal Texture2D connectiontexture;
        internal Color textcolor = Color.Red, connectioncolor = Color.Pink;
        internal List<Evo0NeuronVisualizer> neuronvisualizers = new List<Evo0NeuronVisualizer>();
        public Vector2 drawposition;

        public Evo0Visualiser(GraphicsDevice graphicsDevice, AIFamilyEvo0 ai, int widthpx, int heightpx)
        { 
            int ns = (int)(heightpx / (double)Math.Max(ai.input_count + 1, ai.output_count + 1));
            neurontexture = MHeleper.CreateCircle(graphicsDevice, (int)(ns / 2d - 0.1 * ns), Color.White);
            Color[] cd = new Color[1];
            cd[0] = Color.White;
            connectiontexture = new Texture2D(graphicsDevice, 1, 1);
            connectiontexture.SetData(cd);
            foreach (var neuron in ai.neurons)
            {
                CreateNewNeuronVisualizer(neuron);
                foreach (var connection in neuron.connections) neuronvisualizers[neuronvisualizers.Count-1].CreateNewConnectionVisualizer(connection);
            }
            for (int i = 0; i < ai.input_count; i++)
            {
                neuronvisualizers[i].pos = new PointD(ns, (i + 0.5d) * heightpx / ai.input_count);
            }
            for (int i = 0; i < ai.output_count; i++)
            {
                neuronvisualizers[ai.input_count + i].pos = new PointD(widthpx - ns, (i + 0.5d) * heightpx / ai.output_count);
            }
            foreach (AIFamilyEvo0.HistoryEvent history_event in ai.history)
            {
                if (history_event.GetType().Equals(typeof(AIFamilyEvo0.AddedNeuron)))
                {
                    AIFamilyEvo0.AddedNeuron add_event = (AIFamilyEvo0.AddedNeuron)history_event;
                    neuronvisualizers[add_event.neuron_id].pos = (neuronvisualizers[add_event.from].pos + neuronvisualizers[add_event.to].pos) / 2d;
                }
            }
        }

        public abstract void CreateNewNeuronVisualizer(AIFamilyEvo0.NeuronFamilyEvo0 neuron);

        public virtual void Check(MouseState mouse, KeyboardState keyboard)
        {
            PointD mousepoint = new PointD(mouse.X, mouse.Y) - drawposition.ToPointD();
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                foreach (var nvis in neuronvisualizers)
                {
                    if ((mousepoint - nvis.pos).Length() < neurontexture.Width/2)
                    {
                        nvis.pos = mousepoint;
                        nvis.selected = true;
                        break;
                    }
                }
            }
        }

        public virtual void PreProcess()
        {

        }
        
        public void DrawNetwork(SpriteBatch spriteBatch)
        {
            PreProcess();
            foreach (var neuronvis in neuronvisualizers)
            {
                if (neuronvis.neuron.alive)
                {
                    foreach (var convis in neuronvis.connectionvisualizers)
                    {
                        PointD pos = neuronvis.pos - neuronvisualizers[convis.connection.to_neuron].pos;
                        float blackkoef = Math.Min((float)((neurontexture.Width / 2 + 10) / pos.Length()), 1);
                        spriteBatch.Draw(connectiontexture, drawposition + neuronvis.pos.ToVector2(), null, Color.Black, -(float)pos.Angle(), new Vector2(0, 1), new Vector2(3, (float)(blackkoef * pos.Length())), SpriteEffects.None, 1);
                        spriteBatch.Draw(connectiontexture, drawposition + (neuronvis.pos - pos * blackkoef).ToVector2(), null, connectioncolor, -(float)pos.Angle(), new Vector2(0, 1), new Vector2(3, (float)((1-blackkoef) * pos.Length())), SpriteEffects.None, 1);
                        convis.pos = (neuronvis.pos + neuronvisualizers[convis.connection.to_neuron].pos) / 2;
                        convis.Draw(spriteBatch, drawposition);
                    }
                }
            }
            foreach (var neuronvis in neuronvisualizers)
            {
                if (neuronvis.neuron.alive)
                {
                    spriteBatch.Draw(neurontexture, drawposition + neuronvis.pos.ToVector2(), null, neuronvis.color, 0f, new Vector2(neurontexture.Width / 2, neurontexture.Height / 2), 1f, SpriteEffects.None, 1);
                    neuronvis.Draw(spriteBatch, drawposition, textcolor);
                }
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace quiz_patente
{
    public class Domanda
    {
        public string Testo;
        public string[] Risposte;
        public int Corretta;
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;

        Texture2D sfondo;

        List<Domanda> domande = new List<Domanda>();
        int indiceDomanda = 0;

        int punteggio = 0;
        bool fineGioco = false;
        string messaggio = "";
        string esitoFinale = "";

        KeyboardState oldKeyboard;
        Random random = new Random();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            sfondo = Content.Load<Texture2D>("sfondo");

            CaricaDomande();
            MischiaDomande();
        }

        void CaricaDomande()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "domande.csv");

            foreach (var linea in File.ReadAllLines(path))
            {
                var parti = linea.Split(';');

                Domanda d = new Domanda();
                d.Testo = parti[0];
                d.Risposte = new string[] { parti[1], parti[2], parti[3] };
                d.Corretta = int.Parse(parti[4]);

                domande.Add(d);
            }
        }

        void MischiaDomande()
        {
            domande = domande.OrderBy(x => random.Next()).ToList();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            if (!fineGioco)
            {
                if (KeyPressed(Keys.D1, keyboard)) Rispondi(0);
                if (KeyPressed(Keys.D2, keyboard)) Rispondi(1);
                if (KeyPressed(Keys.D3, keyboard)) Rispondi(2);
            }

            oldKeyboard = keyboard;
            base.Update(gameTime);
        }

        void Rispondi(int scelta)
        {
            if (scelta == domande[indiceDomanda].Corretta)
            {
                punteggio++;
                messaggio = "Risposta CORRETTA!";
            }
            else
            {
                punteggio--;
                messaggio = "Risposta SBAGLIATA!";
            }

            indiceDomanda++;

            if (indiceDomanda >= domande.Count)
                FineGioco();
        }

        void FineGioco()
        {
            fineGioco = true;
            esitoFinale = punteggio >= domande.Count / 2 ? "PROMOSSO!" : "BOCCIATO!";
        }

        bool KeyPressed(Keys key, KeyboardState keyboard)
        {
            return keyboard.IsKeyDown(key) && oldKeyboard.IsKeyUp(key);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _spriteBatch.Draw(sfondo, new Rectangle(0, 0, 1280, 720), Color.White);

            if (!fineGioco)
            {
                var d = domande[indiceDomanda];

                _spriteBatch.DrawString(font, d.Testo, new Vector2(50, 50), Color.White);

                for (int i = 0; i < 3; i++)
                {
                    _spriteBatch.DrawString(
                        font,
                        (i + 1) + ") " + d.Risposte[i],
                        new Vector2(50, 130 + i * 40),
                        Color.White
                    );
                }

                _spriteBatch.DrawString(font, "Punteggio: " + punteggio, new Vector2(50, 300), Color.Yellow);
                _spriteBatch.DrawString(font, messaggio, new Vector2(50, 340), Color.White);
            }
            else
            {
                _spriteBatch.DrawString(font, esitoFinale, new Vector2(500, 350), Color.Red);
                _spriteBatch.DrawString(font, "Punteggio finale: " + punteggio, new Vector2(470, 400), Color.White);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}